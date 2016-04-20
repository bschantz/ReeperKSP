using System;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization.Surrogates
{
    public abstract class SingleValueSurrogate<T> : IConfigNodeItemSerializer<T>
    {
        protected virtual string GetFieldContentsAsString(T instance)
        {
// ReSharper disable once CompareNonConstrainedGenericWithNull
            return typeof(T).IsValueType || instance != null ? instance.ToString() : string.Empty;
        }


        protected abstract T GetFieldContentsFromString(string value);


        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if ((target != null ? target.GetType() : type) != typeof(T))
                throw new WrongSerializerException(type, typeof(T));

            config.AddValue(key, GetFieldContentsAsString((T)target));
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (!config.HasValue(key))
                return;

            var strValue = config.GetValue(key);

            target = GetFieldContentsFromString(strValue);
        }
    }
}
