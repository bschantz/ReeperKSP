using System.Collections.Generic;
using ReeperKSP.FileSystem;

namespace ReeperKSPUnitTests.FileSystem.Framework
{
    public interface IFakeDirectoryBuilder
    {
        IDirectory Build();
        IDirectory BuildIgnoreParents();

        IFakeDirectoryBuilder WithDirectory(string name);
        IFakeDirectoryBuilder WithFile(string filename);
        IFakeDirectoryBuilder MakeDirectory(string name);
        IFakeDirectoryBuilder Parent();

        IEnumerable<IFakeDirectory> Directories { get; }
    }
}
