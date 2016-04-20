using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Surrogates;
using ReeperKSPUnitTests.Fixtures;
using UnityEngine;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests.Surrogates
{
    public class Vector2SurrogateTests
    {
        [Theory, AutoDomainData]
        public void Serialize_Test(Vector2Surrogate sut, Vector2 data, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var obj = (object) data;
            sut.Serialize(typeof (Vector2), ref obj, key, config, serializer);

            Assert.True(config.HasData);
            Assert.True(config.HasValue(key));
            Assert.True(config.GetValue(key) == KSPUtil.WriteVector(data));
        }


        [Theory, AutoDomainData]
        public void Deserialize_Test(Vector2Surrogate sut, Vector2 data, string key, ConfigNode config,
            IConfigNodeSerializer serializer)
        {
            var deserializedVector = default(Vector2);
            var deserializedObject = (object) deserializedVector;

            config.AddValue(key, KSPUtil.WriteVector(data));
            sut.Deserialize(typeof(Vector2), ref deserializedObject, key, config, serializer);

            deserializedVector = (Vector2) deserializedObject;
            Assert.Equal(data, deserializedVector);
        }
    }
}
