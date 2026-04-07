using UnityEngine;
using UnityEngine.UIElements;
using CodeDestroyer.Editor.UIElements;

namespace CodeDestroyer.Editor.EditorTools
{
    internal sealed class ToolsDocumentation
    {
        private static readonly float marginLeftRight = 15f;


        private static readonly string spriteEditorInfo =
            "Sprite Editor can slice current selected sprites with to Grid By Cell Size and set pivots. Select all images you want and then click Slice or set pivots.";

        private static readonly string packagesInitializerInfo =
            $"{GlobalVariables.PackagesInitializerName} will automatically install or remove built-in and Git packages based on the enabled " + $"toggles in the settings when " +
            $"{GlobalVariables.ProjectName} is first installed.\nFor safety reasons, Asset Store packages will only be installed, and any untoggled Asset Store " +
            $"packages will not be removed.";

        private static readonly string roughnessConverterInfo =
            "The Roughness Converter allows you to generate a Metallic Smoothness Map by combining a Metallic Map with a Roughness Map.\n" +
            "Alternatively, you can create a Smoothness Map directly from a Roughness Map.";


        internal static VisualElement ToolsVisualElement()
        {
            VisualElement rootVisualElement = new VisualElement();

            Header toolsHeader = new Header();
            toolsHeader.text = GlobalVariables.ToolsName;
            toolsHeader.style.marginTop = 5f;
            toolsHeader.style.marginLeft = marginLeftRight;
            toolsHeader.style.marginBottom = marginLeftRight;



            VisualElement spriteEditor = MakeDocumentationElement(GlobalVariables.SpriteEditorName, spriteEditorInfo);
            VisualElement packageInitializer = MakeDocumentationElement(GlobalVariables.PackagesInitializerName, packagesInitializerInfo);
            VisualElement rougnhessConverter = MakeDocumentationElement(GlobalVariables.RoughnessConverterName, roughnessConverterInfo);


            rootVisualElement.Add(toolsHeader);
            rootVisualElement.Add(spriteEditor);
            rootVisualElement.Add(packageInitializer);
            rootVisualElement.Add(rougnhessConverter);
            return rootVisualElement;
        }



        private static VisualElement MakeDocumentationElement(string documentationHeader, string documetationLabel)
        {
            VisualElement visualElement = new VisualElement();
            Foldout oneDocumentation = new Foldout();
            oneDocumentation.text = documentationHeader;
            oneDocumentation.style.fontSize = 13;
            oneDocumentation.style.unityFontStyleAndWeight = FontStyle.Bold;
            oneDocumentation.style.marginLeft = marginLeftRight;
            oneDocumentation.style.marginBottom = 4f;
            oneDocumentation.AddToClassList(GlobalVariables.ListViewFoldoutStyleName);

            InfoBox documentationInfoBox = new InfoBox(documetationLabel, InfoBoxIconType.None, 0f);
            documentationInfoBox.style.marginBottom = 5f;
            documentationInfoBox.style.marginLeft = marginLeftRight;
            documentationInfoBox.style.marginRight = marginLeftRight;
            documentationInfoBox.style.whiteSpace = WhiteSpace.Normal;

            oneDocumentation.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    documentationInfoBox.style.display = DisplayStyle.Flex;
                }
                else
                {
                    documentationInfoBox.style.display = DisplayStyle.None;
                }

            });


            visualElement.Add(oneDocumentation);
            visualElement.Add(documentationInfoBox);
            return visualElement;
        }
    }
}