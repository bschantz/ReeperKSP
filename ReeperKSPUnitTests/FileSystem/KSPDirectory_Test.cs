using System;
using System.Linq;
using NSubstitute;
using ReeperKSP.FileSystem;
using ReeperKSPUnitTests.FileSystem.Framework;
using ReeperKSPUnitTests.FileSystem.Framework.Implementations;
using Xunit;

namespace ReeperKSPUnitTests.FileSystem
{
    // ReSharper disable once InconsistentNaming
    public class KSPDirectory_Test
    {
        public static class Factory
        {
            public static IDirectory Create(string name, IUrlDir root)
            {
                return new KSPDirectory(CreateFileSystemFactory(), root);
            }

       
            public static IFileSystemFactory CreateFileSystemFactory()
            {
                var fsf = Substitute.For<IFileSystemFactory>();

                fsf.GetDirectory(Arg.Any<IUrlDir>()).ReturnsForAnyArgs(d => new KSPDirectory(fsf, d.Arg<IUrlDir>()));
                fsf.GetFile(Arg.Any<IDirectory>(), Arg.Any<IUrlFile>()).Returns(d => new KSPFile(d.Arg<IDirectory>(), d.Arg<IUrlFile>()));

                return fsf;
            }

            public static IFakeDirectoryBuilder CreateBuilder(string name = "GameData")
            {
                return new FakeDirectoryBuilder(new FakeDirectory(name, new UrlFileMocker()), new FakeDirectoryFactory(new UrlFileMocker()));
            }
        }



        [Fact]
        void Constructor_NullArgument_Check()
        {
            Assert.Throws<ArgumentNullException>(() => new KSPDirectory(null, Substitute.For<IUrlDir>()));
            Assert.Throws<ArgumentNullException>(() => new KSPDirectory(Substitute.For<IFileSystemFactory>(), null));
        }



        [Fact]
        void Directories_ReturnsDirectSubdirectoriesOnly()
        {
            var builder = Factory.CreateBuilder();

            builder
                .WithFile("file.txt")
                .WithDirectory("TestDirectory")
                .WithDirectory("AnotherTest")
                .MakeDirectory("Subdir")
                    .WithDirectory("DirInSubdir")
                    .WithFile("subfile.txt");

            var sut = builder.Build();

            var directChildDirectories = sut.Directories();

            Assert.Equal(new[] { "TestDirectory", "AnotherTest", "Subdir" },
                directChildDirectories.Select(dir => dir.Name));

        }


        [Fact]
        void Directory_WithNestedName()
        {
            var builder = Factory.CreateBuilder();

            builder.WithFile("toplevel")
                .MakeDirectory("nested")
                .WithDirectory("nested");

            var sut = builder.Build();

            Assert.NotEmpty(sut.Directory(new KSPUrlIdentifier("nested")));
            Assert.NotEmpty(sut.Directory(new KSPUrlIdentifier("nested/nested")));
            Assert.Empty(sut.Directory(new KSPUrlIdentifier("toplevel")));
            Assert.Empty(sut.Directory(new KSPUrlIdentifier("nested/toplevel")));
        }


        [Fact]
        void RecursiveDirectories_ReturnsAllDirectoriesUnder()
        {
            var builder = Factory.CreateBuilder("GameData");

            var sut = builder
                        .WithFile("file.txt")
                        .MakeDirectory("first")
                            .MakeDirectory("second")
                                .MakeDirectory("third")
                                .WithFile("first_second_third.txt")
                        .Build();
            
            var allDirs = sut.RecursiveDirectories().ToList();

            Assert.NotEmpty(allDirs);
            Assert.Equal(new[] {"first", "second", "third"}, allDirs.Select(d => d.Name));
        }



        [Fact]
        void Directory_BugHunting()
        {
            var sut = Factory.CreateBuilder().MakeDirectory("first").WithDirectory("second").Build();

            var test = Factory.CreateBuilder().MakeDirectory("subdir");

            int dirs = test.Directories.Count();

            test.WithDirectory("another");

            int dirs2 = test.Directories.Count();
            
            Assert.True(sut.Directory(new KSPUrlIdentifier("first")).Any());
            Assert.True(sut.Directory(new KSPUrlIdentifier("first/second")).Any());
        }
        


        [Fact]
        void Directory_Returns_ImmediateDir_AndSubDirFromUrl()
        {
            var sut = Factory.CreateBuilder()
                        .WithDirectory("first")
                        .WithDirectory("second")
                        .MakeDirectory("third")
                            .WithDirectory("fourth")
                            .Build();

            Assert.True(sut.Directory(new KSPUrlIdentifier("first")).Any());
            Assert.True(sut.Directory(new KSPUrlIdentifier("third/fourth")).Any());
            Assert.False(sut.Directory(new KSPUrlIdentifier("nonexistent")).Any());
            Assert.False(sut.Directory(new KSPUrlIdentifier("fake/fourth")).Any());
        }



        [Fact]
        void DirectoryExists()
        {
            var sut = Factory.CreateBuilder()
                .MakeDirectory("test")
                .WithDirectory("subdir").Build();

            Assert.True(sut.DirectoryExists(new KSPUrlIdentifier("test")));
            Assert.True(sut.DirectoryExists(new KSPUrlIdentifier("test/subdir")));
            Assert.False(sut.DirectoryExists(new KSPUrlIdentifier("fake")));
            Assert.False(sut.DirectoryExists(new KSPUrlIdentifier("fake/subdir")));
        }



        [Fact]
        void File()
        {
            var sut = Factory.CreateBuilder()
                                .WithFile("test.txt")
                                .WithFile("another")
                                .MakeDirectory("assemblyreloader")
                                    .WithFile("subfile.txt")
                                    .WithFile("assemblyreloader")
                                    .Build();

            Assert.True(sut.File(new KSPUrlIdentifier("test.txt")).Any());
            Assert.True(sut.File(new KSPUrlIdentifier("test")).Any()); // remember -- we accept extensionless also
            Assert.True(sut.File(new KSPUrlIdentifier("another")).Any());
            Assert.True(sut.File(new KSPUrlIdentifier("assemblyreloader/subfile.txt")).Any());
            Assert.True(sut.File(new KSPUrlIdentifier("assemblyreloader/subfile")).Any());
            Assert.True(sut.File(new KSPUrlIdentifier("assemblyreloader/assemblyreloader")).Any());

            Assert.False(sut.File(new KSPUrlIdentifier("nonexistent")).Any());
            Assert.False(sut.File(new KSPUrlIdentifier("assemblyreloader/nonexistent")).Any());
            Assert.False(sut.File(new KSPUrlIdentifier("nonexistent/subfile.txt")).Any());
        }




        [Fact]
        void FileExists_UsingUrlIdentifier()
        {
            var sut = Factory.CreateBuilder()
                                .WithFile("test.txt")
                                .WithFile("test")
                                .MakeDirectory("subdir")
                                    .WithFile("subfile.txt")
                                    .Build();

            Assert.True(sut.FileExists(new KSPUrlIdentifier("test.txt")));
            Assert.True(sut.FileExists(new KSPUrlIdentifier("test")));
            Assert.True(sut.FileExists(new KSPUrlIdentifier("subdir/subfile.txt")));
            Assert.True(sut.FileExists(new KSPUrlIdentifier("subdir/subfile")));

            Assert.False(sut.FileExists(new KSPUrlIdentifier("nonexistent")));
            Assert.False(sut.FileExists(new KSPUrlIdentifier("subdir/nonexistent")));
            Assert.False(sut.FileExists(new KSPUrlIdentifier("nonexistent/subfile.txt")));
        }

        [Fact]
        void FileExists_UsingFilename()
        {
            var sut = Factory.CreateBuilder()
                                .WithFile("test.txt")
                                .WithFile("test")
                                .MakeDirectory("subdir")
                                    .WithFile("subfile.txt")
                                    .Build();

            Assert.True(sut.FileExists("test.txt"));
            Assert.True(sut.FileExists("test"));
            Assert.True(sut.FileExists("subdir/subfile.txt"));
            Assert.True(sut.FileExists("subdir/subfile"));

            Assert.False(sut.FileExists("nonexistent"));
            Assert.False(sut.FileExists("nonexistent.txt"));
            Assert.False(sut.FileExists("subdir/nonexistent"));
            Assert.False(sut.FileExists("subdir/nonexistent.txt"));
            Assert.False(sut.FileExists("nonexistent/subfile"));
            Assert.False(sut.FileExists("nonexistent/subfile.txt"));
        }



        [Fact]
        void Files()
        {
            var sut =
                Factory.CreateBuilder()
                    .WithFile("firstfile.txt")
                    .WithFile("secondfile.bmp")
                    .MakeDirectory("directory")
                        .MakeDirectory("subdir")
                        .WithFile("subfile.txt")
                        .Build();

            Assert.Equal(new[] {"firstfile.txt", "secondfile.bmp"}, sut.Files().Select(f => f.FileName));

            Assert.NotEmpty(sut.Files("bmp"));
            Assert.NotEmpty(sut.Files(".bmp"));
            Assert.NotEmpty(sut.Files("*.bmp"));
            Assert.NotEmpty(sut.Files("*.*"));
            Assert.NotEmpty(sut.Files(".*"));
            Assert.NotEmpty(sut.Files("*"));

            Assert.Equal(new[] {"secondfile.bmp"}, sut.Files("bmp").Select(f => f.FileName));
            Assert.Equal(new[] { "secondfile.bmp" }, sut.Files(".bmp").Select(f => f.FileName));
            Assert.Equal(new[] { "secondfile.bmp" }, sut.Files("*.bmp").Select(f => f.FileName));

            Assert.Equal(new[] {"firstfile.txt", "secondfile.bmp"}, sut.Files("*.*").Select(f => f.FileName));
            Assert.Equal(new[] {"firstfile.txt", "secondfile.bmp"}, sut.Files(".*").Select(f => f.FileName));
            Assert.Equal(new[] {"firstfile.txt", "secondfile.bmp"}, sut.Files("*").Select(f => f.FileName));


            Assert.Empty(sut.Files("directory/*.bmp"));
            Assert.Empty(sut.Files("directory/*.txt"));
            Assert.Empty(sut.Files("directory/*.*"));

            Assert.NotEmpty(sut.Files("directory/subdir/*.*").ToList());
            Assert.NotEmpty(sut.Files("directory/subdir/.*").ToList());
            Assert.NotEmpty(sut.Files("directory/subdir/*").ToList());
            Assert.NotEmpty(sut.Files("directory\\subdir\\*.*").ToList());
            Assert.NotEmpty(sut.Files("directory\\subdir\\.*").ToList());
            Assert.NotEmpty(sut.Files("directory\\subdir\\*").ToList());
            Assert.NotEmpty(sut.Files("directory/subdir\\*.*").ToList());
            Assert.NotEmpty(sut.Files("directory\\subdir/*.*").ToList());

            Assert.Empty(sut.Files("directory/subdir"));
            Assert.NotEmpty(sut.Files("directory/subdir/*.txt"));
            Assert.Equal(new[] { "subfile.txt" }, sut.Files("directory/subdir/*.txt").Select(f => f.FileName));
            Assert.NotEmpty(sut.Files("directory/subdir/*.*"));
            Assert.Equal(new[] {"subfile.txt"}, sut.Files("directory/subdir/*.*").Select(f => f.FileName));

        }



        [Fact]
        void RecursiveFiles()
        {
            var sut =
                Factory.CreateBuilder()
                    .WithFile("firstfile.txt")
                    .WithFile("secondfile.bmp")
                    .WithDirectory("directory")
                        .MakeDirectory("subdir")
                        .WithFile("subfile.txt")
                        .Build();

            Assert.Equal(new[] {"firstfile.txt", "secondfile.bmp", "subfile.txt"},
                sut.RecursiveFiles().Select(f => f.FileName));

            Assert.Equal(new[] {"firstfile.txt", "subfile.txt"}, sut.RecursiveFiles("txt").Select(f => f.FileName));
            Assert.Equal(new[] { "firstfile.txt", "subfile.txt" }, sut.RecursiveFiles(".txt").Select(f => f.FileName));
        }


        //[Fact]
        //void AddFile_ThrowsOnNull()
        //{
        //    var sut = Factory.CreateBuilder().Build();

        //    Assert.Throws<ArgumentNullException>(() => sut.AddFileToHierarchy(null));
        //}


        //[Theory, AutoDomainData]
        //void AddFileToHierarchy_OnRoot_AddsFileCorrectly(IUrlDir innerDir, IFileSystemFactory fsFactory, IUrlFile file)
        //{
        //    var sut = new KSPDirectory(fsFactory, innerDir);

        //    file.Name.Returns("testfile");
        //    file.Url.Returns("/testfile");

        //    sut.AddFileToHierarchy(file);

        //    innerDir.Received(1).AddFile(Arg.Is(file));
        //}


        //[Theory, AutoDomainData]
        //void AddFileToHierarchy_CallsInnerAddFile_WithSubdirectoryFile(IFileSystemFactory fsFactory, IUrlDir root, IUrlDir sub,
        //    IUrlFile file)
        //{
        //    var sut = new KSPDirectory(fsFactory, root);

        //    fsFactory.GetDirectory(Arg.Is(sub)).Returns(ci => new KSPDirectory(fsFactory, sub));

        //    root.Name.Returns("root");
        //    root.Parent.Returns((IUrlDir)null);

        //    root.Children.Returns(new[] {sub});
        //    sub.Name.Returns("subdir");
        //    sub.Url.Returns("/subdir");
        //    sub.Parent.Returns(root);

        //    file.Url.Returns("/subdir/testfile");
        //    file.Name.Returns("testfile.txt");

        //    sut.AddFileToHierarchy(file);

        //    root.DidNotReceive().AddFile(Arg.Any<IUrlFile>()); // root should NOT receive this call!
        //    sub.Received(1).AddFile(Arg.Is(file));

        //    throw new NotImplementedException();
        //}



        [Fact]
        void FullPath_Property()
        {
            var sut = Factory.CreateBuilder("GameData").Build();

            Assert.Equal("C:/GameData/", sut.FullPath);

        }



        [Fact]
        void Parent_Property_IsNull_FromRoot()
        {
            var sut = Factory.CreateBuilder().Build();

            Assert.False(sut.Parent.Any());
        }


        [Fact]
        void Parent_Property_IsNotNull_FromSubdir()
        {
            var sut = Factory.CreateBuilder().MakeDirectory("subdir").Build();

            var result = sut.Directory(new KSPUrlIdentifier("subdir")).Single();

            Assert.True(result.Parent.Any());
        }



        /// <summary>
        /// Note: Urls for a directory in KSP consists of just the directory's name with no
        /// slashes. Only files receive a "fully qualified" url
        /// </summary>
        [Fact]
        void Url_Property_MatchesFormat()
        {
            var sut = Factory.CreateBuilder().WithDirectory("subdir").Build();

            var subdir = sut.Directory(new KSPUrlIdentifier("subdir")).Single();

            Assert.Equal("GameData", sut.Url);
            Assert.Equal("subdir", subdir.Url);
        }



        [Fact]
        void UrlDir_Property_ReturnsCorrectObject()
        {
            var urlSub = Substitute.For<IUrlDir>();

            var sut = Factory.Create("GameData", urlSub);

            Assert.Same(urlSub, sut.UrlDir);
        }



        [Fact]
        void Name_Property_ReturnsInCorrectFormat()
        {
            var sut = Factory.CreateBuilder("GameData").WithDirectory("subdir").Build();

            var subdir = sut.Directory(new KSPUrlIdentifier("subdir")).Single();

            Assert.Equal("GameData", sut.Name);
            Assert.Equal("subdir", subdir.Name);
        }
    }
}
