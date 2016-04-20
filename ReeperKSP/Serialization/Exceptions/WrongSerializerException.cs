using System;

namespace ReeperKSP.Serialization.Exceptions
{
    // don't expect to see this unless I've screwed up somewhere with the serializer selector
    public class WrongSerializerException : ReeperSerializationException
    {
        public WrongSerializerException(Type objectType, Type expected)
            : base("This serializer is for " + expected.FullName + "; received " + objectType.FullName)
        {
        }

        public WrongSerializerException() : base("Wrong serializer for given type")
        {
            
        }

        public WrongSerializerException(string message) : base(message)
        {
            
        }

        public WrongSerializerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
