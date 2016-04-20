using System;
using NSubstitute;
using ReeperKSPUnitTests.FileSystem.Framework.Implementations;
using Xunit;

namespace ReeperKSPUnitTests.FileSystem.Framework.Tests
{
    public class FakeDirectory_Test
    {
        static class DirectoryFactory
        {
            public static IFakeDirectory Create(string name, IUrlFileMocker fmocker)
            {
                return new FakeDirectory(name, fmocker);
            }

            public static IFakeDirectory Create(string name)
            {
                return new FakeDirectory(name, new UrlFileMocker());
            }
        }



        [Fact]
        void Constructor_ThrowsExceptionOnNull_OrEmpty_OrBad_String()
        {
            Assert.Throws<ArgumentNullException>(() => DirectoryFactory.Create(null, Substitute.For<IUrlFileMocker>()));
            Assert.Throws<ArgumentNullException>(() => DirectoryFactory.Create("", Substitute.For<IUrlFileMocker>()));

            Assert.Throws<ArgumentNullException>(() => DirectoryFactory.Create("/", Substitute.For<IUrlFileMocker>()));
            Assert.Throws<ArgumentNullException>(() => DirectoryFactory.Create("\\", Substitute.For<IUrlFileMocker>()));

            Assert.Throws<ArgumentNullException>(() => DirectoryFactory.Create("valid", null));
            Assert.Throws<ArgumentNullException>(() => DirectoryFactory.Create("valid", null));
        }



        [Fact]
        void Construct_CallsConstructOnChildren_PassingCorrectParent()
        {
            // arrange
            var shouldReceiveConstruct = Substitute.For<IFakeDirectory>();
            shouldReceiveConstruct.Name.Returns("ConstructReceiver");


            var sut = DirectoryFactory.Create("valid", Substitute.For<IUrlFileMocker>());

            

            sut.Directories.Add(shouldReceiveConstruct);

            // act
            var result = sut.Construct(null);


            // assert
            shouldReceiveConstruct.Received(1).Construct(Arg.Is(result));
            Assert.NotEmpty(result.Children);
        }


    }
}
