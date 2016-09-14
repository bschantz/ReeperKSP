using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReeperCommon.Containers;

namespace ReeperKSP.FileSystem
{
    public interface IDirectory
    {
        Maybe<IDirectory> Directory(IUrlIdentifier url);


        bool FileExists(IUrlIdentifier url);
        bool FileExists(string filename);

        bool DirectoryExists(IUrlIdentifier url);

        //void AddFileToHierarchy(IUrlFile file);


        Maybe<IFile> File(IUrlIdentifier url);
        Maybe<IFile> File(string filename);
        ReadOnlyCollection<IFile> Files();
        ReadOnlyCollection<IFile> Files(string extension);
        ReadOnlyCollection<IFile> RecursiveFiles();
        ReadOnlyCollection<IFile> RecursiveFiles(string extension);

        ReadOnlyCollection<IDirectory> Directories();
        ReadOnlyCollection<IDirectory> RecursiveDirectories();

        Maybe<IDirectory> Parent { get; }
        string FullPath { get; }
        string Url { get; }
        IUrlDir UrlDir { get; }
        string Name { get; }
    }
}
