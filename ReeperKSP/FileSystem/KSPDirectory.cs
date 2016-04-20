using System;
using System.Collections.Generic;
using System.Linq;
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


        //public void AddFileToHierarchy(IUrlFile file)
        //{
        //    if (file == null) throw new ArgumentNullException("file");

        //    throw new NotImplementedException();
        //    //var id = new KSPUrlIdentifier(file.Url);


        //    //if (id.Depth > 1) // find the correct subdir 
        //    //{
        //    //    var subdirUrl = new KSPUrlIdentifier(id.Parts.Take(id.Depth - 1).Aggregate((s1, s2) => s1 + "/" + s2));
        //    //    var subdir = Directory(subdirUrl);

        //    //    if (!subdir.Any())
        //    //        throw new DirectoryNotFoundException(subdirUrl.Url);

        //    //    subdir.Value.AddFile();
        //    //}

        //    //RootDirectory.AddFile(file);
        //}


        public IEnumerable<IFile> Files(string extension)
        {
            var sanitized = extension.TrimStart('.');

            var files = Files();

            var withExt = files.Where(f => f.Extension == sanitized);

            return withExt;
        }



        public IEnumerable<IFile> RecursiveFiles()
        {
            return
                RootDirectory.AllFiles
               .Select(url => FileSystemFactory.GetFile(FileSystemFactory.GetDirectory(url.Directory), url));
        }



        public IEnumerable<IFile> RecursiveFiles(string extension)
        {
            var sanitized = extension.TrimStart('.');

            return RecursiveFiles().Where(f => f.Extension == sanitized);
        }




        public Maybe<IFile> File(IUrlIdentifier url)
        {
            var filename = System.IO.Path.GetFileName(url.Path);
            var dirPath = System.IO.Path.GetDirectoryName(url.Path);


            if (!string.IsNullOrEmpty(dirPath))
            {
                var owningDirectory = Directory(new KSPUrlIdentifier(dirPath));

                return !owningDirectory.Any()
                    ? Maybe<IFile>.None
                    : owningDirectory.Single().File(new KSPUrlIdentifier(filename));
            }

   
            var file = RootDirectory.Files
                .FirstOrDefault(f => f.Name == System.IO.Path.GetFileNameWithoutExtension(filename) &&
                                     (((System.IO.Path.HasExtension(filename) && System.IO.Path.GetExtension(filename) == ("." + f.Extension)))
                                      ||
                                      (!System.IO.Path.HasExtension(filename))));


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
