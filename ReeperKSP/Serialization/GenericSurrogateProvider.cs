using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReeperCommon.Containers;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization
{
    /// <summary>
    /// This surrogate provider knows how to construct surrogates from generic type definitions
    /// </summary>
    public class GenericSurrogateProvider : ISurrogateProvider
    {
        private readonly IGetSerializationSurrogates _surrogateQuery;
        private readonly IGetSurrogateSupportedTypes _supportedTypeQuery;
        private readonly IEnumerable<Assembly> _assembliesToScan;
        protected readonly Lazy<Dictionary<Type, GenericSurrogateFactoryMethod>> GenericSurrogates;

        public delegate Maybe<IConfigNodeItemSerializer> GenericSurrogateFactoryMethod(Type typeThatNeedsSerializing);

        public GenericSurrogateProvider(
            IGetSerializationSurrogates surrogateQuery,
            IGetSurrogateSupportedTypes supportedTypeQuery,
            IEnumerable<Assembly> assembliesToScan)
        {
            if (surrogateQuery == null) throw new ArgumentNullException("surrogateQuery");
            if (supportedTypeQuery == null) throw new ArgumentNullException("supportedTypeQuery");
            if (assembliesToScan == null) throw new ArgumentNullException("assembliesToScan");
            
            _surrogateQuery = surrogateQuery;
            _supportedTypeQuery = supportedTypeQuery;
            _assembliesToScan = assembliesToScan;

            GenericSurrogates = new Lazy<Dictionary<Type, GenericSurrogateFactoryMethod>>(Initialize);
        }


        protected virtual Dictionary<Type, GenericSurrogateFactoryMethod> Initialize()
        {
            var surrogateTypes = _assembliesToScan
                .SelectMany(targetAssembly => _surrogateQuery.Get(targetAssembly))
                .Where(t => !t.IsAbstract && t.IsGenericTypeDefinition && t.GetGenericArguments().Length == 1)
                .Where(t => t.GetConstructor(Type.EmptyTypes) != null && t.GetConstructor(Type.EmptyTypes).IsPublic);

            var dictionary = new Dictionary<Type, GenericSurrogateFactoryMethod>();

            foreach (var surrogateType in surrogateTypes)
                foreach (var supportedType in _supportedTypeQuery.Get(surrogateType))
                {
                    var type = surrogateType;

                    // note: we won't know the non-generic type we'll be serializing until somebody
                    // asks for a specific one through Get()
                    GenericSurrogateFactoryMethod m = (nonGenericTypeToSerialize) => CreateSurrogate(nonGenericTypeToSerialize, type);
                    dictionary.Add(supportedType.IsGenericTypeDefinition ? supportedType : supportedType.GetGenericTypeDefinition() ?? supportedType, m);
                }

            return dictionary;
        }


        private static Maybe<IConfigNodeItemSerializer> CreateSurrogate(Type typeToBeSerialized, Type surrogateType)
        {
            if (typeToBeSerialized == null) throw new ArgumentNullException("typeToBeSerialized");
            if (surrogateType == null) throw new ArgumentNullException("surrogateType");

            if (!typeof(IConfigNodeItemSerializer).IsAssignableFrom(surrogateType))
                throw new ArgumentException(surrogateType.FullName + " cannot be assigned to IConfigNodeItemSerializer");

            if (!typeToBeSerialized.IsGenericType)
                throw new ArgumentException("Type to be serialized is not generic", "typeToBeSerialized");

            // if this type contains any generic parameters, any generic type we make wouldn't be constructable anyway
            if (typeToBeSerialized.ContainsGenericParameters)
                throw new ArgumentException(typeToBeSerialized + " has unresolved generic parameters",
                    "typeToBeSerialized");

            if (!surrogateType.IsGenericTypeDefinition)
                throw new ArgumentException(surrogateType.FullName + " is not a generic type definition", "surrogateType");

            var concrete = surrogateType.MakeGenericType(new[] {typeToBeSerialized.GetGenericArguments().First()});

            if (concrete.ContainsGenericParameters)
                throw new ReeperSerializationException("Can't create a surrogate of type " + concrete.FullName +
                                                 " because it contains generic parameters");

            return (Activator.CreateInstance(concrete) as IConfigNodeItemSerializer).ToMaybe();
        }


        public Maybe<IConfigNodeItemSerializer> Get(Type toBeSerialized)
        {
            if (toBeSerialized == null) throw new ArgumentNullException("toBeSerialized");

            if (!toBeSerialized.IsGenericType || toBeSerialized.IsGenericTypeDefinition || toBeSerialized.GetGenericTypeDefinition() == null)
                return Maybe<IConfigNodeItemSerializer>.None;

            GenericSurrogateFactoryMethod factory;
            var genericDefOfToBeSerialized = toBeSerialized.GetGenericTypeDefinition();

            return GenericSurrogates.Value
                .TryGetValue(genericDefOfToBeSerialized ?? toBeSerialized, out factory) ? factory(toBeSerialized) : Maybe<IConfigNodeItemSerializer>.None;
        }
    }
}
