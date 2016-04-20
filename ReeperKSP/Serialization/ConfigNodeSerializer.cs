using System;
using System.Linq;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization
{
    public class ConfigNodeSerializer : IConfigNodeSerializer
    {
        private ISerializerSelector _serializerSelector;


        public ConfigNodeSerializer(ISerializerSelector selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            SerializerSelector = selector;
        }


        public ISerializerSelector SerializerSelector
        {
            get { return _serializerSelector; }
            set
            {
                if (value == null) throw new ArgumentException("value cannot be null");
                _serializerSelector = value;
            }
        }


        public ConfigNode CreateConfigNodeFromObject(object target)
        {
// ReSharper disable once CompareNonConstrainedGenericWithNull
            if (target == null) throw new ArgumentNullException("target");

            var config = new ConfigNode(target.GetType().FullName);

            WriteObjectToConfigNode(ref target, config);

            return config;
        }


        public void WriteObjectToConfigNode(ref object source, ConfigNode config)
        {
// ReSharper disable once CompareNonConstrainedGenericWithNull
            if (source == null) throw new ArgumentNullException("source");
            if (config == null) throw new ArgumentNullException("config");

            GetSerializer(source.GetType())
                .Serialize(source.GetType(), ref source, source.GetType().FullName, config, this);
        }

        public void WriteObjectToConfigNode<T>(ref T source, ConfigNode config)
        {
            if (config == null) throw new ArgumentNullException("config");

            var src = (object) source;
            WriteObjectToConfigNode(ref src, config);
        }


        public void LoadObjectFromConfigNode(ref object target, ConfigNode config)
        {
            if (target == null) throw new ArgumentNullException("target");

            GetSerializer(target.GetType()).Deserialize(target.GetType(), ref target, target.GetType().FullName, config, this);
        }

        public void LoadObjectFromConfigNode<T>(ref T target, ConfigNode config)
        {
            if (config == null) throw new ArgumentNullException("config");

            var deserialized = (object) target;

            LoadObjectFromConfigNode(ref deserialized, config);

            target = (T) deserialized;
        }


        private IConfigNodeItemSerializer GetSerializer(Type type)
        {
            var serializer = SerializerSelector.GetSerializer(type);

            if (!serializer.Any())
                throw new NoSerializerFoundException(type);

            return serializer.Single();
        }


        public override string ToString()
        {
            return "ConfigNodeSerializer: " + SerializerSelector;
        }
    }
}
