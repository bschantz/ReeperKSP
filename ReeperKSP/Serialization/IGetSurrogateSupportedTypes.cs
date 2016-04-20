using System;
using System.Collections.Generic;

namespace ReeperKSP.Serialization
{
    public interface IGetSurrogateSupportedTypes
    {
        IEnumerable<Type> Get(Type surrogateType);
    }
}
