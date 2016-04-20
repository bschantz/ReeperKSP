using System;

namespace ReeperKSP.Serialization
{
    public interface IConfigNodeItemSerializer
    {
        //void Serialize(Type type, ref object target, string uniqueKey, ConfigNode config, IConfigNodeSerializer serializer);
        //object Deserialize(Type type, ref object target, string uniqueKey, ConfigNode config, IConfigNodeSerializer serializer);

        void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer);
        void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer);
    }


    public interface IConfigNodeItemSerializer<T> : IConfigNodeItemSerializer
    {
    }
}
