using System;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    /// <summary>
    /// Calls IPersistenceSave/IPersistenceLoad on serialized type at appropriate time
    /// </summary>
    public class PersistenceMethodCaller : IConfigNodeItemSerializer
    {
        public readonly Maybe<IConfigNodeItemSerializer> DecoratedSerializer;

        public PersistenceMethodCaller(Maybe<IConfigNodeItemSerializer> decoratedSerializer)
        {
            DecoratedSerializer = decoratedSerializer;
        }


        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (target == null) return;

            var persistObject = target as IPersistenceSave;

            if (persistObject != null && typeof(IPersistenceSave).IsAssignableFrom(type))
                persistObject.PersistenceSave();

            if (DecoratedSerializer.HasValue)
                DecoratedSerializer.Value.Serialize(type, ref target, key, config, serializer);
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (DecoratedSerializer.HasValue)
                DecoratedSerializer.Value.Deserialize(type, ref target, key, config, serializer);

            var persistObject = target as IPersistenceLoad;

            if (persistObject != null && typeof(IPersistenceLoad).IsAssignableFrom(type))
                persistObject.PersistenceLoad();
        }
    }
}
