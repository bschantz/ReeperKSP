using System;
using System.Reflection;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
// ReSharper disable once ClassNeverInstantiated.Global
    public class DefaultConfigNodeSerializer : IConfigNodeSerializer
    {
        private readonly IConfigNodeSerializer _serializer;

        public DefaultConfigNodeSerializer(params Assembly[] assembliesToScanForSurrogates)
        {
            if (assembliesToScanForSurrogates == null) throw new ArgumentNullException("assembliesToScanForSurrogates");

            var supportedTypeQuery = new GetSurrogateSupportedTypes();
            var surrogateQuery = new GetSerializationSurrogates(supportedTypeQuery);
            var serializableFieldQuery = new GetSerializableFields();

            var standardSerializerSelector =
                new SerializerSelector(
                    new CompositeSurrogateProvider(
                        new GenericSurrogateProvider(surrogateQuery, supportedTypeQuery, assembliesToScanForSurrogates),
                        new SurrogateProvider(surrogateQuery, supportedTypeQuery, assembliesToScanForSurrogates)));

            var preferNativeSelector = new PreferNativeSerializer(standardSerializerSelector);
            var includePersistentFieldsSelector = new SerializerSelectorDecorator(
                preferNativeSelector,
                s => Maybe<IConfigNodeItemSerializer>.With(new FieldSerializer(s, serializableFieldQuery)));

            _serializer = new ConfigNodeSerializer(includePersistentFieldsSelector);
        }


        public ConfigNode CreateConfigNodeFromObject(object target)
        {
            return _serializer.CreateConfigNodeFromObject(target);
        }

        public void WriteObjectToConfigNode(ref object source, ConfigNode config)
        {
            _serializer.WriteObjectToConfigNode(ref source, config);
        }

        public void WriteObjectToConfigNode<T>(ref T source, ConfigNode config)
        {
            _serializer.WriteObjectToConfigNode(ref source, config);
        }

        public void LoadObjectFromConfigNode<T>(ref T target, ConfigNode config)
        {
            _serializer.LoadObjectFromConfigNode(ref target, config);
        }

        public ISerializerSelector SerializerSelector
        {
            get { return _serializer.SerializerSelector; }
            set { _serializer.SerializerSelector = value; }
        }
    }
}
