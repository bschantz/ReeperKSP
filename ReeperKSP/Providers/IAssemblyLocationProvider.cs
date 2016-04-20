using System.Reflection;
using ReeperCommon.Containers;
using ReeperKSP.FileSystem;

namespace ReeperKSP.Providers
{
    public interface IAssemblyLocationProvider
    {
        Maybe<IDirectory> GetDirectory(Assembly target);
        Maybe<IFile> Get(Assembly target);
    }
}
