using System;

namespace ReeperKSP.Serialization.Exceptions
{
    public class WrongNativeSerializerException : ReeperSerializationException
    {
        public WrongNativeSerializerException(Type expectedType, object recvd)
            : base(
                string.Format("Object of type {0} requires a native serializer of type {1}", recvd.GetType().FullName,
                    expectedType.FullName))
        {
        }
    }
}
