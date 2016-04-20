using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReeperKSP.Serialization
{
    public interface IGetSerializationSurrogates
    {
        IEnumerable<Type> Get(Assembly fromAssembly);
    }
}
