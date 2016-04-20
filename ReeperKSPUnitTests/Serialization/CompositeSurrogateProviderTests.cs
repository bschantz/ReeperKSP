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
    public class CompositeSurrogateProviderTests
    {
        [Fact()]
        public void CompositeSurrogateProvider_ThrowsOnNullParameter_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeSurrogateProvider(null));
        }


        [Theory, AutoDomainData]
        public void Get_Returns_First_Useful_ResultTest(ISurrogateProvider first, ISurrogateProvider second, IConfigNodeItemSerializer itemSerializer)
        {
            var arg = typeof (Type);
            first.Get(Arg.Is(arg)).Returns(Maybe<IConfigNodeItemSerializer>.Nothing);
            second.Get(Arg.Is(arg)).Returns(Maybe<IConfigNodeItemSerializer>.With(itemSerializer));

            var sut = new CompositeSurrogateProvider(first, second);

            var result = sut.Get(arg);

            first.Received(1).Get(Arg.Is(arg));
            second.Received(1).Get(Arg.Is(arg));
            Assert.NotEmpty(result);
            Assert.Same(itemSerializer, result.Single());
        }


        [Theory, AutoDomainData]
        public void Get_WhenAllReturnNothing_ReturnsNothing(ISurrogateProvider first, ISurrogateProvider second)
        {
            var arg = typeof(Type);
            first.Get(Arg.Is(arg)).Returns(Maybe<IConfigNodeItemSerializer>.Nothing);
            second.Get(Arg.Is(arg)).Returns(Maybe<IConfigNodeItemSerializer>.Nothing);

            var sut = new CompositeSurrogateProvider(first, second);

            var result = sut.Get(arg);

            first.Received(1).Get(Arg.Is(arg));
            second.Received(1).Get(Arg.Is(arg));
            Assert.Empty(result);
        }
    }
}
