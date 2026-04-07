using System.IO;
using System;
using UnityEditor;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CodeDestroyer.Editor.UIElements"), InternalsVisibleTo("CodeDestroyer.Editor.EditorTools"), InternalsVisibleTo("CodeDestroyer.Editor.SpriteTools")]
namespace CodeDestroyer.Editor
{
    internal static class GlobalVariables
    {
        // Project Paths
        internal const string NickName = "Code Destroyer";
        internal const string ProjectName = "Editor Tools";
        internal const string DomainName = "com.codedestroyer";
        internal const string PackageName = DomainName + ".editortools";
        internal const string ProjectsPath = "Packages/" + PackageName + "/Editor/Projects";
        internal const string ProjectManagementPath = "Packages/" + PackageName + "/Editor/Project Managements";

        internal const string LibrariesName = "Libraries";
        internal const string ToolsName = "Tools";
        internal const string UtilitiesName = "Utilities";
        internal const string AttributesName = "Attributes";
        internal const string _2DName = "2D Tools";
        internal const string _3DName = "3D Tools";
        internal const string SpriteEditorName = "Sprite Editor";
        internal const string UIElementsName = "UI Elements";
        internal const string GitDependencyManagerName = "GitDependencyManager";
        internal const string RemoveDefineSymbolsFromBuildName = "RemoveScriptingDefineSymbolsFromBuild";

        internal const string RoughnessConverterName = "Roughness Converter";
        internal const string PackagesInitializerName = "Package Initializer";

        internal const string UnityLogoIconName = "d_UnityLogo";
        internal const string UnityInfoIconName = "console.infoicon@2x";
        internal const string UnityErrorIconName = "Error@2x";
        internal const string UnityWarnIconName = "Warning@2x";
        internal const string UnityInstalledIconName = "Installed@2x";



        internal static readonly string ListViewFoldoutStyleName = "unity-list-view__foldout-header";


        private static string windowsAssetStorePackagePath = @"Unity\Asset Store-5.x";
        private static string OsxEditorPackagePath = @"Library/Unity/Asset Store-5.x";
        private static string LinuxEditorPackagePath = ".local/share/unity3d/Asset Store-5.x";
        private static string AssetStorePath;
        internal static string CurrentAssetStorePath
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    AssetStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), windowsAssetStorePackagePath);
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    AssetStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), OsxEditorPackagePath);
                }
                else if (Application.platform == RuntimePlatform.LinuxEditor)
                {
                    AssetStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LinuxEditorPackagePath);
                }
                else
                {
                    Debug.Log("Unsupported platform for Asset Store cache location.");
                }

                return AssetStorePath;
            }
        }




        private static readonly Color defaultLineDarkColor = new Color(0.1215686f, 0.1215686f, 0.1215686f, 1f);
        private static readonly Color defaultLineWhiteColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        internal static Color DefaultLineColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return defaultLineDarkColor;
                }
                else
                {
                    return defaultLineWhiteColor;
                }
            }
        }

        private static readonly Color defaultBackgroundDarkColor = new Color(0.2352941f, 0.2352941f, 0.2352941f, 1f);
        private static readonly Color defaultBackgroundWhiteColor = new Color(0.7843138f, 0.7843138f, 0.7843138f, 1f);
        internal static Color DefaultBackgroundColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                {
                    return defaultBackgroundDarkColor;
                }
                else
                {
                    return defaultBackgroundWhiteColor;
                }
            }
        }
    }
}
