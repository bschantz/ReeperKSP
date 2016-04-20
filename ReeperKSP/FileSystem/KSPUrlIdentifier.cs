using System;

namespace ReeperKSP.FileSystem
{
// ReSharper disable once InconsistentNaming
    public class KSPUrlIdentifier : IUrlIdentifier
    {
        private readonly string _url;
        private readonly string _path;
        private readonly UrlType _type;


// ReSharper disable once MemberCanBePrivate.Global
        public KSPUrlIdentifier(string url, UrlType type)
        {
            if (url == null) throw new ArgumentNullException("url");

            url = url.Replace('\\', '/');
            url = url.Trim('\\', '/');
            
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url is empty");

            if (url.StartsWith("."))
                throw new ArgumentException("url cannot start with a period");

            Parts = url.Split('/', '\\');

            _path = url;
            _url = "/" + (System.IO.Path.HasExtension(url) ? System.IO.Path.GetFileName(url) : url);
            _type = type;
        }


        public KSPUrlIdentifier(string url) : this(url, InferFileTypeFromUrl(url))
        {

        }



        public string this[int i] 
        {
            get { return Parts[i]; }
        }



        public string Url { get { return _url; } }
        public string Path { get { return _path; } }
        public int Depth { get { return Parts.Length;  }}
        public string[] Parts { get; private set; }

        public UrlType Type
        {
            get { return _type; }
        }


        private static UrlType InferFileTypeFromUrl(string url)
        {
            var extension = (System.IO.Path.GetExtension(url) ?? string.Empty).ToLowerInvariant();

            switch (extension)
            {
                case "dll":
                    return UrlType.Assembly;

                case "mu":
                    return UrlType.Model;

                case "cfg":
                    return UrlType.Config;

                case "wav":
                case "ogg":
                    return UrlType.Audio;

                case "bmp":
                case "mbm":
                case "dds":
                case "png":
                case "jpg":
                    return UrlType.Texture;

                default:
                    return UrlType.Unknown;
            }
        }
    }
}