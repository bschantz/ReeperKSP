using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization.Surrogates
{
    public class PrimitiveSurrogateSerializer :
        IConfigNodeItemSerializer<string>,
        IConfigNodeItemSerializer<bool>,
        IConfigNodeItemSerializer<int>,
        IConfigNodeItemSerializer<float>,
        IConfigNodeItemSerializer<double>
    {
        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (target == null)
                return; // don't serialize nulls
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");

            CheckSupportedTypes(type);

            if (config.HasValue(key))
                throw new ConfigNodeDuplicateKeyException(key, config);

            var tc = TypeDescriptor.GetConverter(type);

            if (!tc.CanConvertTo(typeof(string)))
                throw new NoConversionException(type, typeof(string));

            if (!tc.IsValid(target))
                throw new Exception("target data of \"" + target + "\" is invalid for " + type.FullName + " TypeConverter");

            var strValue = tc.ConvertToInvariantString(target);

            config.AddValue(key, strValue);
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            CheckSupportedTypes(type);

            if (!config.HasValue(key))
                return; // no changes

            var tc = TypeDescriptor.GetConverter(type);

            if (!tc.CanConvertFrom(typeof(string)))
                throw new NoConversionException(typeof(string), type);

            var strValue = config.GetValue(key);

            target = tc.ConvertFromInvariantString(strValue);
        }


        public IEnumerable<Type> GetSupportedTypes()
        {
            return GetType()
                .GetInterfaces()
                .SelectMany(i => i.GetGenericArguments());
        }


        private void CheckSupportedTypes(Type targetType)
        {
            if (GetSupportedTypes().All(t => t != targetType))
                throw new WrongSerializerException(targetType.FullName + " is not supported by this surrogateSerializer. It handles " + string.Join(",", GetSupportedTypes().Select(t => t.FullName).ToArray()));
        }
    }
}
