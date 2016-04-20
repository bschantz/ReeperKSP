using System;

namespace ReeperKSP.Serialization.Exceptions
{
    public class DuplicateSurrogateException : ReeperSerializationException
    {
        public DuplicateSurrogateException(Type serializedType)
            : base("Already contains a surrogate for " + serializedType.FullName)
        {
        }
    }
}
