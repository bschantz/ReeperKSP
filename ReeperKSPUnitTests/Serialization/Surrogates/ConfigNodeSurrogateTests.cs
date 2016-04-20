using Ploeh.AutoFixture;
using ReeperKSP.Serialization.Surrogates;
using ReeperKSPUnitTests.Fixtures;
using ReeperKSPUnitTests.Serialization.Surrogates;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperKSP.Serialization.Tests.Surrogates
{
    public class ConfigNodeSurrogateTests
    {
        public class ObjectWithConfigNodeField : IReeperPersistent
        {
            [ReeperPersistent] public ConfigNode ConfigNodeField = new ConfigNode("ConfigNodeName");

            public void DuringSerialize(IConfigNodeSerializer serializer, ConfigNode node)
            {

            }

            public void DuringDeserialize(IConfigNodeSerializer serializer, ConfigNode node)
            {

            }
        }

        static class RndConfigNodeFactory
        {
            private static readonly Fixture FactoryFixture;

            static RndConfigNodeFactory()
            {
                FactoryFixture = new Fixture();
                FactoryFixture.RepeatCount = 10;
            }

            public static ConfigNode Create()
            {
                var root = CreateANode();
                root.AddNode(CreateANode());

                return root;
            }


            private static ConfigNode CreateANode()
            {
                var cfg = new ConfigNode(FactoryFixture.CreateAnonymous<string>());

                foreach (var v in FactoryFixture.CreateMany<string>())
                    cfg.AddValue(FactoryFixture.CreateAnonymous<string>(), v);


                return cfg;
            }
        }


        [Theory, AutoDomainData]
        public void Serialize_Test(ConfigNodeSurrogate sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var data = RndConfigNodeFactory.Create();
            var obj = (object)data;
            sut.Serialize(typeof(ConfigNode), ref obj, key, config, serializer);


            Assert.True(config.HasData);
            Assert.True(config.HasNode(key));
            Assert.True(config.GetNode(key).HasNode(data.name));
            Assert.True(config.GetNode(key).GetNode(data.name).HasData);
        }


        [Theory, AutoDomainData]
        public void Deserialize_Test(ConfigNodeSurrogate sut, string key, ConfigNode config,
            IConfigNodeSerializer serializer)
        {
            var n = config.AddNode(key);
            n = n.AddNode("Outer");
            n.AddValue("TestName", "TestValue");
            var n2 = n.AddNode("InnerNode");
            n2.AddValue("InnerName", "InnerValue");
            var data = new ConfigNode("NodeName");
            var objData = (object) data;

            sut.Deserialize(typeof (ConfigNode), ref objData, key, config, serializer);
            data = (ConfigNode) objData;

            Assert.True(data.HasData);
            Assert.Same("Outer", data.name);
            Assert.True(data.HasValue("TestName"));
            Assert.Equal("TestValue", data.GetValue("TestName"));
            Assert.True(data.HasNode("InnerNode"));
            Assert.True(data.GetNode("InnerNode").HasValue("InnerName"));
            Assert.Same("InnerValue", data.GetNode("InnerNode").GetValue("InnerName"));
        }


        [Theory, AutoDomainData]
        public void Serialize_ThenDeserialize_ResultsInSameData_Test(ConfigNodeSurrogate sut, string key,
            ConfigNode config, ConfigNodeSerializer serializer)
        {
            config.name = "Top";
            var testConfig = new ConfigNode("TestConfig");
            testConfig.AddValue("NameOne", "ValueOne");
            testConfig.AddValue("NameTwo", "ValueTwo");
            testConfig.AddNode("SubNode");

            var testObj = (object) testConfig;

            sut.Serialize(typeof (ConfigNode), ref testObj, key, config, serializer);

            Assert.True(config.HasData);

            var deserializedConfig = new ConfigNode("WrongName");
            var deserializedObj = (object) deserializedConfig;

            sut.Deserialize(typeof (ConfigNode), ref deserializedObj, key, config, serializer);

            deserializedConfig = (ConfigNode) deserializedObj;

            Assert.True(testConfig.HasData);
            Assert.True(deserializedConfig.name == testConfig.name);
            Assert.True(deserializedConfig.HasValue("NameOne"));
            Assert.True(deserializedConfig.HasValue("NameTwo"));
            Assert.True(deserializedConfig.HasNode("SubNode"));
            Assert.True(ConfigNodeComparer.Similar(testConfig, deserializedConfig));
        }


        [Theory, AutoDomainData]
        public void Serialize_ObjectWithPersistentField_Test(ConfigNodeSerializer serializer)
        {
            var testObject = new ObjectWithConfigNodeField();
            testObject.ConfigNodeField.AddValue("Test", "Value");

            var result = serializer.CreateConfigNodeFromObject(testObject);

            Assert.True(result.HasData);
            Assert.True(result.HasNode("ConfigNodeField"));
        }


        [Theory, AutoDomainData]
        public void Deserialize_ObjectWithPersistentField_StartingWithNullFieldValue_Test(ConfigNodeSerializer serializer, ConfigNode config)
        {
            config.AddNode("ConfigNodeField").AddNode("NameOfConfigNodeInField");
            config.AddNode("ReeperCommon.Serialization.Tests.Surrogates.ConfigNodeSurrogateTests+ObjectWithConfigNodeField:" + NativeSerializer.NativeNodeName);

            var testObject = new ObjectWithConfigNodeField {ConfigNodeField = null};

            serializer.LoadObjectFromConfigNode(ref testObject, config);

            Assert.NotNull(testObject);
            Assert.NotNull(testObject.ConfigNodeField);
            Assert.True(testObject.ConfigNodeField.name == "NameOfConfigNodeInField");
        }
    }
}
