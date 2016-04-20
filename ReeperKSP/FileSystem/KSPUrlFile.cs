using System;

namespace ReeperKSP.FileSystem
{
    public class KSPUrlFile : IUrlFile
    {
        private readonly UrlDir.UrlFile _file;

        public KSPUrlFile(UrlDir.UrlFile file)
        {
            if (file == null) throw new ArgumentNullException("file");
            _file = file;
        }

        public string FullPath
        {
            get { return _file.fullPath; }
        }

        public string Extension
        {
            get { return _file.fileExtension; }
        }

        public string Name { get { return _file.name; } }
        public string Url { get { return _file.url; }}

        public IUrlDir Directory
        {
            get { return new KSPUrlDir(_file.parent); }
        }

        public UrlDir.UrlFile file
        {
            get { return _file; }
        }
    }
}
