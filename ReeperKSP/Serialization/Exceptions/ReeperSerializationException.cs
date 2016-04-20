using System;

namespace ReeperKSP.Serialization.Exceptions
{
    public class ReeperSerializationException : Exception
    {
        public ReeperSerializationException() : base("An exception has occurred")
        {
            
        }

        public ReeperSerializationException(string message) : base(message)
        {
            
        }

        public ReeperSerializationException(string message, Exception inner)
            : base(message, inner)
        {
            
        }
    }
}
