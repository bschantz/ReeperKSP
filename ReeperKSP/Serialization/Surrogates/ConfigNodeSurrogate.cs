using System;
using System.Linq;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization.Surrogates
{
// ReSharper disable once UnusedMember.Global
// ReSharper disable once ClassNeverInstantiated.Global
    public class ConfigNodeSurrogate : IConfigNodeItemSerializer<ConfigNode>
    {
        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if ((target != null && !type.IsInstanceOfType(target)) || type != typeof(ConfigNode))
                throw new WrongSerializerException(type, typeof(ConfigNode));
            if (target == null) return;

            if (config.HasNode(key))
                throw new ConfigNodeDuplicateKeyException(key, config);

            var copy = ((ConfigNode)target).CreateCopy();

            var sub = config.AddNode(key);
            sub.AddNode(copy);
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if ((target != null && !type.IsInstanceOfType(target)) || type != typeof(ConfigNode))
                throw new WrongSerializerException(type, typeof(ConfigNode));

            if (!config.HasNode(key))
            {
                if (target != null)
                    ((ConfigNode) target).ClearData();
                return;
            }

            var targetConfig = (ConfigNode)target ?? new ConfigNode();

            var serializedNodes = config.GetNodes(key).ToList();
            if (serializedNodes.Count > 1)
                throw new AmbiguousKeyException(key); // which one to use!?


            var scopingNode = serializedNodes.First();

            if (scopingNode.CountNodes != 1)
                throw new ReeperSerializationException("Scoping node has multiple subnodes");

            var sourceNode = scopingNode.GetNodes().First();

            targetConfig.ClearData();
            sourceNode.CopyTo(targetConfig);
            targetConfig.name = sourceNode.name;
            
            target = targetConfig;
        }
    }
}
