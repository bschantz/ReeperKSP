using ReeperKSP.FileSystem;

namespace ReeperKSPUnitTests.FileSystem.Framework
{
    public interface IUrlFileMocker
    {
        IUrlFile Get(string filename);
    }
}
