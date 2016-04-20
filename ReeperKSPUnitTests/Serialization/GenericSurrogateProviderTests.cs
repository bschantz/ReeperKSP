using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public class GenericSurrogateProviderTests
    {
        [Fact()]
        public void GenericSurrogateProvider_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(
                () => new GenericSurrogateProvider(null, Substitute.For<IGetSurrogateSupportedTypes>(), Substitute.For<IEnumerable<Assembly>>()));

            Assert.Throws<ArgumentNullException>(
                () =>
                    new GenericSurrogateProvider(Substitute.For<IGetSerializationSurrogates>(), null,
                        Substitute.For<IEnumerable<Assembly>>()));

            Assert.Throws<ArgumentNullException>(
                () => new GenericSurrogateProvider(Substitute.For<IGetSerializationSurrogates>(), Substitute.For<IGetSurrogateSupportedTypes>(), null));
        }


        [Theory, AutoDomainData]
        public void Get_ReturnsNothing_WhenGivenGenericType_Test(GenericSurrogateProvider sut)
        {
            var result = sut.Get(typeof (List<>));

            Assert.Empty(result);
        }

        [Theory, AutoDomainData]
        public void Get_ThrowsWhenGivenNullType_Test(GenericSurrogateProvider sut)
        {
            Assert.Throws<ArgumentNullException>(() => sut.Get(null));
        }


        [Theory, AutoDomainData]
        public void Get_ReturnsNone_WhenGivenNongenericType_Test(GenericSurrogateProvider sut)
        {
            var result = sut.Get(typeof (string));

            Assert.Empty(result);
        }
    }


    public abstract class GenericSurrogateProvider_ForList_Tests<T>
    {
        [Fact]
        public void GetTest()
        {
            var sut = new GenericSurrogateProvider(
                new GetSerializationSurrogates(new GetSurrogateSupportedTypes()),
                new GetSurrogateSupportedTypes(),
                AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("ReeperCommon")));

            var result = sut.Get(typeof(List<T>));

            Assert.NotEmpty(result);
        }
    }

    public class ListConfigNode : GenericSurrogateProvider_ForList_Tests<ConfigNode>
    {
    }

    public class ListString : GenericSurrogateProvider_ForList_Tests<string>
    {
    }

    public class ListFloat : GenericSurrogateProvider_ForList_Tests<float>
    {
    }
}
