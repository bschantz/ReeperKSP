using System;
using System.Linq;
using NSubstitute;
using ReeperCommon.Containers;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public class PreferNativeSerializerTests
    {

        [Fact()]
        public void PreferNativeSerializer_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new PreferNativeSerializer(null));
        }

        [Theory, AutoDomainData]
        public void GetSerializer_ReturnsNativeSerializerForNativeType(PreferNativeSerializer sut)
        {
            var nativeSerializable = Substitute.For<IReeperPersistent>();

            var result = sut.GetSerializer(nativeSerializable.GetType());

            Assert.NotEmpty(result);
            Assert.Same(PreferNativeSerializer.NativeSerializer, result.Single());
        }

        [Theory, AutoDomainData]
        public void GetSerializer_ReturnsDecoratedSerializerResult_ForNonNativeType(ISerializerSelector selector)
        {
            var surrogateSerializer = Substitute.For<IConfigNodeItemSerializer>();
            selector.GetSerializer(Arg.Any<Type>()).Returns(ci => surrogateSerializer.ToMaybe());

            var sut = new PreferNativeSerializer(selector);
            var notNativeSerializable = Substitute.For<IConfigNode>();

            var result = sut.GetSerializer(notNativeSerializable.GetType());

            Assert.NotEmpty(result);
            Assert.NotSame(PreferNativeSerializer.NativeSerializer, result.Single());
            Assert.Same(surrogateSerializer, result.Single());
        }
    }
}
