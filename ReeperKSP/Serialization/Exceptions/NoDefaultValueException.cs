using System;

namespace ReeperKSP.Serialization.Exceptions
{
    public class NoDefaultValueException : ReeperSerializationException
    {
        public NoDefaultValueException(Type type) : base("Could not create a default instance of " + type.FullName)
        {
        }
    }
}
