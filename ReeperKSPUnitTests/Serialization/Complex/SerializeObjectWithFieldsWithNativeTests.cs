using System;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using UnityEngine;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommonUnitTests.Serialization.Tests.Complex
{
    public class SerializeObjectWithFieldsWithNativeTests
    {
        public class TestObject : IReeperPersistent
        {
            [ReeperPersistent] public string FieldOne = "FieldOneValue";
            [ReeperPersistent] public string FieldTwo = "FieldTwoValue";
            [ReeperPersistent] public Rect FieldRect = new Rect(11f, 22f, 33f, 44);

            public void DuringSerialize(IConfigNodeSerializer serializer, ConfigNode node)
            {
                node.AddValue("custom", "value");
            }

            public void DuringDeserialize(IConfigNodeSerializer serializer, ConfigNode node)
            {
                if (!node.HasValue("custom") || node.GetValue("custom") != "value")
                    throw new Exception("invalid custom data");
            }
        }


        [Theory, AutoDomainData]
        public void Serialize_CorrectlySerializesObject(ConfigNodeSerializer serializer, TestObject testObject)
        {
            var result = serializer.CreateConfigNodeFromObject(testObject);

            Assert.True(result.HasData);
            Assert.True(result.HasValue("FieldOne"));
            Assert.True(result.HasValue("FieldTwo"));
            Assert.True(result.HasNode("FieldRect"));
            Assert.True(result.GetNode("FieldRect").HasData);
            Assert.True(result.GetNode("FieldRect").HasValue("x"));
            Assert.True(result.GetNode("FieldRect").HasValue("y"));
            Assert.True(result.GetNode("FieldRect").HasValue("width"));
            Assert.True(result.GetNode("FieldRect").HasValue("height"));
        }


        [Theory, AutoDomainData]
        public void Deserialize_CorrectlyDeserializesObject(ConfigNodeSerializer serializer, TestObject testObject, ConfigNode config)
        {
            config.AddValue("FieldOne", "TestValue");
            config.AddValue("FieldTwo", "SecondTestValue");

            var rectConfig = config.AddNode("FieldRect");
            rectConfig.AddValue("x", 25f);
            rectConfig.AddValue("y", 50f);
            rectConfig.AddValue("width", 75f);
            rectConfig.AddValue("height", 100f);

            var persistentData =
                config.AddNode(
                    "ReeperCommonUnitTests.Serialization.Tests.Complex.SerializeObjectWithFieldsWithNativeTests+TestObject:" +
                    NativeSerializer.NativeNodeName);
            persistentData.AddValue("custom", "value");

            serializer.LoadObjectFromConfigNode(ref testObject, config);

            Assert.Equal("TestValue", testObject.FieldOne);
            Assert.Equal("SecondTestValue", testObject.FieldTwo);

            Assert.True(Mathf.Approximately(25f, testObject.FieldRect.x));
            Assert.True(Mathf.Approximately(50f, testObject.FieldRect.y));
            Assert.True(Mathf.Approximately(75f, testObject.FieldRect.width));
            Assert.True(Mathf.Approximately(100f, testObject.FieldRect.height));
        }
    }
}
