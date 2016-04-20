namespace ReeperKSP.AssetBundleLoading
{
    public interface IAssetBundleAssetsInjectedCallbackReceiver
    {
        void BeforeAssetInjection();
        void AfterAssetInjection();
    }
}
