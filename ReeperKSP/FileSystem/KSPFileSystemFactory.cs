using System;

namespace ReeperKSP.FileSystem
{
// ReSharper disable once InconsistentNaming
    public class KSPFileSystemFactory : IFileSystemFactory
    {
        public KSPFileSystemFactory(IUrlDir gameData)
        {
            if (gameData == null) throw new ArgumentNullException("gameData");
            GameData = GetDirectory(gameData);
        }



        public IFile GetFile(IDirectory directory, IUrlFile file)
        {
            if (directory == null) throw new ArgumentNullException("directory");
            if (file == null) throw new ArgumentNullException("file");

            return new KSPFile(directory, file);
        }

        public IDirectory GetDirectory(IUrlDir dir)
        {
            return new KSPDirectory(this, dir);
        }

        public IDirectory GameData { get; private set; }

    }
}
