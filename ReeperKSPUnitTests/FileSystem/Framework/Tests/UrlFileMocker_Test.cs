using System.IO;
using System.Linq;
using ReeperKSPUnitTests.FileSystem.Framework.Implementations;
using Xunit;
using Xunit.Extensions;

namespace ReeperKSPUnitTests.FileSystem.Framework.Tests
{
    public class UrlFileMocker_Test
    {
        static class UrlFileMockerFactory
        {
            public static IUrlFileMocker Create()
            {
                return new UrlFileMocker();
            }
        }



        [Theory]
        [InlineData("testFile.txt")]
        [InlineData("testFile")]
        [InlineData("/testFile.txt")]
        [InlineData("/testFile")]
        [InlineData("\\testFile.txt")]
        [InlineData("\\testFile")]
        void Get_Returns(string filename)
        {
            var sut = UrlFileMockerFactory.Create().Get(filename);

            Assert.True(sut.FullPath.Any());
            Assert.True(sut.FullPath.Any());
            Assert.True(sut.Name.Any());
            Assert.True(!Path.HasExtension(filename) || sut.Extension.Any());
            Assert.True(sut.Url.Any());
        }
    }
}
