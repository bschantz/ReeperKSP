using System.Collections.Generic;

namespace ReeperKSP.FileSystem
{
    public interface IUrlDir
    {
        string Name { get; }
        string FullPath { get; }
        string Url { get; }
        IUrlDir Parent { get; }
        UrlDir KspDir { get; }

        IEnumerable<IUrlDir> Children { get; }
        IEnumerable<IUrlFile> Files { get; }
        IEnumerable<IUrlFile> AllFiles { get; }

        void AddFile(IUrlFile file);
    }
}
