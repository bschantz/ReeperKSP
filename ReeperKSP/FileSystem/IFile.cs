using ReeperCommon.Containers;

namespace ReeperKSP.FileSystem
{
    public interface IFile
    {
        IUrlFile UrlFile { get; }
        Maybe<System.IO.FileInfo> Info { get; }
        IDirectory Directory { get; }
        string Extension { get; }
        string FullPath { get; }
        string Name { get; }
        string FileName { get; } // includes extension (if any)
        string Url { get; }
    }
}
