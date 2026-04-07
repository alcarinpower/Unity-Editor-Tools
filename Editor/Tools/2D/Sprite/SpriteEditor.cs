#if HAS_SPRITE2D
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("CodeDestroyer.Editor.EditorTools")]
namespace CodeDestroyer.Editor.SpriteTools
{
    internal sealed class SpriteEditor
    {
        private static readonly int globalMarginLeftRight = 15;

        internal static VisualElement SpriteEditorVisualElement()
        {
            VisualElement rootVisualElement = new VisualElement();

            VisualElement spacer = new VisualElement();
            spacer.style.height = 5f;
            spacer.style.whiteSpace = WhiteSpace.Normal;


            Label toolLabel = new Label(GlobalVariables.SpriteEditorName);
            toolLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            toolLabel.style.fontSize = 18;
            toolLabel.style.whiteSpace = WhiteSpace.Normal;
            toolLabel.style.marginLeft = globalMarginLeftRight;
            toolLabel.style.marginBottom = 10f;



            Button sliceButton = new Button();
            FloatField xSliceFloatField = new FloatField();
            FloatField ySlicefloatField = new FloatField();
            sliceButton.text = "Slice";
            sliceButton.clicked += delegate
            {
                if (xSliceFloatField.value == 0 || ySlicefloatField.value == 0)
                {
                    Debug.LogError($"Slice X or Slice Y cannot be {xSliceFloatField.value}.");

                    return;
                }
                UnityEngine.Object[] textures = Selection.objects;


                for (int i = 0; i < textures.Length; i++)
                {
                    string texturePath = AssetDatabase.GetAssetPath(textures[i]);
                    SliceSprite(texturePath, (int)xSliceFloatField.value, (int)ySlicefloatField.value);
                }
            };

            VisualElement spriteSlicercontainer = new VisualElement();
            spriteSlicercontainer.style.borderLeftWidth = globalMarginLeftRight;
            spriteSlicercontainer.style.borderRightWidth = globalMarginLeftRight;

            VisualElement spritePivotChangercontainer = new VisualElement();
            spritePivotChangercontainer.style.borderTopWidth = 15f;
            spritePivotChangercontainer.style.borderLeftWidth = globalMarginLeftRight;
            spritePivotChangercontainer.style.borderRightWidth = globalMarginLeftRight;

            EnumField spriteAlignment = new EnumField("Pivot");
            spriteAlignment.Init(SpriteAlignment.BottomCenter);

            spriteAlignment.RegisterCallback<ChangeEvent<SpriteAlignment>>(evt =>
            {
                spriteAlignment.value = evt.newValue;
            });

            EnumField pivotUnitModeEnum = new EnumField("Pivot Unit Mode");
            pivotUnitModeEnum.Init(PivotUnits.Normalized);

            pivotUnitModeEnum.RegisterCallback<ChangeEvent<Pivot>>(evt =>
            {
                pivotUnitModeEnum.value = evt.newValue;
            });

            Vector2Field customPivot = new Vector2Field("Custom Pivot");
            customPivot.SetEnabled(false);

            Button setPivotsButton = new Button();
            setPivotsButton.text = "Set Pivots";
            setPivotsButton.clicked += delegate
            {
                UnityEngine.Object[] textures = Selection.objects;

                if (textures.Length == 0)
                {
                    Debug.LogWarning($"No Selected Textures");
                    return;
                }

                for (int i = 0; i < textures.Length; i++)
                {
                    string texturePath = AssetDatabase.GetAssetPath(textures[i]);
                    SetSpritePivots(texturePath, (SpriteAlignment)spriteAlignment.value, customPivot.value.x, customPivot.value.y, (PivotUnits)pivotUnitModeEnum.value);
                }
            };


            spriteAlignment.RegisterValueChangedCallback(evt =>
            {
                if ((SpriteAlignment)evt.newValue == SpriteAlignment.Custom)
                {
                    customPivot.SetEnabled(true);
                }
                else
                {
                    customPivot.SetEnabled(false);
                }

            });



            spritePivotChangercontainer.Add(spriteAlignment);
            spritePivotChangercontainer.Add(pivotUnitModeEnum);
            spritePivotChangercontainer.Add(customPivot);
            spritePivotChangercontainer.Add(setPivotsButton);

            Label xSliceLabel = new Label("Slice X");
            Label ySliceLabel = new Label("Slice Y");


            spriteSlicercontainer.Add(xSliceLabel);
            spriteSlicercontainer.Add(xSliceFloatField);

            spriteSlicercontainer.Add(ySliceLabel);
            spriteSlicercontainer.Add(ySlicefloatField);
            spriteSlicercontainer.Add(sliceButton);

            rootVisualElement.Add(spacer);
            rootVisualElement.Add(toolLabel);
            rootVisualElement.Add(spriteSlicercontainer);
            rootVisualElement.Add(spritePivotChangercontainer);
            return rootVisualElement;
        }





        private static void SliceSprite(string texturePath, int sizeX, int sizeY)
        {
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError($"Importer at {importer.assetPath} is null!");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            //importer.isReadable = true;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
                return;

            SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
            factory.Init();

            ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();

            List<SpriteRect> spriteRects = new List<SpriteRect>();
            List<SpriteNameFileIdPair> nameFileIdPairs = new List<SpriteNameFileIdPair>();

            int frameNumber = 0;

            for (int y = texture.height - sizeY; y >= 0; y -= sizeY)
            {
                for (int x = 0; x <= texture.width - sizeX; x += sizeX)
                {
                    GUID spriteId = GUID.Generate();

                    SpriteRect spriteRect = new SpriteRect
                    {
                        name = $"{texture.name}_{frameNumber}",
                        spriteID = spriteId,
                        rect = new Rect(x, y, sizeX, sizeY),
                        alignment = SpriteAlignment.Custom,
                        pivot = Vector2.zero
                    };

                    spriteRects.Add(spriteRect);
                    nameFileIdPairs.Add(new SpriteNameFileIdPair(spriteRect.name, spriteRect.spriteID));
                    frameNumber++;
                }
            }

            dataProvider.SetSpriteRects(spriteRects.ToArray());

            ISpriteNameFileIdDataProvider nameFileIdProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            nameFileIdProvider.SetNameFileIdPairs(nameFileIdPairs);

            dataProvider.Apply();
            importer.SaveAndReimport();
            
            Debug.Log($"Sliced {frameNumber} sprites from {texturePath}");
        }



        private static void SetSpritePivots(string texturePath, SpriteAlignment alignment, float customPivotX = 0.5f, float customPivotY = 0.5f, PivotUnits pivotUnits = PivotUnits.Normalized)
        {
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError($"Importer at {importer.assetPath} is null!");
                return;
            }

            SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
            factory.Init();

            ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();

            SpriteRect[] spriteRects = dataProvider.GetSpriteRects();
            if (spriteRects == null || spriteRects.Length == 0)
                return;

            for (int i = 0; i < spriteRects.Length; i++)
            {
                Rect rect = spriteRects[i].rect;

                spriteRects[i].alignment = alignment;

                if (alignment == SpriteAlignment.Custom)
                {
                    spriteRects[i].pivot = GetPivot(
                        customPivotX,
                        customPivotY,
                        Mathf.RoundToInt(rect.width),
                        Mathf.RoundToInt(rect.height),
                        pivotUnits
                    );
                }
            }

            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.Apply();
            importer.SaveAndReimport();

            Debug.Log($"Pivots are set.");
        }
        private enum PivotUnits
        {
            Normalized,
            Pixels
        }

        private static Vector2 GetPivot(float pivotX, float pivotY, int spriteWidth, int spriteHeight, PivotUnits units)
        {
            if (units == PivotUnits.Pixels)
            {
                return new Vector2(
                    pivotX / spriteWidth,
                    pivotY / spriteHeight
                );
            }

            return new Vector2(pivotX, pivotY);
        }

    }
}
#endif
