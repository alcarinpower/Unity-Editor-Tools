using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using CodeDestroyer.Editor.UIElements;

namespace CodeDestroyer.Editor.EditorTools
{
    internal sealed class RoughnessConverter
    {
        private static Texture2D metallicMap;
        private static Texture2D roughnessMap;

        private static Header rougnhessConverterLabel;
        private static ObjectField metallicField;
        private static ObjectField roughnessField;

        private static readonly int globalMarginLeftRight = 15;

        internal static VisualElement ConvertRoughnessToMetallicSmoothnessVisualElement()
        {
            VisualElement rootVisualElement = new VisualElement();

            VisualElement spacer = new VisualElement();
            spacer.style.height = 5f;
            spacer.style.whiteSpace = WhiteSpace.Normal;


            rougnhessConverterLabel = new Header();
            rougnhessConverterLabel.text = GlobalVariables.RoughnessConverterName;
            rougnhessConverterLabel.style.marginBottom = 15;
            rougnhessConverterLabel.style.marginLeft = globalMarginLeftRight;

            // Metallic Map Field
            VisualElement metallicRow = new VisualElement();
            metallicRow.style.flexDirection = FlexDirection.Row;
            metallicRow.style.marginBottom = 10;
            metallicRow.style.marginLeft = globalMarginLeftRight;
            metallicRow.style.justifyContent = Justify.SpaceBetween;

            Label metallicLabel = new Label("Metallic Map");
            metallicLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            metallicLabel.style.flexGrow = 1;

            metallicField = new ObjectField();
            metallicField.objectType = typeof(Texture2D);
            metallicField.allowSceneObjects = false;
            metallicField.style.width = 300f;
            metallicField.style.flexGrow = 0;
            metallicField.style.alignSelf = Align.FlexEnd;
            metallicField.style.marginRight = globalMarginLeftRight;
            metallicField.RegisterValueChangedCallback(evt =>
            {
                metallicMap = evt.newValue as Texture2D;
            });

            metallicRow.Add(metallicLabel);
            metallicRow.Add(metallicField);


            // Roughness Map Field
            VisualElement roughnessRow = new VisualElement();
            roughnessRow.style.flexDirection = FlexDirection.Row;
            roughnessRow.style.marginBottom = 10;
            roughnessRow.style.marginLeft = globalMarginLeftRight;
            roughnessRow.style.justifyContent = Justify.SpaceBetween;

            Label roughnessLabel = new Label("Roughness Map");
            roughnessLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            roughnessLabel.style.flexGrow = 1;

            roughnessField = new ObjectField();
            roughnessField.objectType = typeof(Texture2D);
            roughnessField.allowSceneObjects = false;
            roughnessField.style.width = 300f;
            roughnessField.style.flexGrow = 0;
            roughnessField.style.alignSelf = Align.FlexEnd;
            roughnessField.style.marginRight = globalMarginLeftRight;
            roughnessField.RegisterValueChangedCallback(evt =>
            {
                roughnessMap = evt.newValue as Texture2D;
            });





            Label saveLabel = new Label("Will be saved to: ");
            saveLabel.style.marginLeft = globalMarginLeftRight;
            saveLabel.style.whiteSpace = WhiteSpace.Normal;

            metallicField.RegisterValueChangedCallback((evt) =>
            {
                string assetPath = AssetDatabase.GetAssetPath(metallicMap);
                string fileName = Path.ChangeExtension(assetPath, null) + " Combined.png";

                saveLabel.text = "Will be saved to: " + fileName;
            });
            roughnessField.RegisterValueChangedCallback((evt) =>
            {
                if (metallicMap == null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(roughnessMap);
                    string fileName = Path.ChangeExtension(assetPath, null) + " Combined.png";

                    saveLabel.text = "Will be saved to: " + fileName;
                }
            });


            // Create Button
            Button createButton = new Button();
            createButton.text = "Create Metallic Smoothness";
            createButton.style.marginLeft = globalMarginLeftRight;
            createButton.style.marginRight = globalMarginLeftRight;
            createButton.clicked += () =>
            {
                if (metallicMap != null)
                {
                    SetReadWriteEnabledFlag(metallicMap, true);
                }
            
                if (roughnessMap != null)
                {
                    SetReadWriteEnabledFlag(roughnessMap, true);
                }

                if (roughnessMap == null)
                {
                    Debug.LogError("You don't have roughness map. there is no point to create metallic map!");
                    return;
                }
                if (metallicMap == null && roughnessMap != null)
                {
                    CreateSmoothnessMap(roughnessMap);
                }
                else if (metallicMap != null && roughnessMap != null)
                {
                    CreateMetallicAndSmoothnessMap(metallicMap, roughnessMap);
                }
                else if (roughnessMap == null && metallicMap == null)
                {
                    Debug.LogError("You don't have roughness or metallic map!");
                }
            };

            rootVisualElement.Add(spacer);
            rootVisualElement.Add(rougnhessConverterLabel);
            rootVisualElement.Add(metallicRow);
            roughnessRow.Add(roughnessLabel);
            roughnessRow.Add(roughnessField);
            rootVisualElement.Add(roughnessRow);
            rootVisualElement.Add(saveLabel);
            rootVisualElement.Add(createButton);


            return rootVisualElement;
        }
        private static bool SetReadWriteEnabledFlag(Texture targetTexture, bool isReadable)
        {
            string assetPath = AssetDatabase.GetAssetPath(targetTexture);
            if (string.IsNullOrEmpty(assetPath))
                return false;
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                return false;
            if (importer.isReadable != isReadable)
            {
                importer.isReadable = isReadable;
                importer.SaveAndReimport();
                AssetDatabase.SaveAssets();
            }
            return true;
        }
        private static void CreateSmoothnessMap(Texture2D roughnessMap)
        {
            Texture2D smoothnessMap = new Texture2D(roughnessMap.width, roughnessMap.height);
            Color[] roughnessPixels = roughnessMap.GetPixels();

            for (int i = 0; i < roughnessPixels.Length; i++)
            {
                roughnessPixels[i].r = 1f - roughnessPixels[i].r;

                roughnessPixels[i].a = 1f;
            }

            smoothnessMap.SetPixels(roughnessPixels);
            smoothnessMap.Apply();

            Texture2D createdMetallicMap = new Texture2D(roughnessMap.width, roughnessMap.height);
            Color[] smoothnessPixels = smoothnessMap.GetPixels();

            for (int i = 0; i < smoothnessPixels.Length; i++)
            {
                float smoothness = smoothnessPixels[i].r;

                smoothnessPixels[i] = new Color(0f, 0f, 0f, smoothness);
            }

            createdMetallicMap.SetPixels(smoothnessPixels);
            createdMetallicMap.Apply();

            string assetPath = AssetDatabase.GetAssetPath(roughnessMap);
            string directory = Path.GetDirectoryName(assetPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            string newFileName = $"{directory}/{fileNameWithoutExtension}_Combined.png";
            byte[] bytes = createdMetallicMap.EncodeToPNG();

            File.WriteAllBytes(newFileName, bytes);

            AssetDatabase.ImportAsset(newFileName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Smoothness map saved as {newFileName}");
        }

        private static void CreateMetallicAndSmoothnessMap(Texture2D metallicMap, Texture2D roughnessMap)
        {
            Texture2D smoothnessMap = new Texture2D(roughnessMap.width, roughnessMap.height);
            Color[] roughnessPixels = roughnessMap.GetPixels();

            for (int i = 0; i < roughnessPixels.Length; i++)
            {
                roughnessPixels[i].r = 1f - roughnessPixels[i].r;
                roughnessPixels[i].a = 1f;
            }

            smoothnessMap.SetPixels(roughnessPixels);
            smoothnessMap.Apply();

            Texture2D createdMetallicMap = new Texture2D(metallicMap.width, metallicMap.height);
            Color[] metallicPixels = metallicMap.GetPixels();
            Color[] smoothnessPixels = smoothnessMap.GetPixels();

            for (int i = 0; i < metallicPixels.Length; i++)
            {
                float metallic = metallicPixels[i].r;

                float smoothness = smoothnessPixels[i].r;

                metallicPixels[i] = new Color(metallic, 0, 0, smoothness);
            }

            createdMetallicMap.SetPixels(metallicPixels);
            createdMetallicMap.Apply();

            string assetPath = AssetDatabase.GetAssetPath(metallicMap);
            string fileName = $"{Path.ChangeExtension(assetPath, null)}_Combined.png";
            byte[] bytes = createdMetallicMap.EncodeToPNG();

            File.WriteAllBytes(fileName, bytes);

            AssetDatabase.ImportAsset(fileName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}