using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReeperKSP.Serialization
{
    /// <summary>
    /// Retrieve surrogates from target assembly. Note that abstract aren't permitted
    /// </summary>
    public class GetSerializationSurrogates : IGetSerializationSurrogates
    {
        public readonly IGetSurrogateSupportedTypes SurrogateSupportedTypesQuery;

        public GetSerializationSurrogates(IGetSurrogateSupportedTypes surrogateSupportedTypesQuery)
        {
            if (surrogateSupportedTypesQuery == null) throw new ArgumentNullException("surrogateSupportedTypesQuery");
            SurrogateSupportedTypesQuery = surrogateSupportedTypesQuery;
        }


        public IEnumerable<Type> Get(Assembly fromAssembly)
        {
            return fromAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.IsVisible || ReferenceEquals(Assembly.GetExecutingAssembly(), fromAssembly))
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null && t.GetConstructor(Type.EmptyTypes).IsPublic)
                .Where(ImplementsGenericSerializationSurrogateInterface);
        }


        private bool ImplementsGenericSerializationSurrogateInterface(Type typeCheck)
        {
            return SurrogateSupportedTypesQuery.Get(typeCheck).Any();
        }
    }
}
