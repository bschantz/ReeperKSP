using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ReeperCommon.Containers;
using ReeperKSP.FileSystem;

namespace ReeperKSP.Providers
{
    public class AssemblyLocationProvider : IAssemblyLocationProvider
    {
        private readonly IFileSystemFactory _fsFactory;

        public AssemblyLocationProvider(IFileSystemFactory fsFactory)
        {
            if (fsFactory == null) throw new ArgumentNullException("fsFactory");
            _fsFactory = fsFactory;
        }


        public Maybe<IDirectory> GetDirectory(Assembly target)
        {
            var laLocation = AssemblyLoader.loadedAssemblies.FirstOrDefault(la => ReferenceEquals(la.assembly, target));

            return laLocation == null ? Maybe<IDirectory>.None : _fsFactory.GameData.Directory(new KSPUrlIdentifier(laLocation.url));
        }

        public Maybe<IFile> Get(Assembly target)
        {
            var results = AssemblyLoader.loadedAssemblies.Where(la => ReferenceEquals(la.assembly, target)).ToList();

            if (results.Count > 1) throw new InvalidOperationException("Multiple targets found in assembly loader");
            if (!results.Any()) return Maybe<IFile>.None;

            // oddly, the urls in AssemblyLoader don't specify the filename, only the directory
            var url = new KSPUrlIdentifier(results.First().url + Path.DirectorySeparatorChar + results.First().dllName);

            return _fsFactory.GameData.File(url);
        }
    }
}
