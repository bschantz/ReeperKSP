using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using UnityEngine;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests.Surrogates
{
    public class RectSurrogateTests
    {
        [Theory, AutoDomainData]
        public void Serialize_Test(ConfigNodeSerializer serializer, Rect data)
        {
            var result = serializer.CreateConfigNodeFromObject(data);

            Assert.True(result.HasData);
            Assert.True(result.CountNodes > 0);
            Assert.True(result.nodes[0].HasData);
        }


        [Theory, AutoDomainData]
        public void Deserialize_Test(ConfigNodeSerializer serializer, Rect data, string key,
            ConfigNode config)
        {
            var n = config.AddNode(typeof (Rect).FullName);
            n.AddValue("x", 11f);
            n.AddValue("y", 22f);
            n.AddValue("width", 33f);
            n.AddValue("height", 44f);

            serializer.LoadObjectFromConfigNode(ref data, config);

            Assert.True(Mathf.Approximately(data.x, 11f));
            Assert.True(Mathf.Approximately(data.y, 22f));
            Assert.True(Mathf.Approximately(data.width, 33f));
            Assert.True(Mathf.Approximately(data.height, 44f));
        }
    }
}
