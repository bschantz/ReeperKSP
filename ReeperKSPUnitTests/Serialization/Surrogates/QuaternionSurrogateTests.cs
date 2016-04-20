using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Surrogates;
using ReeperKSPUnitTests.Fixtures;
using UnityEngine;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests.Surrogates
{
    public class QuaternionSurrogateTests
    {
        [Theory, AutoDomainData]
        public void Serialize_Test(QuaternionSurrogate sut, Quaternion data, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var obj = (object)data;
            sut.Serialize(typeof(Vector3), ref obj, key, config, serializer);

            Assert.True(config.HasData);
            Assert.True(config.HasValue(key));
            Assert.True(config.GetValue(key) == KSPUtil.WriteQuaternion(data));
        }


        [Theory, AutoDomainData]
        public void Deserialize_Test(QuaternionSurrogate sut, Quaternion data, string key, ConfigNode config,
            IConfigNodeSerializer serializer)
        {
            var deserializedVector = default(Quaternion);
            var deserializedObject = (object)deserializedVector;

            config.AddValue(key, KSPUtil.WriteQuaternion(data));
            sut.Deserialize(typeof(Vector3), ref deserializedObject, key, config, serializer);

            deserializedVector = (Quaternion)deserializedObject;
            Assert.True(Mathf.Approximately(data.x, deserializedVector.x));
            Assert.True(Mathf.Approximately(data.y, deserializedVector.y));
            Assert.True(Mathf.Approximately(data.z, deserializedVector.z));
            Assert.True(Mathf.Approximately(data.w, deserializedVector.w));
        }
    }
}
