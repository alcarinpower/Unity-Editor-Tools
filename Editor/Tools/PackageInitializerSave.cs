using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;


namespace CodeDestroyer.Editor.EditorTools
{

    [FilePath(GlobalVariables.PackageName + "/packageinitializersave.binary", FilePathAttribute.Location.PreferencesFolder)]
    internal sealed class PackageInitializerSave : ScriptableSingleton<PackageInitializerSave>
    {
        [SerializeField] internal bool isPackageInitializerEnabled = true;
        [SerializeField] internal List<Package> builtInPackages = new List<Package>();
        [SerializeField] internal List<Package> customPackages = new List<Package>();
        [SerializeField] internal List<Package> assetStorePackages = new List<Package>();

        internal void Save()
        {
            EditorUtility.SetDirty(this);
            Save(true);
        }
        internal string GetSavePath() => GetFilePath();
    }


    [Serializable]
    internal class Package : IComparable<Package>
    {
        [SerializeField] public string packageName;
        [SerializeField] internal bool shouldPackageInstalled;


        internal Package()
        {

        }
        internal Package(string name, bool shouldPackageInstalled)
        {
            packageName = name;
            this.shouldPackageInstalled = shouldPackageInstalled;
        }

        public override bool Equals(object obj)
        {
            if (obj is Package other)
                return this.packageName == other.packageName;
            return false;
        }

        public override int GetHashCode()
        {
            return packageName.GetHashCode();
        }

        public int CompareTo(Package other)
        {
            if (other == null) return 1;

            return string.Compare(this.packageName, other.packageName, StringComparison.Ordinal);
        }
    }
}