using System;
using System.IO;
using System.Linq;
using ReeperCommon.Containers;
using ReeperCommon.Extensions;
using ReeperCommon.Repositories;
using ReeperKSP.FileSystem;
using UnityEngine;

namespace ReeperKSP.Repositories
{
    public class ResourceFromDirectory : IResourceRepository
    {
        private readonly IDirectory _directory;
        private readonly float _accessTimeout;


// ReSharper disable once IntroduceOptionalParameters.Global
        public ResourceFromDirectory(IDirectory directory) : this(directory, 1f)
        {
            
        }


// ReSharper disable once MemberCanBePrivate.Global
        public ResourceFromDirectory(IDirectory directory, float accessTimeout)
        {
            if (directory == null) throw new ArgumentNullException("directory");
            _directory = directory;
            _accessTimeout = accessTimeout;
        }



        private Maybe<WWW> LoadFromDisk(string path)
        {
            // windows requires three slashes.  see:
            // http://docs.unity3d.com/Documentation/ScriptReference/WWW.html
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (!path.StartsWith("file:///"))
                    path = "file:///" + path;
            }
            else if (!path.StartsWith("file://")) path = "file://" + path;

            // WWW.EscapeURL doesn't seem to work all that great.  I couldn't get
            // AudioClips to come out of it correctly.  Non-escaped local urls
            // worked just fine but the docs say they should be escaped and this
            // works so I think it's the best solution currently
            //WWW clipData = new WWW(WWW.EscapeURL(path));
            var data = new WWW(System.Uri.EscapeUriString(path));

            float start = Time.realtimeSinceStartup;

            while (!data.isDone && Time.realtimeSinceStartup - start < _accessTimeout)
            {
            }


            return data.isDone ? Maybe<WWW>.With(data) : Maybe<WWW>.None;
        }



        public Maybe<byte[]> GetRaw(string identifier)
        {
            var file = _directory.File(new KSPUrlIdentifier(identifier));

            return file.IsNull() ? Maybe<byte[]>.None : Maybe<byte[]>.With(System.IO.File.ReadAllBytes(file.Single().FullPath));
        }



        public Maybe<Material> GetMaterial(string identifier)
        {
            var data = GetRaw(identifier);

            if (!data.Any()) return Maybe<Material>.None;

            var material = new Material(Convert.ToString(data.First()));

            return material.IsNull() ? Maybe<Material>.None : Maybe<Material>.With(material);
        }



        public Maybe<Texture2D> GetTexture(string identifier)
        {
            var file = _directory.File(new KSPUrlIdentifier(identifier));

            if (!file.Any()) return Maybe<Texture2D>.None;

            // we could LoadImage here, but since we're already implementing WWW for AudioClips
            // we might as well be consistent
            var data = LoadFromDisk(file.Single().FullPath);

            return (data.Any() && data.Single().isDone) ? Maybe<Texture2D>.With(data.Single().texture) : Maybe<Texture2D>.None;
        }



        public Maybe<AudioClip> GetClip(string identifier)
        {
            if (!_directory.FileExists(new KSPUrlIdentifier(identifier))) return Maybe<AudioClip>.None;

            var data = LoadFromDisk(_directory.File(new KSPUrlIdentifier(identifier)).Single().FullPath);

            return (data.Any() && data.Single().isDone) ? Maybe<AudioClip>.With(data.Single().audioClip) : Maybe<AudioClip>.None;
        }

        public Maybe<Stream> GetStream(string identifier)
        {
            if (!_directory.FileExists(new KSPUrlIdentifier(identifier))) return Maybe<Stream>.None;

            var stream = new FileStream(_directory.File(new KSPUrlIdentifier(identifier)).Single().FullPath, FileMode.Open);

            return stream.CanRead && stream.CanSeek ? Maybe<Stream>.With(stream) : Maybe<Stream>.None;
        }


        public override string ToString()
        {
            return string.Format("ResourceFromDirectory: '{0}'", _directory.Url) + System.Environment.NewLine +
                   string.Join(System.Environment.NewLine,
                       _directory.Directories().Select(d => "Directory: " + d.Url).Union(
                           _directory.Files().Select(f => "File: " + f.Url + "/" + f.FileName)).ToArray());
        }
    }
}
