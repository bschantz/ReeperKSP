using ReeperKSP.Serialization.Surrogates;
using ReeperKSPUnitTests.Fixtures;
using UnityEngine;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperKSP.Serialization.Tests.Surrogates
{
    public class ColorSurrogateTests
    {
        [Theory, AutoDomainData]
        public void Serialize_Test(ColorSurrogate sut, Color data, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var obj = (object)data;
            sut.Serialize(typeof(Vector2), ref obj, key, config, serializer);

            Assert.True(config.HasData);
            Assert.True(config.HasValue(key));
            Assert.True(config.GetValue(key) == KSPUtil.WriteVector(data));
        }


        [Theory, AutoDomainData]
        public void Deserialize_Test(ColorSurrogate sut, Color data, string key, ConfigNode config,
            IConfigNodeSerializer serializer)
        {
            var deserializedVector = default(Color);
            var deserializedObject = (object)deserializedVector;

            config.AddValue(key, KSPUtil.WriteVector(data));
            sut.Deserialize(typeof(Color), ref deserializedObject, key, config, serializer);

            deserializedVector = (Color)deserializedObject;
            Assert.Equal(data, deserializedVector);
        }
    }
}
