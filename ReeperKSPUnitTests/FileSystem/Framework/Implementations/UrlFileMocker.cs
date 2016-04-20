using System.IO;
using NSubstitute;
using ReeperKSP.FileSystem;

namespace ReeperKSPUnitTests.FileSystem.Framework.Implementations
{
    class UrlFileMocker : IUrlFileMocker
    {
        public IUrlFile Get(string filename)
        {
            var f = Substitute.For<IUrlFile>();

            filename = filename.Trim('/', '\\');

            f.FullPath.Returns("C:/" + filename);
            f.Name.Returns(Path.GetFileNameWithoutExtension(filename));
            f.Extension.Returns((Path.GetExtension(filename) ?? "").TrimStart('.'));
            f.Url.Returns("/" + Path.GetFileNameWithoutExtension(filename));

            return f;
        }
    }
}
