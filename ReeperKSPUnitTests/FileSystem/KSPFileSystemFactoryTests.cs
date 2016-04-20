using System;
using NSubstitute;
using ReeperKSP.FileSystem;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

namespace ReeperKSPUnitTests.FileSystem
{
// ReSharper disable once InconsistentNaming
    public class KSPFileSystemFactoryTests
    {
        [Fact()]
        public void KSPFileSystemFactory_WithNullParameter_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new KSPFileSystemFactory(null));
        }


        [Theory, AutoDomainData]
        public void GetFile_WithUrlDirAndUrlFile_ReturnsWrappedUrlDirAndUrlFile(KSPFileSystemFactory sut)
        {
            var expectedFile = Substitute.For<IUrlFile>();
            var expectedDir = sut.GetDirectory(Substitute.For<IUrlDir>());

            var actual = sut.GetFile(expectedDir, expectedFile);

            Assert.Same(expectedDir, actual.Directory);
            Assert.Same(expectedFile, actual.UrlFile);
        }


        [Theory, AutoDomainData]
        public void GetDirectory_WithUrlDir_ReturnsWrappedUrlDir(KSPFileSystemFactory sut)
        {
            var expected = Substitute.For<IUrlDir>();

            var actual = sut.GetDirectory(expected);

            Assert.Same(expected, actual.UrlDir);
            Assert.True(actual is KSPDirectory);
        }

        [Fact()]
        public void GameData_Is_SameAsPassedIn()
        {
            var expected = Substitute.For<IUrlDir>();

            var sut = new KSPFileSystemFactory(expected);

            Assert.Same(expected, sut.GameData.UrlDir);
        }
    }
}
