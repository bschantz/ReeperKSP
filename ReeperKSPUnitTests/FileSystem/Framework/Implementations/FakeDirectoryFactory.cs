using System;

namespace ReeperKSPUnitTests.FileSystem.Framework.Implementations
{
    class FakeDirectoryFactory : IFakeDirectoryFactory
    {
        private readonly IUrlFileMocker _fmocker;

        public FakeDirectoryFactory(IUrlFileMocker fmocker)
        {
            if (fmocker == null) throw new ArgumentNullException("fmocker");
            _fmocker = fmocker;
        }


        public IFakeDirectory Create(string name)
        {
            return new FakeDirectory(name, _fmocker);
        }
    }
}
