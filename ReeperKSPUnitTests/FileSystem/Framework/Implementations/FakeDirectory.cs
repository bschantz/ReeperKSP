using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using ReeperKSP.FileSystem;

namespace ReeperKSPUnitTests.FileSystem.Framework.Implementations
{
    class FakeDirectory : IFakeDirectory
    {
        private readonly List<string> _files = new List<string>();

        private readonly string _name;
        private readonly IUrlFileMocker _fmocker;


        public FakeDirectory(string name, IUrlFileMocker fmocker)
        {
            if (fmocker == null) throw new ArgumentNullException("fmocker");
            if (name == null || string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            name = name.Trim('/', '\\');

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _name = name;
            _fmocker = fmocker;
            Directories = new List<IFakeDirectory>();
        }



        public string Name
        {
            get { return _name; }
        }



        public IEnumerable<string> AllFiles
        {
            get
            {
                return Files.Union(
                                    Directories.SelectMany(dir => dir.AllFiles.Select(f => dir.Name + "/" + f))
                                  );
            }
        }



        public List<IFakeDirectory> Directories { get; private set; }

        public IUrlDir Construct(IUrlDir parent)
        {
            var root = Substitute.For<IUrlDir>();

            root.Name.Returns(Name);
            root.FullPath.Returns("C:/" + Name + "/");
            root.Parent.Returns(parent);
            root.Url.Returns(Name); // stock KSP behaviour is to return dir name only, no slashes

            // has to be done outside .Returns()
            var children = Directories.Select(dir => dir.Construct(root)).ToList();
            root.Children.Returns(children);

            var mockedFiles = Files.Select(filename => _fmocker.Get(filename)).ToList(); // uses NSubstitute, so don't put inside Returns()
            root.Files.Returns(mockedFiles); // files in same dir

            var allMockedFiles = AllFiles.Select(f => _fmocker.Get(f)).ToList();
            root.AllFiles.Returns(allMockedFiles);



            return root;  
        }

 


        public List<string> Files
        {
            get
            {
                return _files;
            }
        }
    }
}
