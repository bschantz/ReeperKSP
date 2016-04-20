using System;

namespace ReeperKSP.Serialization.Exceptions
{
    public class NoConversionException : ReeperSerializationException
    {
        public NoConversionException(Type from, Type to)
            : base("No conversion from " + from.FullName + " to " + to.FullName + " available")
        {
        }
    }
}
