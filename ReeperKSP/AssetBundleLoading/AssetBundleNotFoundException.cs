using System;

namespace ReeperKSP.AssetBundleLoading
{
    public class AssetBundleNotFoundException : Exception
    {
        public AssetBundleNotFoundException(string fullPath, string relativePath)
            : base(string.Format("AssetBundle at '{0}' [relative path '{1}'] not found", fullPath, relativePath))
        {
        }

        public AssetBundleNotFoundException(AssetBundleHandle handle)
            : base("No AssetBundle that matches " + handle + " found")
        {
            
        }

        public AssetBundleNotFoundException(string message) : base(message)
        {
            
        }
    }
}