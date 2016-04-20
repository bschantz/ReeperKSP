using System;

namespace ReeperKSP.Serialization.Exceptions
{
    public class NoSerializerFoundException : ReeperSerializationException
    {
        public NoSerializerFoundException(Type type):base(string.Format("No serializer found for {0}", type.FullName))
        {
            
        }
    }
}
