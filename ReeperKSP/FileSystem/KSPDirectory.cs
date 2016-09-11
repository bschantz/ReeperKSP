using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using ReeperCommon.Containers;
using ReeperCommon.Extensions;

namespace ReeperKSP.FileSystem
{
// ReSharper disable once InconsistentNaming
    public class KSPDirectory : IDirectory
    {
        public readonly IFileSystemFactory FileSystemFactory;
        public readonly IUrlDir RootDirectory;


        public KSPDirectory(
            IFileSystemFactory fsFactory,
            IUrlDir root)
        {
            if (fsFactory == null) throw new ArgumentNullException("fsFactory");
            if (root.IsNull())
                throw new ArgumentNullException("root");


            FileSystemFactory = fsFactory;
            RootDirectory = root;
        }




        public Maybe<IDirectory> Directory(IUrlIdentifier url)
        {
            if (url.Depth < 1)
                return Maybe<IDirectory>.None;
            

            var dir = RootDirectory.Children.FirstOrDefault(d => d.Name == url[0]);
            if (dir.IsNull())
                return Maybe<IDirectory>.None;

            var found = FileSystemFactory.GetDirectory(dir);

            return url.Depth <= 1 ? 
                Maybe<IDirectory>.With(found) : 
                found.Directory(new KSPUrlIdentifier(url.Parts.Skip(1).Aggregate((s1, s2) => s1 + "/" + s2)));
        }



        public IEnumerable<IDirectory> Directories()
        {
            return RootDirectory.Children
                .Select(url => FileSystemFactory.GetDirectory(url));
        }



        public IEnumerable<IDirectory> RecursiveDirectories()
        {
            return RootDirectory.Children
                .Select(child => FileSystemFactory.GetDirectory(child))
                .Union(
                    RootDirectory.Children
                        .SelectMany(child => FileSystemFactory.GetDirectory(child).RecursiveDirectories()));
        }



        public Maybe<IDirectory> Parent
        {
            get { return RootDirectory.Parent.IsNull() ? Maybe<IDirectory>.None : Maybe<IDirectory>.With(FileSystemFactory.GetDirectory(RootDirectory.Parent)); }
        }



        public bool FileExists(IUrlIdentifier url)
        {
            return File(url).Any();
        }


        public bool FileExists(string filename)
        {
            var url = new KSPUrlIdentifier(filename);

            return url.Depth != 0 && FileExists(url);
        }



        public bool DirectoryExists(IUrlIdentifier url)
        {
            return Directory(url).Any();
        }



        public IEnumerable<IFile> Files()
        {
            return
                RootDirectory.Files
                .Select(url => FileSystemFactory.GetFile(this, url));
        }


        public IEnumerable<IFile> Files(string extension)
        {
            bool relative = extension.Contains("/") || extension.Contains("\\");

            if (relative)
            {
                var url = new KSPUrlIdentifier(extension);

                if (Url.Length <= 1) // eh??
                    return Enumerable.Empty<IFile>();

                // the last bit will be the filename, strip that out for the dir name
                var dirUrl = url.Parts.Take(url.Parts.Length - 1).Aggregate((s1, s2) => s1 + "/" + s2);
                var fileUrl = url.Parts.Last();

                var dir = Directory(new KSPUrlIdentifier(dirUrl, UrlType.Directory));

                return !dir.Any() ? Enumerable.Empty<IFile>() : dir.Value.Files(fileUrl);
            }

            var files = Files();

            var withExt = files.Where(f => MatchesExtension(extension, f));

            return withExt;
        }


        // strip away leading *s or periods; convert null or empty to *
        private static string GetSanitizedExtension(string extension)
        {
            return string.IsNullOrEmpty(extension) ? "*" : extension.TrimStart('*').TrimStart('.').With(s => string.IsNullOrEmpty(s) ? "*" : s).ToUpperInvariant();
        }


        private static bool MatchesExtension(string extension, [NotNull] IFile file)
        {
            if (string.IsNullOrEmpty(extension)) return true;
            if (file == null) throw new ArgumentNullException("file");

            var searchingForExtension = GetSanitizedExtension(extension);
            if (searchingForExtension == "*") return true;

            return GetSanitizedExtension(extension) == GetSanitizedExtension(file.Extension);
        }


        public IEnumerable<IFile> RecursiveFiles()
        {
            return
                RootDirectory.Files
                .Select(url => FileSystemFactory.GetFile(FileSystemFactory.GetDirectory(url.Directory), url))
                .Union(
                    RootDirectory.Children.SelectMany(
                        chDir => new KSPDirectory(FileSystemFactory, chDir).RecursiveFiles()));
        }



        public IEnumerable<IFile> RecursiveFiles(string extension)
        {
            return RecursiveFiles().Where(f => MatchesExtension(extension, f));
        }




        public Maybe<IFile> File(IUrlIdentifier url)
        {
            var filename = Path.GetFileName(url.Path);
            var dirPath = Path.GetDirectoryName(url.Path);


            if (!string.IsNullOrEmpty(dirPath))
            {
                var owningDirectory = Directory(new KSPUrlIdentifier(dirPath));

                return !owningDirectory.Any()
                    ? Maybe<IFile>.None
                    : owningDirectory.Single().File(new KSPUrlIdentifier(filename));
            }

   
            var file = RootDirectory.Files
                .FirstOrDefault(f => f.Name == Path.GetFileNameWithoutExtension(filename) &&
                                     (((Path.HasExtension(filename) && Path.GetExtension(filename) == ("." + f.Extension)))
                                      ||
                                      (!Path.HasExtension(filename))));


            return file.IsNull()
                ? Maybe<IFile>.None
                : Maybe<IFile>.With(FileSystemFactory.GetFile(this, file));
        }


        public Maybe<IFile> File(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("Filename cannot be null or empty", "filename");

            return File(new KSPUrlIdentifier(filename));
        }


        public string FullPath
        {
            get { return RootDirectory.FullPath; } // fully qualified path
        }



        public string Url { get { return RootDirectory.Url; } }


        public IUrlDir UrlDir { get { return RootDirectory; }}

        public string Name
        {
            get { return RootDirectory.Name; }
        }
    }
}
