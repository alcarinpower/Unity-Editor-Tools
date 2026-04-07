<!----------------------------------------------------Main Header Part------------------------------------------------------------------ -->
<h1 align="center">Unity Editor Tools</h1>

<p align="center"> Unity Editor Tools are a collection of tools, libraries and projects</p>
 <div align="center">
<img align= "center" src= https://github.com/user-attachments/assets/84d389a1-df42-46e8-889d-687fad040e25 width="600">
</div>

<br>

<!-- ----------------------------------------------------Table of Contents----------------------------------------------------- -->
<h2 align= "center">Table of Contents</h2>

<table align="center" border="1" cellpadding="10" cellspacing="0">
  <thead>
    <tr>
      <th>Section</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><a href="#Libraries">Libraries</a></td>
      <td>Libraries are a collection of classes and methods for Attributes and UI Elements</td>
    <tr>
      <td><a href="#Tools">Tools</a></td>
      <td>Tools speed up work in the Unity Editor by adding functionality and automating tasks</td>
    </tr>
       <tr>
      <td><a href="#Utilities">Utilities</a></td>
      <td>Utilities are a collection of short scripts and methods that provide functionality for the Unity Editor</td>
    </tr>
  </tbody>
</table>

<!-- -------------------------------------------------------------------------------------------------------------------------- -->


<br><br>

<!----------------------------------------------------Installation Part------------------------------------------------------------------ -->
<h2 align="left">Installation</h2>

<!--Local Installation Part-->
<p>Unity Editor Tools can be installed locally with unity package manager</p>

<!--Git Installation Part-->

<p>Or</p>

<p>Can be installed through git link with unity package manager:</p>

```
https://github.com/alcarinpower/Unity-Editor-Tools.git
```

<!-- ------------------------------------------------------------------------------------------------------------------------------- -->


<h2 align="center">Libraries</h2>
<h3>Attributes</h3>

${\color{grey}namespace: CompilerDestroyer.Editor.Attributes}$

<h5 align="left">    1- ReadonlyAttribute</h5>
<p>Allows you to make fields readonly.</p>

```csharp
[ReadOnly] public int health;
```
<br>

---

<br>

<h3>UI Elements</h3>

${\color{grey}namespace: CompilerDestroyer.Editor.UIElements}$

<h5 align="left">    1- Header</h5>
<p>Basic general label for headers. Default font size is 18.</p>

```csharp
Header header = new Header("Basic Header");
```

<h5 align="left">    2- InfoBox</h5>
<p>A custom box. InfoBoxIconType can be used to determine icon type.</p>

```cs
InfoBox infoBox = new InfoBox("An infobox can be used to give information", InfoBoxIconType.Info, 3f);
```

<h5 align="left">    3- Line</h5>
<p>A line that can be used to draw lines.</p>

```csharp
Line line = new Line(4f, false, Color.red);
```

<h5 align="left">    4- SettingsPanel</h5>
<p>You can use SettingsPanel to create Unity's project settings-like UIElements.
 In order to add items, you need to use TreeViewItemData<string> and in order to add functionality to it, you need to add a VisualElement to a dictionary with the same name as TreeViewItemData<string>.</p>

```cs
List<TreeViewItemData<string>> items = new List<TreeViewItemData<string>>();
TreeViewItemData<string> example1TreeViewItemData = new TreeViewItemData<string>(0, "Example 1");
TreeViewItemData<string> example2TreeViewItemData = new TreeViewItemData<string>(1, "Example 2");
items.Add(example1TreeViewItemData);
items.Add(example2TreeViewItemData);
Dictionary<string, VisualElement> itemsVisualElementsDict = new Dictionary<string, VisualElement>();
itemsVisualElementsDict.Add("Example 1", new Label("I am example 1"));
itemsVisualElementsDict.Add("Example 2", new Label("I am example 2"));

SettingsPanel panel = new SettingsPanel(ref items, ref itemsVisualElementsDict);
```

<h5 align="left">    5- ToolbarSearchPanel</h5>
<p>Same as ToolbarSearchField but search implemented with strings.</p>

```cs
List<string> toolbarSearchList = new List<string>() { "Level Editor", "Terrain Licker", "Inspector Destroyer", "Mesh Consumer" };
List<string> resultList = new List<string>();

VisualElement searchBarContainer = new VisualElement();


ListView listView = new ListView(toolbarSearchList, 15);
listView.makeItem = () => new Label();
listView.bindItem = (element, index) => (element as Label).text = listView.itemsSource[index] as string;

void OnEmpty()
{
    listView.itemsSource = toolbarSearchList;
    listView.Rebuild();
}
void OnFilled()
{
    listView.itemsSource = resultList;
    listView.Rebuild();
}
ToolbarSearchPanel toolbarSearchPanel = new ToolbarSearchPanel(toolbarSearchList, resultList, OnEmpty, OnFilled);
```

<h2 align="center">Tools</h2>
<h3 align="left">1- Package Initializer</h3>

![Package Initializer]<img width="706" height="527" alt="image" src="https://github.com/user-attachments/assets/cb20bc40-a015-4131-95e6-2176fd81b8d3" />


<p>Package Initializer can be used to install/remove built-in, git and asset store packages based on the toggles in the editor tool settings. For safety reasons, Asset Store packages will not be removed.<br>
After adjusting toggle of packages, settings will be saved to default preferences path. <br>Then apply button can pressed to apply changes.<br><br>
Package Initializer can be found in the "Tools > Compiler Destroyer > Editor Tools > Tools > Package Initializer"</p>

<br>

<h3 align="left">2- Roughness Converter</h3>

![Roughness Converter](https://github.com/user-attachments/assets/22f3d77b-a445-4f31-8e42-8b25aa5ae2ec)

<p>The Roughness Converter allows you to generate a Metallic Smoothness Map by combining a Metallic Map with a Roughness Map.<br>
 Alternatively, you can create a Smoothness Map directly from a Roughness Map.<br><br>Roughness Converter can be found in the "Tools > Compiler Destroyer > Editor Tools > Tools > Roughness Converter"</p>
 


<h2 align="left">Utilities</h2>
<h3 align="left">1- GitDependencyManager</h3>
<p>Checks gitDependencies=["https://github.com/ExamplePerson/ExampleRepo.git"] from package.json files automatically. If there are git repositories in there, this script ask user to install dependencies.  If it is null this does nothing. <br> In order to use this you should add this to <code>Events.registeredPackages += OnPackagesRegisteredCheckDependencies</code> You can copy this scripts into your packages or repositories to use it.</p>
 
<div align="left">

</div>
<br>

<!-- ------------------------------------------------------------------------------------------------------------------------------- -->

<!-- Support -->
<div align= "left">
<h2 align="left">Support</h2>
<p align="left">If you encounter any problems or bugs, create new issue in Issues page:
  <a href="https://github.com/compilerdestroyer/Unity-Editor-Tools/issues">Issues</a>
</p>

<h2 align="left">License</h2>
<p align="left">MIT LICENSE:
<a href="https://github.com/compilerdestroyer/Unity-Editor-Tools/blob/main/LICENSE">LICENSE</a>
 <p align="left">You can do whatever you want. Just don't try to re-upload and sell it on anywhere.</p>
</div>

