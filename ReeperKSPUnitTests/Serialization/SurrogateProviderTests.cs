using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute;
using ReeperKSP.Serialization;
using Xunit;

// ReSharper disable once CheckNamespace
namespace ReeperKSP.Serialization.Tests
{
    public class SurrogateProviderTests
    {
        [Fact()]
        public void SurrogateProvider_ConstructorNullParams_Test()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    new SurrogateProvider(null, Substitute.For<IGetSurrogateSupportedTypes>(),
                        Substitute.For<IEnumerable<Assembly>>()));

            Assert.Throws<ArgumentNullException>(
                () =>
                    new SurrogateProvider(Substitute.For<IGetSerializationSurrogates>(), null,
                        Substitute.For<IEnumerable<Assembly>>()));

            Assert.Throws<ArgumentNullException>(
                () =>
                    new SurrogateProvider(Substitute.For<IGetSerializationSurrogates>(),
                        Substitute.For<IGetSurrogateSupportedTypes>(), null));

        }



    }

    public abstract class SurrogateProviderTypeTest<T>
    {
        [Fact]
        public void Get_WithGivenType_ContainsAResult_Test()
        {
            var sut = new SurrogateProvider(new GetSerializationSurrogates(new GetSurrogateSupportedTypes()),
                new GetSurrogateSupportedTypes(), new[] {typeof (IConfigNodeSerializer).Assembly});

            var result = sut.Get(typeof (T));

            Assert.NotEmpty(result);
        }
    }




    public class SurogateProviderStringTypeTest : SurrogateProviderTypeTest<string>
    {
    }

    public class SurogateProviderFloatTypeTest : SurrogateProviderTypeTest<float>
    {
    }

    public class SurogateProviderConfigNodeTypeTest : SurrogateProviderTypeTest<ConfigNode>
    {
    }
}
