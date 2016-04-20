using System.Linq;
using Ploeh.AutoFixture.Xunit;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.Fixtures;
using ReeperKSPUnitTests.TestData;
using Xunit;
using Xunit.Extensions;

namespace ReeperKSPUnitTests.Serialization
{
    public class SerializableFieldQueryTests
    {
        [Theory, AutoDomainData]
        public void Get_WithSimpleObject_ReturnsTheSingleSerializableField([Frozen] SimplePersistentObject targetObject, GetSerializableFields sut)
        {
            var actual = sut.Get(targetObject).ToList();

            Assert.NotEmpty(actual);
            Assert.Single(actual); // simple object should only have one persistent field
        }


        [Theory, AutoDomainData]
        public void Get_WithComplexObject_ReturnsAllSerializableFields([Frozen] ComplexPersistentObject targetObject,
            GetSerializableFields sut)
        {
            var actual = sut.Get(targetObject).ToList();

            Assert.NotEmpty(actual);
            Assert.True(actual.Count == 6); 
        }
    }
}
