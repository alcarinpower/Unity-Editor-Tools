using CodeDestroyer.Editor.EditorTools;
using CodeDestroyer.Editor.UIElements;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeDestroyer.Editor.ToolsManager
{
    internal sealed class EditorToolsSettings : EditorWindow
    {
        private static readonly Vector2 windowSize = new Vector2(1020f, 610f);

        private List<TreeViewItemData<string>> projectSettingsList = new List<TreeViewItemData<string>>();
        private Dictionary<string, VisualElement> rootDict = new Dictionary<string, VisualElement>();

        [MenuItem("Tools/Code Destroyer/Editor Tools")]
        private static void ShowWindow()
        {
            EditorToolsSettings settingsWindow = GetWindow<EditorToolsSettings>();
            settingsWindow.titleContent.text = "Editor Tools Settings";
            settingsWindow.titleContent.image = EditorGUIUtility.FindTexture("SettingsIcon");
            Rect main = EditorGUIUtility.GetMainWindowPosition();
            Vector2 center = main.center;

            settingsWindow.position = new Rect(
                center.x - windowSize.x * 0.5f,
                center.y - windowSize.y * 0.5f,
                windowSize.x,
                windowSize.y
            );
        }

        public void CreateGUI()
        {
            // Libraries
            TreeViewItemData<string> librariesSetting = new TreeViewItemData<string>(0, GlobalVariables.LibrariesName);
            // ----------------------

            // Tools
            List<TreeViewItemData<string>> toolChildren = new List<TreeViewItemData<string>>();
            TreeViewItemData<string> packageInitializerSetting = new TreeViewItemData<string>(1, GlobalVariables.PackagesInitializerName);
            TreeViewItemData<string> roughnessConverterSetting = new TreeViewItemData<string>(2, GlobalVariables.RoughnessConverterName);

            TreeViewItemData<string> toolsSetting = new TreeViewItemData<string>(3, GlobalVariables.ToolsName, toolChildren);
            // ----------------------

            // Utilities
            TreeViewItemData<string> utilitiesSetting = new TreeViewItemData<string>(4, GlobalVariables.UtilitiesName);
            // ----------------------
            List<TreeViewItemData<string>> _2DChildren = new List<TreeViewItemData<string>>();
            TreeViewItemData<string> _2DSetting = new TreeViewItemData<string>(6, GlobalVariables._2DName, _2DChildren);


#if HAS_SPRITE2D
            // 2D

            TreeViewItemData<string> spriteEditor = new TreeViewItemData<string>(5, GlobalVariables.SpriteEditorName);
            _2DChildren.Add(spriteEditor);
#endif

            // 3D
            List<TreeViewItemData<string>> _3DChildren = new List<TreeViewItemData<string>>();
            TreeViewItemData<string> _3DSetting = new TreeViewItemData<string>(7, GlobalVariables._3DName, _3DChildren);

            _3DChildren.Add(roughnessConverterSetting);
            toolChildren.Add(_2DSetting);
            toolChildren.Add(_3DSetting);
            toolChildren.Add(packageInitializerSetting);


            // Tools
            rootDict.Add(GlobalVariables.ToolsName, ToolsDocumentation.ToolsVisualElement());
            rootDict.Add(GlobalVariables.RoughnessConverterName, RoughnessConverter.ConvertRoughnessToMetallicSmoothnessVisualElement());
            rootDict.Add(GlobalVariables._2DName, null);
#if HAS_SPRITE2D
            rootDict.Add(GlobalVariables.SpriteEditorName, SpriteEditor.SpriteEditorVisualElement());
#endif
            rootDict.Add(GlobalVariables._3DName, null);
            rootDict.Add(GlobalVariables.PackagesInitializerName, PackageInitializer.PackageInitializerVisualElement());

            projectSettingsList.Add(toolsSetting);



            SettingsPanel settingsWindow = new SettingsPanel(ref projectSettingsList, ref rootDict);


            rootVisualElement.Add(settingsWindow);
        }
    }
}
