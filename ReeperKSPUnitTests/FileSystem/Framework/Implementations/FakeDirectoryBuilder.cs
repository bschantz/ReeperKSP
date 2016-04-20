using System;
using System.Collections.Generic;
using System.Linq;
using ReeperCommon.Extensions;
using ReeperKSP.FileSystem;

namespace ReeperKSPUnitTests.FileSystem.Framework.Implementations
{
    public class FakeDirectoryBuilder : IFakeDirectoryBuilder
    {
        private readonly IFakeDirectory _root;
        private readonly IFakeDirectoryFactory _dirFactory;
        private readonly IFakeDirectoryBuilder _parent;
        private readonly List<Action<KSPDirectory>> _actions = new List<Action<KSPDirectory>>();




        public FakeDirectoryBuilder(IFakeDirectory root, IFakeDirectoryFactory dirFactory)
        {
            if (root == null) throw new ArgumentNullException("root");
            if (dirFactory == null) throw new ArgumentNullException("dirFactory");
            _root = root;
            _dirFactory = dirFactory;
        }


        private FakeDirectoryBuilder(
            IFakeDirectory root,
            IFakeDirectoryFactory dirFactory, 
            IFakeDirectoryBuilder parent):this(root, dirFactory)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            _parent = parent;

        }



        public IDirectory Build()
        {
            if (!_parent.IsNull()) return _parent.Build();

            return BuildIgnoreParents();
        }


        public IDirectory BuildIgnoreParents()
        {
            var root = _root.Construct(null);

            var fss = new KSPFileSystemFactory(root);
            var dir = new KSPDirectory(fss, root);

            _actions.ForEach(action => action(dir));

            return dir;
        }


        public IFakeDirectoryBuilder WithDirectory(string name)
        {
            if (_root.Directories.Any(d => d.Name == name))
                throw new InvalidOperationException(_root.Name + " already contains directory called " + name);

            _root.Directories.Add(_dirFactory.Create(name));
            return this;
        }



        public IFakeDirectoryBuilder WithFile(string filename)
        {
            if (_root.Files.Any(f => f == filename))
                throw new InvalidOperationException(_root.Name + " already contains file " + filename);

            _root.Files.Add(filename);

            return this;
        }



        public IFakeDirectoryBuilder MakeDirectory(string name)
        {
            if (_root.Directories.Any(d => d.Name == name))
                throw new InvalidOperationException(_root.Name + " already contains directory called " + name);

            var newDir = _dirFactory.Create(name);

            var newBuilder = new FakeDirectoryBuilder(newDir, _dirFactory, this);

            _root.Directories.Add(newDir);

            return newBuilder;
        }



        public IFakeDirectoryBuilder Parent()
        {
            if (_parent.IsNull())
                throw new InvalidOperationException(_root.Name + " does not have a parent directory");

            return _parent;
        }



        public IEnumerable<IFakeDirectory> Directories
        {
            get { return _root.Directories; }
        }



        public IFakeDirectory FakeDirectory
        {
            get { return _root; }
        }
    }
}
