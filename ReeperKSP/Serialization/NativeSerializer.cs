using System;
using System.Linq;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization
{
    public class NativeSerializer : IConfigNodeItemSerializer
    {
        public const string NativeNodeName = "NativeData";

        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (!type.IsInstanceOfType(target) && target != null) // if target is null, we might not be able to determine its type. But that's okay since nothing will be written
                throw new WrongNativeSerializerException(type, target);
            if (!typeof(IReeperPersistent).IsAssignableFrom(type))
                throw new ArgumentException("Couldn't cast " + type.FullName + " to " + typeof(IReeperPersistent).FullName);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Can't be null or empty");

            var reeperPersistent = target as IReeperPersistent;
            if (reeperPersistent == null)
                throw new Exception("Failed to cast target " + type.FullName + " to " +
                                    typeof(IReeperPersistent).FullName);

            // it's necessary to add a scope node here in case other same-level serialized objects are
            // also natively serialized; if they are, there would be multiple NativeData nodes

            var persistentConfig = config.AddNode(GetNativeDataNodeName(key));
            reeperPersistent.DuringSerialize(serializer, persistentConfig);
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (!type.IsInstanceOfType(target) && target != null) // if target is null, we might not be able to determine its type. But that's okay since nothing will be written
                throw new WrongNativeSerializerException(type, target);
            if (!typeof(IReeperPersistent).IsAssignableFrom(type))
                throw new ArgumentException("Couldn't cast " + type.FullName + " to " + typeof(IReeperPersistent).FullName);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Can't be null or empty");

            var canCreateDefault = type.GetConstructors().Any(ci => ci.GetParameters().Length == 0 && ci.IsPublic);

            if (!canCreateDefault && target == null)
                throw new NoDefaultValueException(type);

            var nativeNodeName = GetNativeDataNodeName(key);

            if (!config.HasNode(nativeNodeName))
                throw new ReeperSerializationException("Can't deserialize " + type.FullName + " because given ConfigNode is missing scope node");

            var reeperPersistent = (target ?? Activator.CreateInstance(type)) as IReeperPersistent;
            if (reeperPersistent == null) // uh ... how??
                throw new Exception("Failed to create instance of type " + type.FullName + " for unknown reasons");


            var configValue = config.GetNode(nativeNodeName);

            reeperPersistent.DuringDeserialize(serializer, configValue);

            target = reeperPersistent;
        }


        private static string GetNativeDataNodeName(string key)
        {
            return key + ":" + NativeNodeName;
        }
    }
}
