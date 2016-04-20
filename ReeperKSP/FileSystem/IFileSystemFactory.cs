namespace ReeperKSP.FileSystem
{
    public interface IFileSystemFactory
    {
        IFile GetFile(IDirectory directory, IUrlFile file);
        IDirectory GetDirectory(IUrlDir dir);


        IDirectory GameData { get; }
    }
}
