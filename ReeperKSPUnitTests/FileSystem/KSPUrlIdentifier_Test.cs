using System;
using ReeperKSP.FileSystem;
using Xunit;
using Xunit.Extensions;

namespace ReeperKSPUnitTests.FileSystem
{
    public class KSPUrlIdentifier_Test
    {
        [Theory]
        [InlineData("file.txt")]
        [InlineData("file")]
        [InlineData("/file.txt")]
        [InlineData("/file")]
        [InlineData("\\file.txt")]
        [InlineData("\\file")]
        [InlineData("subdir/file.txt")]
        [InlineData("subdir/file")]
        [InlineData("subdir\\file.txt")]
        [InlineData("subdir\\file")]
        [InlineData("\\subdir\\file.txt")]
        [InlineData("\\subdir\\file")]
        [InlineData("/subdir/file.txt")]
        [InlineData("/subdir/file")]
        private void Constructor_WithValidUrl(string url)
        {
            Assert.DoesNotThrow(() => new KSPUrlIdentifier(url));
        }



        [Fact]
        private void Constructor_WithNullArg_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new KSPUrlIdentifier(null));
        }



        [Theory]
        [InlineData("")]
        [InlineData(".txt")]
        [InlineData(".")]
        [InlineData(".txt")]
        private void Constructor_InvalidUrl_Throws(string url)
        {
            Assert.Throws<ArgumentException>(() => new KSPUrlIdentifier(url));
        }
    }
}
