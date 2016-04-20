using System.Collections.Generic;
using ReeperKSP.Serialization;

namespace ReeperKSPUnitTests.Serialization.Complex
{
    public class SerializeObjectWithComplexFieldsAndNative : IReeperPersistent
    {
        public class InnerPersistent
        {
            [ReeperPersistent, Persistent] public string SomeStringField = "That has a value";
        }

        [ReeperPersistent, Persistent] public string HelloWorldField = "Hello, world!";
        [ReeperPersistent, Persistent] public ConfigNode SimpleConfigNodeField = new ConfigNode("Simple");
        [ReeperPersistent, Persistent] public List<float> FloatListField = new List<float> { 1f, 2f, 3f };
        [ReeperPersistent, Persistent] public List<string> StringListField = new List<string> { "apple", "banana", "pear" };
        [ReeperPersistent, Persistent] public List<ConfigNode> ConfigNodeListField = new List<ConfigNode>
        {
            new ConfigNode("first"),
            new ConfigNode("second"),
            new ConfigNode("third")
        };
        [ReeperPersistent, Persistent]
        public List<InnerPersistent> InnerPersistentListField = new List<InnerPersistent>
        {
            new InnerPersistent(),
            new InnerPersistent(),
            new InnerPersistent()
        }; 

        public void DuringSerialize(IConfigNodeSerializer serializer, ConfigNode node)
        {
            node.AddValue("complexName", "complexValue");
        }

        public void DuringDeserialize(IConfigNodeSerializer serializer, ConfigNode node)
        {

        }
    }
}
