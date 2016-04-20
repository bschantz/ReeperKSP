using System;
using System.Collections.Generic;
using System.Linq;

namespace ReeperKSP.FileSystem
{
// ReSharper disable once InconsistentNaming
    public class KSPUrlDir : IUrlDir
    {
        private readonly UrlDir _kspDir;


        public KSPUrlDir(
            UrlDir kspDir)
        {
            if (kspDir == null) throw new ArgumentNullException("kspDir");

            _kspDir = kspDir;
        }


        public string Name { get { return _kspDir.name; }}

        public string FullPath {
            get { return _kspDir.path; }
        }

        public string Url { get { return _kspDir.url; } }

        public IUrlDir Parent {
            get { return new KSPUrlDir(_kspDir.parent); }
        }

        public UrlDir KspDir
        {
            get { return _kspDir; }
        }

        public IEnumerable<IUrlDir> Children { get { return _kspDir.children.Select(child => new KSPUrlDir(child)).Cast<IUrlDir>().ToList(); } }


        public IEnumerable<IUrlFile> Files
        {
            get
            {
                return _kspDir.files.Select(f => new KSPUrlFile(f)).Cast<IUrlFile>().ToList();
            }
        }

        public IEnumerable<IUrlFile> AllFiles
        {
            get
            {
                return _kspDir.AllFiles.Select(f => new KSPUrlFile(f)).Cast<IUrlFile>().ToList();
            }
        }


        public void AddFile(IUrlFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            _kspDir.files.AddUnique(file.file);
        }
    }
}
