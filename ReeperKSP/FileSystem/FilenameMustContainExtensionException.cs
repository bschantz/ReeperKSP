using System;

namespace ReeperKSP.FileSystem
{
    public class FilenameMustContainExtensionException : Exception
    {
        public FilenameMustContainExtensionException() : base("The provided filename must contain an extension")
        {
            
        }

        public FilenameMustContainExtensionException(string filename)
            : base("The filename \"" + filename + "\" must contain an extension")
        {
            
        }

        public FilenameMustContainExtensionException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
