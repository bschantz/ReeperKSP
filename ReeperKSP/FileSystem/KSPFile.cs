using System;
using System.IO;
using ReeperCommon.Containers;
using ReeperCommon.Extensions;

namespace ReeperKSP.FileSystem
{
    public class KSPFile : IFile
    {
        private readonly IUrlFile _file;
        private FileInfo _info;         
        private readonly IDirectory _directory;


        public KSPFile(IDirectory directory, IUrlFile file)
        {
            if (directory.IsNull())
                throw new ArgumentNullException("directory");

            if (file.IsNull())
                throw new ArgumentNullException("file");

            _directory = directory;
            _file = file;
        }

        public IUrlFile UrlFile
        {
            get { return _file; }
        }

        public Maybe<FileInfo> Info
        {
            get
            {
                if (_info.IsNull())
                    _info = new FileInfo(FullPath);

                return _info.IsNull() ? Maybe<FileInfo>.None : Maybe<FileInfo>.With(_info);
            }
        }

        public IDirectory Directory
        {
            get { return _directory; }
        }


        public string Extension
        {
            get { return _file.Extension; }
        }

        public string FullPath
        {
            get { return _file.FullPath; }
        }

        public string Name
        {
            get
            {
                return _file.Name.Trim('/', '\\');
            }
        }

        public string FileName
        {
            get
            {
                return string.IsNullOrEmpty(Extension) ? Name : Name + "." + Extension;
            }
        }

        public string Url { get { return _file.Url; } }
    }
}
