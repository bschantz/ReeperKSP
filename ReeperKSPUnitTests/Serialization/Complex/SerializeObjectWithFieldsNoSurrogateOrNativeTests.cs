using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using UnityEngine;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests.Complex
{
    public class SerializeObjectWithFieldsNoSurrogateOrNativeTests
    {
        public class TestObject
        {
            [ReeperPersistent] public Vector2 FieldOne = default(Vector2);
            [ReeperPersistent] public Vector3 FieldTwo = default(Vector3);
        }


        [Theory, AutoDomainData]
        public void Serialize_CorrectlySerializesObject(ConfigNodeSerializer serializer, TestObject testObject)
        {
            var result = serializer.CreateConfigNodeFromObject(testObject);

            Assert.True(result.HasData);
            Assert.True(result.HasValue("FieldOne"));
            Assert.True(result.HasValue("FieldTwo"));
        }


        [Theory, AutoDomainData]
        public void Deserialize_CorrectlyDeserializesObject(ConfigNodeSerializer serializer, TestObject testObject, ConfigNode config)
        {
            config.AddValue("FieldOne", "66,77");
            config.AddValue("FieldTwo", "33,44,55");

            serializer.LoadObjectFromConfigNode(ref testObject, config);

            Assert.True(Mathf.Approximately(66f, testObject.FieldOne.x));
            Assert.True(Mathf.Approximately(77f, testObject.FieldOne.y));

            Assert.True(Mathf.Approximately(33f, testObject.FieldTwo.x));
            Assert.True(Mathf.Approximately(44f, testObject.FieldTwo.y));
            Assert.True(Mathf.Approximately(55f, testObject.FieldTwo.z));
        }
    }
}
