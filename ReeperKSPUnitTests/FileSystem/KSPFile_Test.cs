using System;
using NSubstitute;
using ReeperKSP.FileSystem;
using Xunit;

namespace ReeperKSPUnitTests.FileSystem
{
    public class KSPFile_Test
    {
        [Fact]
        void Constructor_NullArgument_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new KSPFile(null, Substitute.For<IUrlFile>()));
            Assert.Throws<ArgumentNullException>(() => new KSPFile(Substitute.For<IDirectory>(), null));
        }



        [Fact]
        void Properties_ReturnCorrect()
        {
            var dir = Substitute.For<IDirectory>();
            var file = Substitute.For<IUrlFile>();
           
            var sut = new KSPFile(dir, file);



            var ext = sut.Extension;
            var fullpath = sut.FullPath;
            var name = sut.Name;
            var filename = sut.FileName;
            var url = sut.Url;



            Assert.Equal(file, sut.UrlFile);
            Assert.Equal(dir, sut.Directory);

            // must assign result to variable to keep compiler happy
            var extgetter = file.Received().Extension;
            var fpgetter = file.Received().FullPath;
            var namegetter = file.Received().Name;
            var urlgetter = file.Received().Url;
        }
    }
}
