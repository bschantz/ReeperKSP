using System;
using System.IO;

namespace ReeperKSP.FileSystem
{
    public class TemporaryFile : IDisposable
    {
        public string Path { get; private set; }

        public TemporaryFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path))
                throw new FileNotFoundException("Temporary file was not created", path);

            Path = path;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (File.Exists(Path))
                File.Delete(Path);
        }
    }
}
