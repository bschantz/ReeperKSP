using System;

namespace ReeperKSP.AssetBundleLoading
{
    [AttributeUsage(AttributeTargets.Field)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class AssetBundleAssetAttribute : Attribute
    {
        public string Name { get; private set; }
        public string AssetBundleRelativeUrl { get; private set; }
        public AssetCreationStyle CreationStyle { get; private set; }

        public enum AssetCreationStyle
        {
            Prefab,  // just return exactly what the AssetBundle gives us
            Instance // actually create an instance, a convenience that would allow the consumer to begin using the asset immediately
        }

        public AssetBundleAssetAttribute(string name, string assetBundleRelativeUrl, AssetCreationStyle creationStyle = AssetCreationStyle.Prefab)
        {
            Name = name;
            AssetBundleRelativeUrl = assetBundleRelativeUrl;
            CreationStyle = creationStyle;
        }


        public override string ToString()
        {
            return string.Format("AssetBundleAsset: Name: {0}, Bundle: {1}, creationStyle: {2}", Name, AssetBundleRelativeUrl, CreationStyle);
        }
    }
}
