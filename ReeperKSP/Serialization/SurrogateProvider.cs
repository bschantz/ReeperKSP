using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReeperCommon.Containers;


namespace ReeperKSP.Serialization
{
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    /// <summary>
    /// This is your standard, non-generic surrogate provider which uses lazy loading to cache
    /// serializers
    /// </summary>
    public class SurrogateProvider : ISurrogateProvider
    {
        // This simply prevents creating a new serializer on every single request, by lazily
        // creating them as requested and then keeping them for reuse
        protected class Surrogate
        {
            private readonly Lazy<Maybe<IConfigNodeItemSerializer>> _surrogate;
 
            public Surrogate(IEnumerable<SurrogateFactoryMethod> factories) // note: first successful factory result will be used
            {
                if (factories == null) throw new ArgumentNullException("factories");

                _surrogate = new Lazy<Maybe<IConfigNodeItemSerializer>>(() =>
                {
                    foreach (var f in factories)
                    {
                        var surrogate = f();
                        if (surrogate.Any())
                            return surrogate;
                    }

                    return Maybe<IConfigNodeItemSerializer>.None;
                });
            }

            public Maybe<IConfigNodeItemSerializer> Value
            {
                get { return _surrogate.Value; }
            } 
        }

        private readonly IGetSerializationSurrogates _getSerializationSurrogates;
        private readonly IGetSurrogateSupportedTypes _getSurrogateSupportedTypes;
        private readonly IEnumerable<Assembly> _assembliesToSearch;

// ReSharper disable once FieldCanBeMadeReadOnly.Global
// ReSharper disable once MemberCanBePrivate.Global
        protected readonly Lazy<Dictionary<Type, Surrogate>> Surrogates;


        public SurrogateProvider(
            IGetSerializationSurrogates getSerializationSurrogates, 
            IGetSurrogateSupportedTypes getSurrogateSupportedTypes,
            IEnumerable<Assembly> assembliesToSearch)
        {
            if (getSerializationSurrogates == null) throw new ArgumentNullException("getSerializationSurrogates");
            if (getSurrogateSupportedTypes == null) throw new ArgumentNullException("getSurrogateSupportedTypes");
            if (assembliesToSearch == null) throw new ArgumentNullException("assembliesToSearch");
       
            _getSerializationSurrogates = getSerializationSurrogates;
            _getSurrogateSupportedTypes = getSurrogateSupportedTypes;
            _assembliesToSearch = assembliesToSearch;

            Surrogates = new Lazy<Dictionary<Type, Surrogate>>(Initialize);
        }


        protected virtual Dictionary<Type, Surrogate> Initialize()
        {
            var surrogateTypes = _assembliesToSearch
                .SelectMany(targetAssembly => _getSerializationSurrogates.Get(targetAssembly))
                .Where(t => !t.IsAbstract && !t.ContainsGenericParameters && !t.IsGenericTypeDefinition)
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null && t.GetConstructor(Type.EmptyTypes).IsPublic)
                .Where(t => _getSurrogateSupportedTypes.Get(t).Any());

            var dictionary = new Dictionary<Type, List<SurrogateFactoryMethod>>();

            foreach (var surrogateType in surrogateTypes)
            {
                var supportedSerializedTypes = _getSurrogateSupportedTypes.Get(surrogateType);

                foreach (var serializedType in supportedSerializedTypes)
                {
                    List<SurrogateFactoryMethod> list;
                    var typeToBeSerialized = serializedType;
                    var surrogateTypeToCreate = surrogateType;

                    SurrogateFactoryMethod m = () => CreateSurrogate(typeToBeSerialized, surrogateTypeToCreate);

                    if (dictionary.TryGetValue(serializedType, out list))
                        list.Add(m);
                    else dictionary.Add(serializedType, new List<SurrogateFactoryMethod> {m});
                }
            }

            return dictionary.ToDictionary(kvp => kvp.Key, kvp => new Surrogate(kvp.Value));
        }


        private static Maybe<IConfigNodeItemSerializer> CreateSurrogate(Type typeToBeSerialized, Type surrogateType)
        {
            if (typeToBeSerialized == null) throw new ArgumentNullException("typeToBeSerialized");
            if (surrogateType == null) throw new ArgumentNullException("surrogateType");

            if (!typeof(IConfigNodeItemSerializer).IsAssignableFrom(surrogateType))
                throw new ArgumentException(surrogateType.FullName + " cannot be assigned to IConfigNodeItemSerializer");

            return (Activator.CreateInstance(surrogateType) as IConfigNodeItemSerializer).ToMaybe();
        }


        public Maybe<IConfigNodeItemSerializer> Get(Type toBeSerialized)
        {
            if (toBeSerialized == null) throw new ArgumentNullException("toBeSerialized");
            if (toBeSerialized.IsGenericTypeDefinition)
                throw new ArgumentException("Cannot provide surrogate for incomplete type");

            Surrogate surrogate;

            return !Surrogates.Value.TryGetValue(toBeSerialized, out surrogate) ? Maybe<IConfigNodeItemSerializer>.None : surrogate.Value;
        }
    }
}
