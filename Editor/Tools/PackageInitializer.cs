using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEditor;
using System.Linq;
using System.IO;
using CodeDestroyer.Editor.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace CodeDestroyer.Editor.EditorTools
{
    internal sealed class PackageInitializer
    {
        private static readonly string BuiltInListViewName = "Built-In Packages";
        private static readonly string GitListViewName = "Git Packages";
        private static readonly string AssetStoreListViewName = "Asset Store Packages";


        private static readonly string savePath = PackageInitializerSave.instance.GetSavePath();
        private static readonly int globalMarginLeftRight = 15;
        private static readonly int globalMiniBottomMargin = 7;


        private static readonly List<string> currentBuiltinPackageNames = new List<string>();
        private static ListRequest listBuiltInPackages;
        private static readonly List<string> removeCoreUnityPackages = new List<string>()
        {
            "com.unity.modules.hierarchycore",
            "com.unity.modules.subsystems",
            "com.unity.burst",
            "com.unity.collections",
            "com.unity.render-pipelines.core",
            "com.unity.ext.nunit",
            "com.unity.mathematics",
            "com.unity.nuget.mono-cecil",
            "com.unity.test-framework.performance",
            "com.unity.searcher",
            "com.unity.shadergraph",
            "com.unity.rendering.light-transport",
            "com.unity.render-pipelines.universal-config"
        };
        private static readonly List<string> resetBuiltInList = new List<string>()
        {
            "com.unity.ai.navigation",
            "com.unity.inputsystem",
            "com.unity.ide.rider",
            "com.unity.multiplayer.center",
            "com.unity.test-framework",
            "com.unity.timeline",
            "com.unity.ugui",
            "com.unity.render-pipelines.universal",
            "com.unity.collab-proxy",
            "com.unity.visualscripting",
            "com.unity.ide.visualstudio",
            "com.unity.modules.accessibility",
            "com.unity.modules.ai",
            "com.unity.modules.androidjni",
            "com.unity.modules.animation",
            "com.unity.modules.assetbundle",
            "com.unity.modules.audio",
            "com.unity.modules.cloth",
            "com.unity.modules.director",
            "com.unity.modules.imageconversion",
            "com.unity.modules.imgui",
            "com.unity.modules.jsonserialize",
            "com.unity.modules.particlesystem",
            "com.unity.modules.physics",
            "com.unity.modules.physics2d",
            "com.unity.modules.screencapture",
            "com.unity.modules.terrain",
            "com.unity.modules.terrainphysics",
            "com.unity.modules.tilemap",
            "com.unity.modules.ui",
            "com.unity.modules.uielements",
            "com.unity.modules.umbra",
            "com.unity.modules.unityanalytics",
            "com.unity.modules.unitywebrequest",
            "com.unity.modules.unitywebrequestassetbundle",
            "com.unity.modules.unitywebrequestaudio",
            "com.unity.modules.unitywebrequesttexture",
            "com.unity.modules.unitywebrequestwww",
            "com.unity.modules.vehicles",
            "com.unity.modules.video",
            "com.unity.modules.vr",
            "com.unity.modules.wind",
            "com.unity.modules.xr"
        };


        private static VisualElement builtInListViewHeaderContainer;
        private static VisualElement customListViewHeaderContainer;

        private static SearchRequest builtInPackageSearchRequest;
        private static AddAndRemoveRequest resetAddRemoveRequest;
        private static AddAndRemoveRequest addRemoveOfPackageInitializer;
        private static ListRequest listInstalledPackages;
        private static readonly List<PackageInfo> installedPackages = new List<PackageInfo>();

        private static VisualElement WholePackageInitializerContainer;

        private static List<Package> temporaryBuiltinPackagesList;
        private static List<string> builtInPackagesSearchList = new List<string>();
        private static List<string> builtInPackagesSearchResultList = new List<string>();
        private static ToolbarSearchPanel builtInPackagesSearchPanel = new ToolbarSearchPanel();

        private static List<Package> temporaryCustomPackagesList;
        private static List<string> customPackagesSearchlist = new List<string>();
        private static List<string> customPackagesSearchResultList = new List<string>();
        private static ToolbarSearchPanel customPackagesSearchPanel = new ToolbarSearchPanel();

        private static List<Package> temporaryassetStorePackagesList;
        private static List<string> assetStorePackagesSearchlist = new List<string>();
        private static List<string> assetStorePackagesSearchResultList = new List<string>();
        private static ToolbarSearchPanel assetStorePackagesSearchPanel = new ToolbarSearchPanel();
        
        private static ListView builtInPackagesListView;
        private static ListView customPackageListView;
        private static ListView assetStorePackagesListView;


        private static void UpdateProjectPackagesAccordingToPackageInitializer()
        {
            List<string> addList = new List<string>();
            List<string> removeList = new List<string>();
            if (PackageInitializerSave.instance != null)
            {
                // Install or remove built-in unity packages
                for (int i = 0; i < PackageInitializerSave.instance.builtInPackages.Count; i++)
                {
                    Package currentBuiltinPackage = PackageInitializerSave.instance.builtInPackages[i];
                    PackageInfo currentBuiltInPackageInfo = PackageInfo.FindForPackageName(currentBuiltinPackage.packageName);

                    if (currentBuiltinPackage.shouldPackageInstalled)
                    {
                        if (currentBuiltInPackageInfo == null)
                        {
                            addList.Add(currentBuiltinPackage.packageName);
                        }
                    }
                    else if (!currentBuiltinPackage.shouldPackageInstalled)
                    {
                        if (currentBuiltInPackageInfo != null)
                        {

                            if (!currentBuiltInPackageInfo.isDirectDependency)
                            {
                                Debug.LogWarning($"{GlobalVariables.PackagesInitializerName} Package: [{currentBuiltInPackageInfo.name}] is installed as dependency. It will be ignored.");
                            }
                            else
                            {
                                removeList.Add(currentBuiltinPackage.packageName);
                            }
                        }
                    }
                }

                // Install or remove git packages
                for (int i = 0; i < PackageInitializerSave.instance.customPackages.Count; i++)
                {
                    Package currentGitPackage = PackageInitializerSave.instance.customPackages[i];

                    if (string.IsNullOrEmpty(currentGitPackage.packageName))
                    {
                        Debug.LogError("Package name in the index: [" + i + "] is null. Cannot install null values. It will be ignored.");
                        continue;
                    }


                    PackageInfo customPackage = installedPackages.Find(package => package.packageId.Contains(currentGitPackage.packageName) && package.source == PackageSource.Git);

                    if (currentGitPackage.shouldPackageInstalled)
                    {
                        if (customPackage == null)
                        {
                            addList.Add(currentGitPackage.packageName);
                        }
                    }
                    else if (!currentGitPackage.shouldPackageInstalled)
                    {
                        if (customPackage != null)
                        {
                            if (!customPackage.isDirectDependency)
                            {
                                Debug.LogError($"{GlobalVariables.PackagesInitializerName} Package: [{customPackage.name}] is installed as dependency. It will be ignored. Do not try to remove it directly.");

                                Package package = PackageInitializerSave.instance.customPackages.Find((package) => package.packageName == currentGitPackage.packageName);

                                package.shouldPackageInstalled = true;
                                PackageInitializerSave.instance.Save();

                                customPackageListView.Rebuild();
                            }
                            else
                            {
                                removeList.Add(currentGitPackage.packageName);
                            }
                        }
                    }
                }



                if (addList.Count > 0 || removeList.Count > 0)
                {
                    string[] finalAddList = addList.ToArray();
                    string[] finalRemoveList = removeList.ToArray();

                    if (finalAddList.Length > 0)
                    {
                        Debug.Log(GlobalVariables.PackagesInitializerName + " Added Packages: " + string.Join(", ", finalAddList));
                    }
                    if (finalRemoveList.Length > 0)
                    {
                        Debug.Log(GlobalVariables.PackagesInitializerName + " Removed Packages: " + string.Join(", ", finalRemoveList));
                    }


                    addRemoveOfPackageInitializer = Client.AddAndRemove(finalAddList, finalRemoveList);
                    EditorApplication.update += AddOrRemoveProgress;
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }
            }
        }
        private static void ResetPackageInitializer()
        {
            List<string> resetAddList = new List<string>();
            List<string> resetRemoveList = new List<string>();

            for (int i = 0; i < PackageInitializerSave.instance.builtInPackages.Count; i++)
            {
                Package builtInPackage = PackageInitializerSave.instance.builtInPackages[i];


                PackageInfo packageInfo = PackageInfo.FindForPackageName(builtInPackage.packageName);
                if (!resetBuiltInList.Contains(builtInPackage.packageName) && packageInfo != null && packageInfo.isDirectDependency)
                {
                    resetRemoveList.Add(builtInPackage.packageName);
                }

                builtInPackage.shouldPackageInstalled = false;
            }

            for (int i = 0; i < resetBuiltInList.Count; i++)
            {
                string resetPackageName = resetBuiltInList[i];

                PackageInfo currentBuiltInPackageInfo = PackageInfo.FindForPackageName(resetPackageName);


                if (currentBuiltInPackageInfo == null)
                {
                    resetAddList.Add(resetPackageName);
                }

                Package foundedPackage = PackageInitializerSave.instance.builtInPackages.Find(_package => _package.packageName == resetPackageName);
                foundedPackage.shouldPackageInstalled = true;
            }


            PackageInitializerSave.instance.customPackages.Clear();

            for (int i = 0; i < PackageInitializerSave.instance.assetStorePackages.Count; i++)
            {
                Package assetStorePackage = PackageInitializerSave.instance.assetStorePackages[i];

                assetStorePackage.shouldPackageInstalled = false;
            }

            PackageInitializerSave.instance.Save();

            builtInPackagesListView.Rebuild();
            customPackageListView.Rebuild();
            assetStorePackagesListView.Rebuild();

            if (resetAddList.Count > 0 || resetRemoveList.Count > 0)
            {
                string[] finalAddList = resetAddList.ToArray();
                string[] finalRemoveList = resetRemoveList.ToArray();

                if (finalAddList.Length > 0)
                {
                    Debug.Log(GlobalVariables.PackagesInitializerName + " Added Packages: " + string.Join(", ", finalAddList));
                }
                if (finalRemoveList.Length > 0)
                {
                    Debug.Log(GlobalVariables.PackagesInitializerName + " Removed Packages: " + string.Join(", ", finalRemoveList));
                }

                
                resetAddRemoveRequest = Client.AddAndRemove(finalAddList, finalRemoveList);
                EditorApplication.update += ResetAddRemoveRequest;
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        private static void ApplyChanges()
        {
            if (PackageInitializerSave.instance != null)
            {
                if (!PackageInitializerSave.instance.isPackageInitializerEnabled) return;

                char sepChar = Path.DirectorySeparatorChar;
                string folder = Path.GetDirectoryName(Application.dataPath) + sepChar + GlobalVariables.DomainName;
                string path = folder + sepChar + GlobalVariables.PackagesInitializerName + ".flag";

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                if (!File.Exists(path))
                {
                    // Run
                    ListInstalledPackages();
                    File.WriteAllText(path, "Saved!");
                    // Run
                }
            }
        }
        internal static VisualElement PackageInitializerVisualElement()
        {
            VisualElement rootVisualElement = new VisualElement();


            VisualElement spacer = new VisualElement();
            spacer.style.height = 5f;
            spacer.style.whiteSpace = WhiteSpace.Normal;

            VisualElement toolLabelAndDisableContainer = new VisualElement();
            toolLabelAndDisableContainer.style.marginBottom = 5f;
            toolLabelAndDisableContainer.style.flexDirection = FlexDirection.Row;

            Label toolLabel = new Label(GlobalVariables.PackagesInitializerName);
            toolLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            toolLabel.style.fontSize = 18;
            toolLabel.style.whiteSpace = WhiteSpace.Normal;
            toolLabel.style.marginLeft = globalMarginLeftRight;


            Toggle disablePackageInitializer = new Toggle();
            disablePackageInitializer.style.alignSelf = Align.FlexEnd;
            disablePackageInitializer.value = PackageInitializerSave.instance.isPackageInitializerEnabled;

            VisualElement savePathContainer = new VisualElement();
            savePathContainer.style.flexDirection = FlexDirection.Row;
            savePathContainer.style.marginLeft = globalMarginLeftRight;
            savePathContainer.style.marginRight = globalMarginLeftRight;
            savePathContainer.style.marginBottom = globalMiniBottomMargin;

            Label savePathLabel = new Label("Save Path: ");

            savePathLabel.style.unityTextAlign = TextAnchor.MiddleLeft;


            TextField savePathTextField = new TextField();
            savePathTextField.style.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.3f);
            savePathTextField.value = savePath;
            savePathTextField.style.whiteSpace = WhiteSpace.Normal;
            savePathTextField.style.flexShrink = 1f;
            savePathTextField.isReadOnly = true;
            savePathTextField.selectAllOnMouseUp = true;
            savePathTextField.multiline = true;



            WholePackageInitializerContainer = new VisualElement();
            WholePackageInitializerContainer.SetEnabled(disablePackageInitializer.value);

            disablePackageInitializer.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Toggle Package Initializer");
                disablePackageInitializer.value = evt.newValue;
                WholePackageInitializerContainer.SetEnabled(evt.newValue);
                PackageInitializerSave.instance.isPackageInitializerEnabled = evt.newValue;
                PackageInitializerSave.instance.Save();

            });

            Undo.undoRedoPerformed += () =>
            {
                disablePackageInitializer.SetValueWithoutNotify(PackageInitializerSave.instance.isPackageInitializerEnabled);
                WholePackageInitializerContainer.SetEnabled(PackageInitializerSave.instance.isPackageInitializerEnabled);
            };


            if (PackageInitializerSave.instance.builtInPackages.Count == 0)
            {
                listBuiltInPackages = Client.List(false, true);
                EditorApplication.update += ListBuiltInPackagesProgress;
                EditorUtility.DisplayProgressBar(GlobalVariables.PackagesInitializerName, "Loading Built-In Packages", 0.3f);
            }
            else
            {
                BuiltInPackagesSearchPanel();
            }


            VisualElement builtInPackageList = BuiltInPackagesList();
            PackageInitializerSave.instance.Save();
            builtInPackageList.style.maxHeight = 300f;
            builtInPackageList.style.marginLeft = globalMarginLeftRight;
            builtInPackageList.style.marginRight = globalMarginLeftRight;
            builtInPackageList.style.marginBottom = globalMiniBottomMargin;


            CustomPackagesSearchPanel();

            VisualElement customPackageList = CustomPackageListView();
            customPackageList.style.maxHeight = 300f;
            customPackageList.style.marginLeft = globalMarginLeftRight;
            customPackageList.style.marginRight = globalMarginLeftRight;
            customPackageList.style.marginBottom = globalMiniBottomMargin;


            AssetStorePackagesSearchPanel();

            VisualElement assetStorePackageList = AssetStorePackagesListView();
            assetStorePackageList.style.maxHeight = 300f;
            assetStorePackageList.style.marginLeft = globalMarginLeftRight;
            assetStorePackageList.style.marginRight = globalMarginLeftRight;
            assetStorePackageList.style.marginBottom = globalMiniBottomMargin;



           


            Button updateButton = new Button();
            updateButton.text = "Update Packages";
            updateButton.style.marginLeft = globalMarginLeftRight;
            updateButton.style.marginRight = globalMarginLeftRight;
            updateButton.style.marginBottom = globalMiniBottomMargin;
            updateButton.clicked += () =>
            {
                ListInstalledPackages();
            };

            Button resetButton = new Button();
            resetButton.text = "Reset Packages";
            resetButton.style.marginLeft = globalMarginLeftRight;
            resetButton.style.marginRight = globalMarginLeftRight;
            resetButton.style.marginBottom = globalMiniBottomMargin;
            resetButton.clicked += () =>
            {
                if (EditorUtility.DisplayDialog("Reset Packages", "This will completely remove all of the updated built-in, git and asset store packages from project", "Apply", "Cancel"))
                {
                    ResetPackageInitializer();
                }
            };

            Button applyButton = new Button();
            applyButton.text = "Apply";
            applyButton.style.marginLeft = globalMarginLeftRight;
            applyButton.style.marginRight = globalMarginLeftRight;
            applyButton.style.marginBottom = globalMiniBottomMargin;
            applyButton.clicked += () =>
            {
                if (EditorUtility.DisplayDialog("Apply Changes", "This will install every selected packages and remove every deselected packages.", "Apply", "Cancel"))
                {
                    ApplyChanges();
                }
            };


            rootVisualElement.Add(spacer);
            toolLabelAndDisableContainer.Add(toolLabel);
            toolLabelAndDisableContainer.Add(disablePackageInitializer);
            rootVisualElement.Add(toolLabelAndDisableContainer);
            savePathContainer.Add(savePathLabel);
            savePathContainer.Add(savePathTextField);
            WholePackageInitializerContainer.Add(savePathContainer);
            WholePackageInitializerContainer.Add(updateButton);
            WholePackageInitializerContainer.Add(resetButton);
            WholePackageInitializerContainer.Add(applyButton);
            WholePackageInitializerContainer.Add(builtInPackageList);
            WholePackageInitializerContainer.Add(customPackageList);
            WholePackageInitializerContainer.Add(assetStorePackageList);

            rootVisualElement.Add(WholePackageInitializerContainer);
            return rootVisualElement;
        }
        


        // Built-In Packages
        private static void BuiltInPackagesSearchPanel()
        {
            builtInPackagesSearchList = new List<string>();
            builtInPackagesSearchResultList = new List<string>();

            for (int i = 0; i < PackageInitializerSave.instance.builtInPackages.Count; i++)
            {
                builtInPackagesSearchList.Add(PackageInitializerSave.instance.builtInPackages[i].packageName);
            }
            temporaryBuiltinPackagesList = new List<Package>();

            builtInPackagesSearchPanel = new ToolbarSearchPanel(builtInPackagesSearchList, builtInPackagesSearchResultList, BuiltInPackageSearchIsEmpty, BuiltInPackageSearchIsFilled);
            builtInPackagesSearchPanel.style.alignSelf = Align.FlexEnd;
        }
        private static void BuiltInPackageSearchIsFilled()
        {
            temporaryBuiltinPackagesList.Clear();
            for (int i = 0; i < builtInPackagesSearchResultList.Count; i++)
            {
                string resultPackageName = builtInPackagesSearchResultList[i];

                Package package = PackageInitializerSave.instance.builtInPackages.Find((_element) => _element.packageName == resultPackageName);

                if (package != null)
                {
                    temporaryBuiltinPackagesList.Add(package);
                }
            }
            builtInPackagesListView.itemsSource = temporaryBuiltinPackagesList;
            builtInPackagesListView.RefreshItems();
        }
        private static void BuiltInPackageSearchIsEmpty()
        {
            for (int i = 0; i < PackageInitializerSave.instance.builtInPackages.Count; i++)
            {
                Package package = PackageInitializerSave.instance.builtInPackages[i];
                Package tempPackage = temporaryBuiltinPackagesList.Find((_element) => _element.packageName == package.packageName);
                if (tempPackage != null)
                {
                    package.shouldPackageInstalled = tempPackage.shouldPackageInstalled;
                }
            }
            PackageInitializerSave.instance.Save();


            temporaryBuiltinPackagesList.Clear();
            builtInPackagesListView.itemsSource = PackageInitializerSave.instance.builtInPackages;
            builtInPackagesListView.RefreshItems();
        }
        private static VisualElement BuiltInPackagesList()
        {
            VisualElement rootVisualElement = new VisualElement();
            builtInPackagesListView = new ListView(PackageInitializerSave.instance.builtInPackages, 24, MakeItemBuiltInListView, BindBuiltInListView);

            builtInPackagesListView.selectionType = SelectionType.Single;
            builtInPackagesListView.style.flexGrow = 1;
            builtInPackagesListView.showFoldoutHeader = false;
            builtInPackagesListView.showBoundCollectionSize = false;

            builtInPackagesListView.showBorder = true;
            builtInPackagesListView.viewDataKey = "BuiltInPackages";

            builtInListViewHeaderContainer = new VisualElement();
            builtInListViewHeaderContainer.style.flexDirection = FlexDirection.Row;
            builtInListViewHeaderContainer.style.justifyContent = Justify.SpaceBetween;

            Foldout foldout = new Foldout();
            foldout.viewDataKey = "builtinFoldout";
            foldout.style.flexGrow = 1;
            foldout.text = BuiltInListViewName;
            foldout.value = true;

            foldout.AddToClassList(GlobalVariables.ListViewFoldoutStyleName);
            foldout.RegisterValueChangedCallback(evt =>
            {
                builtInPackagesListView.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            builtInListViewHeaderContainer.Add(foldout);
            if (PackageInitializerSave.instance.builtInPackages.Count != 0)
            {
                builtInListViewHeaderContainer.Add(builtInPackagesSearchPanel);
            }
            rootVisualElement.Add(builtInListViewHeaderContainer);
            rootVisualElement.Add(builtInPackagesListView);

            return rootVisualElement;
        }
        private static void BindBuiltInListView(VisualElement element, int index)
        {
            Toggle toggle = element.Q<Toggle>();
            Label label = element.Q<Label>();

            List<Package> builtInPackages = builtInPackagesListView.itemsSource as List<Package>;
            toggle.value = builtInPackages[index].shouldPackageInstalled;
            label.text = builtInPackages[index].packageName;

            toggle.RegisterCallback<ClickEvent>((clickEvent) =>
            {
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.currentTarget == toggle)
                    {
                        Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Toggle Package Install");

                        toggle.value = evt.newValue;
                        string currentTogglesLabel = (evt.currentTarget as Toggle).parent.Q<Label>().text;
                        Package package = PackageInitializerSave.instance.builtInPackages.Find(element => element.packageName == currentTogglesLabel);
                        package.shouldPackageInstalled = toggle.value;
                        PackageInitializerSave.instance.Save();
                    }
                });
            });

            label.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.currentTarget == label)
                {
                    Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Toggle Package Install");

                    toggle.value = !toggle.value;

                    Package package = PackageInitializerSave.instance.builtInPackages.Find(element => element.packageName == (evt.currentTarget as Label).text);
                    package.shouldPackageInstalled = toggle.value;
                    PackageInitializerSave.instance.Save();
                    evt.StopImmediatePropagation();
                }
            });
            Undo.undoRedoPerformed += () =>
            {
                if (index < builtInPackages.Count)
                {
                    toggle.SetValueWithoutNotify(builtInPackages[index].shouldPackageInstalled);
                }
            };
        }
        private static VisualElement MakeItemBuiltInListView()
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.paddingLeft = 10;
            row.style.paddingRight = 10;

            Toggle toggle = new Toggle();
            toggle.style.marginRight = 10;

            Label label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;

            row.Add(toggle);
            row.Add(label);

            return row;
        }
        // --------------------------------------------------------------------------------------



        // Custom Packages (Git Packages)
        private static void CustomPackagesSearchPanel()
        {
            customPackagesSearchlist = new List<string>();
            customPackagesSearchResultList = new List<string>();

            for (int i = 0; i < PackageInitializerSave.instance.customPackages.Count; i++)
            {
                customPackagesSearchlist.Add(PackageInitializerSave.instance.customPackages[i].packageName);
            }
            temporaryCustomPackagesList = new List<Package>();

            customPackagesSearchPanel = new ToolbarSearchPanel(customPackagesSearchlist, customPackagesSearchResultList, CustomPackageSearchIsEmpty, CustomPackageSearchIsFilled);
            customPackagesSearchPanel.style.alignSelf = Align.FlexEnd;
        }
        private static void CustomPackageSearchIsFilled()
        {
            temporaryCustomPackagesList.Clear();
            for (int i = 0; i < customPackagesSearchResultList.Count; i++)
            {
                string resultPackageName = customPackagesSearchResultList[i];
                Package package = PackageInitializerSave.instance.customPackages.Find((_element) => _element.packageName == resultPackageName);

                if (package != null)
                {
                    temporaryCustomPackagesList.Add(package);
                }
            }
            
            customPackageListView.bindItem = BindCustomPackagesForSearchField;
            customPackageListView.itemsSource = temporaryCustomPackagesList;
            customPackageListView.Rebuild();
        }
        private static void CustomPackageSearchIsEmpty()
        {
            for (int i = 0; i < PackageInitializerSave.instance.customPackages.Count; i++)
            {
                Package package = PackageInitializerSave.instance.customPackages[i];
                Package tempPackage = temporaryCustomPackagesList.Find((_element) => _element.packageName == package.packageName);
                if (tempPackage != null)
                {

                    package.shouldPackageInstalled = tempPackage.shouldPackageInstalled;
                }
            }
            PackageInitializerSave.instance.Save();

            temporaryCustomPackagesList.Clear();
            customPackageListView.itemsSource = PackageInitializerSave.instance.customPackages;
            customPackageListView.Rebuild();
        }
        private static void BindCustomPackagesForSearchField(VisualElement element, int index)
        {
            Toggle toggle = element.Q<Toggle>();
            TextField textField = element.Q<TextField>();

            List<Package> customPackages = customPackageListView.itemsSource as List<Package>;
            toggle.value = customPackages[index].shouldPackageInstalled;
            textField.value = customPackages[index].packageName;


            toggle.RegisterCallback<ClickEvent>((clickEvent) =>
            {
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.currentTarget == toggle)
                    {
                        customPackages[index].shouldPackageInstalled = evt.newValue;
                        PackageInitializerSave.instance.Save();
                    }
                });
            });

            textField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (index < PackageInitializerSave.instance.customPackages.Count)
                {
                    customPackages[index].packageName = evt.newValue;
                    PackageInitializerSave.instance.Save();
                }
            });


            textField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (!customPackagesSearchlist.Contains(textField.value))
                {
                    customPackagesSearchlist.Add(textField.value);
                }
                RemoveAndAddToCustomPackageSearchElement();
            });
        }
        private static VisualElement CustomPackageListView()
        {
            VisualElement rootVisualElement = new VisualElement();

            customPackageListView = new ListView(PackageInitializerSave.instance.customPackages, 24, MakeItemCustomPackageListView, BindCustomPackageListView);

            customPackageListView.onAdd = view =>
            {
                Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Add Package");

                view.itemsSource.Add(new Package());
                PackageInitializerSave.instance.Save();

                customPackageListView.Rebuild();
            };
            customPackageListView.onRemove = view =>
            {
                List<Package> list = view.itemsSource as List<Package>;

                if (view.selectedItem == null)
                {
                    int itemsSourceCount = view.itemsSource.Count;
                    if (itemsSourceCount - 1 != -1)
                    {
                        Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Remove Package");

                        list.RemoveAt(itemsSourceCount - 1);
                        PackageInitializerSave.instance.Save();
                        customPackageListView.Rebuild();
                    }
                }
                else
                {
                    Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Remove Package");

                    list.RemoveAt(view.selectedIndex);
                    PackageInitializerSave.instance.Save();
                    customPackageListView.Rebuild();
                }
            };
            Undo.undoRedoPerformed += () =>
            {
                customPackageListView.Rebuild();
            };
            customPackageListView.itemsAdded += addedItems =>
            {
                PackageInitializerSave.instance.Save();
                customPackageListView.Rebuild();
            };
            customPackageListView.itemsRemoved += removedItems =>
            {
                if (removedItems.Count() == PackageInitializerSave.instance.customPackages.Count)
                {
                    EditorApplication.update += RemoveItemsWhenZeroEnteredToCollectionSize;
                    return;
                }
                PackageInitializerSave.instance.Save();
                customPackageListView.Rebuild();
            };


            customPackageListView.viewDataKey = "Custom Packages";
            customPackageListView.selectionType = SelectionType.Single;
            customPackageListView.style.flexGrow = 1;
            customPackageListView.showFoldoutHeader = false;
            customPackageListView.showAddRemoveFooter = true;
            customPackageListView.showBoundCollectionSize = true;
            customPackageListView.showBorder = true;


            customListViewHeaderContainer = new VisualElement();
            customListViewHeaderContainer.style.flexDirection = FlexDirection.Row;
            customListViewHeaderContainer.style.justifyContent = Justify.SpaceBetween;
            Foldout foldout = new Foldout();
            foldout.viewDataKey = "GitPackagesFoldout";

            foldout.style.flexGrow = 1;
            foldout.text = GitListViewName;
            foldout.value = true;

            foldout.AddToClassList(GlobalVariables.ListViewFoldoutStyleName);
            foldout.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    customPackageListView.Rebuild();
                }
                else
                {
                    customPackageListView.Rebuild();
                }

                customPackageListView.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            customListViewHeaderContainer.Add(foldout);
            customListViewHeaderContainer.Add(customPackagesSearchPanel);

            rootVisualElement.Add(customListViewHeaderContainer);
            rootVisualElement.Add(customPackageListView);

            return rootVisualElement;
        }
        private static void RemoveItemsWhenZeroEnteredToCollectionSize()
        {
            if (customPackageListView.itemsSource.Count != 0) return;

            PackageInitializerSave.instance.Save();
            customPackageListView.Rebuild();

            EditorApplication.update -= RemoveItemsWhenZeroEnteredToCollectionSize;
        }
        private static VisualElement MakeItemCustomPackageListView()
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.paddingLeft = 10;
            row.style.paddingRight = 10;

            Toggle toggle = new Toggle();
            toggle.style.marginRight = 10;

            TextField textField = new TextField();
            textField.style.flexGrow = 1;
            textField.style.unityTextAlign = TextAnchor.MiddleLeft;

            row.Add(toggle);
            row.Add(textField);

            return row;
        }
        private static void BindCustomPackageListView(VisualElement element, int index)
        {
            Toggle toggle = element.Q<Toggle>();
            TextField textField = element.Q<TextField>();


            if (index < PackageInitializerSave.instance.customPackages.Count && PackageInitializerSave.instance.customPackages != null)
            {
                if (PackageInitializerSave.instance.customPackages[index] != null)
                {
                    toggle.value = PackageInitializerSave.instance.customPackages[index].shouldPackageInstalled;
                    textField.value = PackageInitializerSave.instance.customPackages[index].packageName;
                }
            }


            toggle.RegisterCallback<ClickEvent>((clickEvent) =>
            {
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.currentTarget == toggle)
                    {
                        Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Change Git Packages Toggle");

                        PackageInitializerSave.instance.customPackages[index].shouldPackageInstalled = evt.newValue;
                        PackageInitializerSave.instance.Save();
                    }
                });
            });

            textField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (index < PackageInitializerSave.instance.customPackages.Count)
                {
                    Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Change Git Packages Text");

                    PackageInitializerSave.instance.customPackages[index].packageName = evt.newValue;
                    PackageInitializerSave.instance.Save();
                }
            });

            textField.RegisterCallback<FocusOutEvent>(evt =>
            {
                if (!customPackagesSearchlist.Contains(textField.value))
                {
                    customPackagesSearchlist.Add(textField.value);
                }
                RemoveAndAddToCustomPackageSearchElement();
            });

            Undo.undoRedoPerformed += () =>
            {
                if (index < PackageInitializerSave.instance.customPackages.Count)
                {
                    Package package = PackageInitializerSave.instance.customPackages[index];

                    if (package != null)
                    {
                        toggle.SetValueWithoutNotify(package.shouldPackageInstalled);
                        textField.SetValueWithoutNotify(package.packageName);
                    }
                }
            };
        }
        private static void RemoveAndAddToCustomPackageSearchElement()
        {
            customPackagesSearchlist = new List<string>();
            customPackagesSearchResultList = new List<string>();

            for (int i = 0; i < PackageInitializerSave.instance.customPackages.Count; i++)
            {
                customPackagesSearchlist.Add(PackageInitializerSave.instance.customPackages[i].packageName);
            }
            temporaryCustomPackagesList = new List<Package>();

            customListViewHeaderContainer.RemoveAt(1);

            customPackagesSearchPanel = new ToolbarSearchPanel(customPackagesSearchlist, customPackagesSearchResultList, CustomPackageSearchIsEmpty, CustomPackageSearchIsFilled);
            customPackagesSearchPanel.style.alignSelf = Align.FlexEnd;

            customListViewHeaderContainer.Add(customPackagesSearchPanel);
        }
        // --------------------------------------------------------------------------------------
        
        // Asset Store Packages
        private static void AssetStorePackagesSearchPanel()
        {
            assetStorePackagesSearchlist = new List<string>();
            assetStorePackagesSearchResultList = new List<string>();

            for (int i = 0; i < PackageInitializerSave.instance.assetStorePackages.Count; i++)
            {
                assetStorePackagesSearchlist.Add(PackageInitializerSave.instance.assetStorePackages[i].packageName);
            }
            temporaryassetStorePackagesList = new List<Package>();

            assetStorePackagesSearchPanel = new ToolbarSearchPanel(assetStorePackagesSearchlist, assetStorePackagesSearchResultList, AssetStorePackageSearchIsEmpty, AssetStorePackageSearchIsFilled);
            assetStorePackagesSearchPanel.style.alignSelf = Align.FlexEnd;
        }
        private static void AssetStorePackageSearchIsFilled()
        {
            temporaryassetStorePackagesList.Clear();
            for (int i = 0; i < assetStorePackagesSearchResultList.Count; i++)
            {
                string resultPackageName = assetStorePackagesSearchResultList[i];

                Package package = PackageInitializerSave.instance.assetStorePackages.Find((_element) => _element.packageName == resultPackageName);

                if (package != null)
                {
                    temporaryassetStorePackagesList.Add(package);
                }
            }
            assetStorePackagesListView.itemsSource = temporaryassetStorePackagesList;
            assetStorePackagesListView.RefreshItems();
        }
        private static void AssetStorePackageSearchIsEmpty()
        {
            for (int i = 0; i < PackageInitializerSave.instance.assetStorePackages.Count; i++)
            {
                Package package = PackageInitializerSave.instance.assetStorePackages[i];
                Package tempPackage = temporaryassetStorePackagesList.Find((_element) => _element.packageName == package.packageName);
                if (tempPackage != null)
                {
                    package.shouldPackageInstalled = tempPackage.shouldPackageInstalled;
                }
            }
            PackageInitializerSave.instance.Save();


            temporaryassetStorePackagesList.Clear();
            assetStorePackagesListView.itemsSource = PackageInitializerSave.instance.assetStorePackages;
            assetStorePackagesListView.RefreshItems();
        }
        private static VisualElement AssetStorePackagesListView()
        {
            VisualElement rootVisualElement = new VisualElement();
            List<string> unityPackages = FindUnityPackages(GlobalVariables.CurrentAssetStorePath);

            if (PackageInitializerSave.instance.assetStorePackages != null)
            {
                for (int i = 0; i < unityPackages.Count; i++)
                {
                    string currentPackageName = Path.GetFileNameWithoutExtension(unityPackages[i]);

                    Package package = PackageInitializerSave.instance.assetStorePackages.Find((_package) => _package.packageName == currentPackageName);
                    if (package == null)
                    {
                        PackageInitializerSave.instance.assetStorePackages.Add(new Package(currentPackageName, false));
                    }
                }

                PackageInitializerSave.instance.assetStorePackages.Sort();
                PackageInitializerSave.instance.Save();
            }



            assetStorePackagesListView = new ListView(PackageInitializerSave.instance.assetStorePackages, 30, MakeItemAssetStorePackagesListView, BindAssetStorePackagesListView);

            assetStorePackagesListView.selectionType = SelectionType.Single;
            assetStorePackagesListView.style.flexGrow = 1;
            assetStorePackagesListView.showBorder = true;
            assetStorePackagesListView.viewDataKey = "AssetStorePackages";




            VisualElement assetStoreListViewHeaderContainer = new VisualElement();
            assetStoreListViewHeaderContainer.style.flexDirection = FlexDirection.Row;
            assetStoreListViewHeaderContainer.style.justifyContent = Justify.SpaceBetween;
            Foldout foldout = new Foldout();
            foldout.viewDataKey = "AssetStorePackagesFoldout";
            foldout.style.flexGrow = 1;
            foldout.text = AssetStoreListViewName;
            foldout.value = true;

            foldout.AddToClassList(GlobalVariables.ListViewFoldoutStyleName);
            foldout.RegisterValueChangedCallback(evt =>
            {
                assetStorePackagesListView.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            assetStoreListViewHeaderContainer.Add(foldout);
            assetStoreListViewHeaderContainer.Add(assetStorePackagesSearchPanel);

            rootVisualElement.Add(assetStoreListViewHeaderContainer);
            rootVisualElement.Add(assetStorePackagesListView);

            return rootVisualElement;
        }
        private static VisualElement MakeItemAssetStorePackagesListView()
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.paddingLeft = 10;
            row.style.paddingRight = 10;

            Toggle toggle = new Toggle();
            Image image = new Image();
            Label label = new Label();
            image.style.width = 20f;
            image.style.height = 20f;
            image.style.marginRight = 10f;
            image.style.marginLeft = 5f;
            image.style.marginTop = 4.5f;
            image.style.alignItems = Align.FlexEnd;
            image.style.justifyContent = Justify.FlexEnd;
            image.style.flexShrink = 0;
            image.image = EditorGUIUtility.IconContent(GlobalVariables.UnityLogoIconName).image as Texture2D;

            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;


            row.Add(toggle);
            row.Add(image);
            row.Add(label);

            return row;
        }
        private static void BindAssetStorePackagesListView(VisualElement element, int index)
        {
            Toggle toggle = element.Q<Toggle>();
            Image image = element.Q<Image>();
            Label label = element.Q<Label>();


            List<Package> packages = assetStorePackagesListView.itemsSource as List<Package>;
            toggle.value = packages[index].shouldPackageInstalled;
            label.text = packages[index].packageName;


            toggle.RegisterCallback<ClickEvent>((clickEvent) =>
            {
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.currentTarget == toggle)
                    {
                        Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Change Package Initializer Toggle");

                        toggle.value = evt.newValue;

                        PackageInitializerSave.instance.assetStorePackages[index].shouldPackageInstalled = toggle.value;
                        PackageInitializerSave.instance.Save();
                    }
                });
            });

            image.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.currentTarget == image)
                {
                    Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Change Package Initializer Toggle");

                    toggle.value = !toggle.value;
                    Package clickedPackage = assetStorePackagesListView.selectedItem as Package;
                    clickedPackage.shouldPackageInstalled = toggle.value;

                    PackageInitializerSave.instance.Save();
                    evt.StopImmediatePropagation();
                }
            });

            label.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.currentTarget == label)
                {
                    Undo.RegisterCompleteObjectUndo(PackageInitializerSave.instance, "Change Package Initializer Toggle");

                    toggle.value = !toggle.value;

                    Package clickedPackage = assetStorePackagesListView.selectedItem as Package;
                    clickedPackage.shouldPackageInstalled = toggle.value;

                    PackageInitializerSave.instance.Save();
                    evt.StopImmediatePropagation();
                }
            });
            Undo.undoRedoPerformed += () =>
            {
                if (index < packages.Count)
                {
                    Package package = packages[index];

                    toggle.SetValueWithoutNotify(package.shouldPackageInstalled);
                    label.text = package.packageName;
                }
            };
        }
        // --------------------------------------------------------------------------------------

        private static List<string> FindUnityPackages(string assetStorePath)
        {
            List<string> unityPackages = new List<string>();

            if (Directory.Exists(assetStorePath))
            {
                string[] unityPackageFiles = Directory.GetFiles(assetStorePath, "*.unitypackage", SearchOption.AllDirectories);

                if (unityPackageFiles.Length > 0)
                {
                    for (int i = 0; i < unityPackageFiles.Length; i++)
                    {
                        unityPackages.Add(unityPackageFiles[i]);
                    }
                }
            }

            if (unityPackages.Count > 0)
            {
                unityPackages.Distinct().ToList().Sort();
            }

            return unityPackages;
        }
        private static void ImportTrueAssetStorePackages()
        {
            // Install or remove asset store packages
            List<string> unityPackages = FindUnityPackages(GlobalVariables.CurrentAssetStorePath);
            if (unityPackages.Count > 0)
            {
                for (int i = 0; i < PackageInitializerSave.instance.assetStorePackages.Count; i++)
                {
                    Package currentAssetStorePackage = PackageInitializerSave.instance.assetStorePackages[i];

                    if (currentAssetStorePackage.shouldPackageInstalled)
                    {
                        string currentPackageInstallPath = unityPackages.Find((packageName) => Path.GetFileNameWithoutExtension(packageName) == currentAssetStorePackage.packageName);
                        AssetDatabase.ImportPackage(currentPackageInstallPath, false);
                    }
                }
            }
        }
        private static void ListInstalledPackagesProgress()
        {
            if (listInstalledPackages.IsCompleted)
            {
                if (listInstalledPackages.Status == StatusCode.Success)
                {
                    foreach (var package in listInstalledPackages.Result)
                    {
                        installedPackages.Add(package);
                    }

                }

                UpdateProjectPackagesAccordingToPackageInitializer();
                EditorApplication.update -= ListInstalledPackagesProgress;
            }
        }
        private static void ListInstalledPackages()
        {
            listInstalledPackages = Client.List(false, true);
            EditorApplication.update += ListInstalledPackagesProgress;
        }

        private static void ResetAddRemoveRequest()
        {
            if (resetAddRemoveRequest.IsCompleted)
            {

                Debug.Log(resetAddRemoveRequest.Error.message);

                EditorApplication.update -= ResetAddRemoveRequest;
            }
        }
        private static void AddOrRemoveProgress()
        {
            if (addRemoveOfPackageInitializer.IsCompleted)
            {
                EditorApplication.update -= AddOrRemoveProgress;
                EditorUtility.ClearProgressBar();


                if (addRemoveOfPackageInitializer.Result == null)
                {
                    if (addRemoveOfPackageInitializer.Error != null)
                    {
                        string cachedErrorMessage = addRemoveOfPackageInitializer.Error.message;
                        Debug.LogError(cachedErrorMessage + "\n Package initializer will stop working.");
                    }
                }


                ImportTrueAssetStorePackages();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Package Initializer", "Installing or removing packages...", 0.5f);
            }
            if (addRemoveOfPackageInitializer.Result == null)
            {
                EditorUtility.ClearProgressBar();
            }
            if (addRemoveOfPackageInitializer.Error != null)
            {
                if (addRemoveOfPackageInitializer.Error.message != null)
                {
                    EditorUtility.ClearProgressBar();
                }
            }
        }
        private static void ListBuiltInPackagesProgress()
        {
            if (listBuiltInPackages.IsCompleted)
            {
                if (listBuiltInPackages.Status == StatusCode.Success)
                {
                    foreach (var package in listBuiltInPackages.Result)
                    {
                        currentBuiltinPackageNames.Add(package.name);
                    }
                }
                EditorApplication.update -= ListBuiltInPackagesProgress;

                builtInPackageSearchRequest = Client.SearchAll();
                EditorApplication.update += SearchBuiltInPackagesProgress;
            }
        }
        private static void SearchBuiltInPackagesProgress()
        {
            if (builtInPackageSearchRequest.IsCompleted)
            {
                if (builtInPackageSearchRequest.Status == StatusCode.Success)
                {
                    List<string> packageNames = new List<string>();
                    foreach (PackageInfo package in builtInPackageSearchRequest.Result)
                    {
                        packageNames.Add(package.name);
                    }

                    packageNames.RemoveAll(item => removeCoreUnityPackages.Any(removeItem => removeItem == item));

                    packageNames.Distinct().ToList();
                    packageNames.Sort();


                    for (int i = 0; i < packageNames.Count; i++)
                    {
                        string packageName = packageNames[i];
                        if (PackageInitializerSave.instance.builtInPackages.Any((_packageName) => _packageName.packageName == packageName)) continue;

                        bool anyExist = currentBuiltinPackageNames.Any(item => item == packageName);

                        if (anyExist)
                        {
                            PackageInitializerSave.instance.builtInPackages.Add(new Package(packageName, true));
                        }
                        else
                        {
                            PackageInitializerSave.instance.builtInPackages.Add(new Package(packageName, false));
                        }
                    }


                    EditorApplication.update -= SearchBuiltInPackagesProgress;
                    PackageInitializerSave.instance.Save();
                    builtInPackagesListView.Rebuild();

                    BuiltInPackagesSearchPanel();

                    builtInListViewHeaderContainer.Add(builtInPackagesSearchPanel);
                }

                EditorUtility.ClearProgressBar();
            }
        }
    }
}