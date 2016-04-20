using System.Linq;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using ReeperKSPUnitTests.TestData;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public abstract class GetSerializableFieldsTests<T>
    {
        public class ObjectWithPersistentFields
        {
            [ReeperPersistent] public T PublicField;
            public T PublicFieldNotSerialized;

            [ReeperPersistent] private T PrivateField;
            private T PrivateFieldNotSerialized;
        }


        [Theory, AutoDomainData]
        public void Get_WithNullObjectReturnsNothing_Test(GetSerializableFields sut)
        {
            Assert.DoesNotThrow(() => sut.Get(null));
            var results = sut.Get(null);

            Assert.Empty(results);
        }


        [Theory, AutoDomainData]
        public void GetTest(GetSerializableFields sut, ObjectWithPersistentFields obj)
        {
            var result = sut.Get(obj).ToList();
            var fieldNames = result.Select(r => r.Name).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains("PublicField", fieldNames);
            Assert.Contains("PrivateField", fieldNames);
            Assert.DoesNotContain("PublicFieldNotSerialized", fieldNames);
            Assert.DoesNotContain("PrivateFieldNotSerialized", fieldNames);
        }
    }

    public class GetSerializableFieldsWithString : GetSerializableFieldsTests<string>
    {
    }

    public class GetSerializableFieldsWithInt : GetSerializableFieldsTests<int>
    {  
    }

    public class GetSerializableFieldsWithSimple : GetSerializableFieldsTests<SimplePersistentObject>
    {
    }

    public class GetSerializableFieldsWithConfigNode : GetSerializableFieldsTests<ConfigNode>
    {
    }
}
