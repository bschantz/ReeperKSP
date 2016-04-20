using System;
using NSubstitute;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public class PersistenceMethodCallerTests
    {
        [Fact]
        public void PersistenceMethodCaller_ConstructorThrowsOnNullParameter_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new PersistenceMethodCaller(null));
        }


        [Theory, AutoDomainData]
        public void Serialize_WithIPersistenceSave_MethodCalled_Test(PersistenceMethodCaller sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var persistentSaveable = Substitute.For<IPersistenceSave>();
            var testObject = (object) persistentSaveable;

            sut.Serialize(testObject.GetType(), ref testObject, key, config, serializer);

            persistentSaveable.Received(1).PersistenceSave();
        }

        [Theory, AutoDomainData]
        public void Deserialize_WithIPersistenceLoad_MethodCalled_Test(PersistenceMethodCaller sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var persistentLoadable = Substitute.For<IPersistenceLoad>();
            var testObject = (object)persistentLoadable;

            sut.Deserialize(testObject.GetType(), ref testObject, key, config, serializer);

            persistentLoadable.Received(1).PersistenceLoad();
        }
    }
}
