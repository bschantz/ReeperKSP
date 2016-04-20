using System;

namespace ReeperKSP.FileSystem
{
    public class DirectoryNotFoundException : Exception
    {
        public DirectoryNotFoundException() : base("Specified directory does not exist")
        {
            
        }


        public DirectoryNotFoundException(string name) : base("Directory \"" + name + "\" does not exist")
        {
            
        }

        public DirectoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
