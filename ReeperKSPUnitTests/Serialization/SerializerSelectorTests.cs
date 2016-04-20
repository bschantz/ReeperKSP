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
    public class SerializerSelectorTests
    {
        [Fact()]
        public void SerializerSelector_ConstructorThrowsOnNull_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new SerializerSelector(null));
        }

        [Theory, AutoDomainData]
        public void GetSerializer_UsesSurrogate_ForSelection_Test(ISurrogateProvider surrogateProvider)
        {
            var surrogate = Substitute.For<IConfigNodeItemSerializer>();
            surrogateProvider.Get(Arg.Any<Type>()).Returns(surrogate.ToMaybe());

            var sut = new SerializerSelector(surrogateProvider);

            var result = sut.GetSerializer(typeof (string));

            Assert.NotEmpty(result);
            Assert.Same(surrogate, result.Single());

            surrogateProvider.Received(1).Get(Arg.Is(typeof (string)));
        }
    }
}
