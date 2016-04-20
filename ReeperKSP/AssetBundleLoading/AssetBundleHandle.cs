using System;
using JetBrains.Annotations;
using ReeperKSP.AssetBundleLoading;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace ReeperCommon.AssetBundleLoading
{
    /// <summary>
    /// If we just return the raw AssetBundle, the user might do something stupid with it (like Unload it) without us knowing
    /// </summary>
    public class AssetBundleHandle : IEquatable<AssetBundle>
    {
        private readonly AssetBundle _bundle;

        public AssetBundleHandle([NotNull] AssetBundle bundle)
        {
            if (bundle == null) throw new ArgumentNullException("bundle");
            _bundle = bundle;
        }


        public void Unload(bool unloadedAllLoadedObjects = false)
        {
            AssetBundleAssetLoader.UnloadBundle(this, unloadedAllLoadedObjects);
        }


        public Object mainAsset
        {
            get { return _bundle.mainAsset; }
        }


        public bool Contains(string name)
        {
            return _bundle.Contains(name);
        }


        public string[] GetAllAssetNames()
        {
            return _bundle.GetAllAssetNames();
        }


        public string[] GetAllScenePaths()
        {
            return _bundle.GetAllScenePaths();
        }


        public Object[] LoadAllAssets()
        {
            return _bundle.LoadAllAssets();
        }


        public T[] LoadAllAssets<T>() where T : Object
        {
            return _bundle.LoadAllAssets<T>();
        }


        public Object[] LoadAllAssets(Type type)
        {
            return _bundle.LoadAllAssets(type);
        }


        public AssetBundleRequest LoadAllAssetsAsync<T>()
        {
            return _bundle.LoadAllAssetsAsync<T>();
        }


        public AssetBundleRequest LoadAllAssetsAsync()
        {
            return _bundle.LoadAllAssetsAsync();
        }


        public AssetBundleRequest LoadAllAssetsAsync(Type type)
        {
            return _bundle.LoadAllAssetsAsync(type);
        }


        public Object LoadAsset(string name)
        {
            return _bundle.LoadAsset(name);
        }


        public T LoadAsset<T>(string name) where T : Object
        {
            return _bundle.LoadAsset<T>(name);
        }


        public Object LoadAsset(string name, Type type)
        {
            return _bundle.LoadAsset(name, type);
        }


        public AssetBundleRequest LoadAssetAsync(string name)
        {
            return _bundle.LoadAssetAsync(name);
        }


        public AssetBundleRequest LoadAssetAsync<T>(string name)
        {
            return _bundle.LoadAssetAsync<T>(name);
        }


        public AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            return _bundle.LoadAssetAsync(name, type);
        }


        public T[] LoadAssetWithSubAssets<T>(string name) where T : Object
        {
            return _bundle.LoadAssetWithSubAssets<T>(name);
        }


        public Object[] LoadAssetWithSubAssets(string name)
        {
            return _bundle.LoadAssetWithSubAssets(name);
        }


        public Object[] LoadAssetWithSubAssets(string name, Type type)
        {
            return _bundle.LoadAssetWithSubAssets(name, type);
        }


        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
        {
            return _bundle.LoadAssetWithSubAssetsAsync(name);
        }


        public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
        {
            return _bundle.LoadAssetWithSubAssetsAsync<T>(name);
        }


        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
        {
            return _bundle.LoadAssetWithSubAssetsAsync(name, type);
        }


        public bool Equals(AssetBundle other)
        {
            return ReferenceEquals(_bundle, other);
        }


        public override bool Equals(object obj)
        {
            var other = obj as AssetBundleHandle;

            if (other != null) return ReferenceEquals(this, other) || ReferenceEquals(_bundle, other._bundle);

            var bundle = obj as AssetBundle;
            return bundle != null && ReferenceEquals(_bundle, bundle);
        }

        protected bool Equals(AssetBundleHandle other)
        {
            return Equals(_bundle, other._bundle);
        }

        public override int GetHashCode()
        {
            return (_bundle != null ? _bundle.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return GetType().Name + ": " + _bundle.name;
        }
    }
}
