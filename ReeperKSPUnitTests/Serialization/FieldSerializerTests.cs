using System;
using System.Reflection;
using NSubstitute;
using ReeperCommon.Containers;
using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Exceptions;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public class FieldSerializerTests
    {
        public class TestObject
        {
            [ReeperPersistent] public float FloatField;
            [ReeperPersistent] public string StringField;
        }

        public class TestObjectWithFieldThatHasNoSerializer
        {
            [ReeperPersistent] public TestObject FieldWithoutSerializer = new TestObject();
        }


        [Fact]
        public void FieldSerializer_Constructor_ThrowsOnNullParameters_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new FieldSerializer(Maybe<IConfigNodeItemSerializer>.None, null));
        }


        [Theory, AutoDomainData]
        public void Serialize_UsesSerializerForEachPersistentField_Test(
            IConfigNodeItemSerializer decoratedSerializer, 
            TestObject testObject, 
            IConfigNodeSerializer serializer,
            string key, 
            ConfigNode config)
        {
            var obj = (object)testObject;
            var fields = typeof (TestObject).GetFields(BindingFlags.Instance | BindingFlags.Public);
            var fieldQuery = Substitute.For<IGetObjectFields>();
            fieldQuery.Get(Arg.Is(testObject)).Returns(fields);

            serializer.SerializerSelector.GetSerializer(Arg.Any<Type>())
                .Returns(Substitute.For<IConfigNodeItemSerializer>().ToMaybe());

            var sut = new FieldSerializer(decoratedSerializer.ToMaybe(), fieldQuery);

            sut.Serialize(typeof (TestObject), ref obj, key, config, serializer);

            decoratedSerializer.Received()
                .Serialize(Arg.Is(typeof (TestObject)), ref obj, Arg.Is(key), Arg.Any<ConfigNode>(), Arg.Is(serializer));
            serializer.SerializerSelector.Received().GetSerializer(Arg.Is(typeof (float)));
            serializer.SerializerSelector.Received().GetSerializer(Arg.Is(typeof (string)));
        }


        [Theory, AutoDomainData]
        public void Deserialize_UsesSerializerForEachPersistentField_Test(
            IConfigNodeItemSerializer decoratedSerializer,
            TestObject testObject,
            IConfigNodeSerializer serializer,
            string key,
            ConfigNode config)
        {
            var obj = (object)testObject;
            var fields = typeof(TestObject).GetFields(BindingFlags.Instance | BindingFlags.Public);
            var fieldQuery = Substitute.For<IGetObjectFields>();
            fieldQuery.Get(Arg.Is(testObject)).Returns(fields);

            serializer.SerializerSelector.GetSerializer(Arg.Any<Type>())
                .Returns(Substitute.For<IConfigNodeItemSerializer>().ToMaybe());

            var sut = new FieldSerializer(decoratedSerializer.ToMaybe(), fieldQuery);

            sut.Deserialize(typeof(TestObject), ref obj, key, config, serializer);

            decoratedSerializer.Received()
                .Deserialize(Arg.Is(typeof(TestObject)), ref obj, Arg.Is(key), Arg.Any<ConfigNode>(), Arg.Is(serializer));
            serializer.SerializerSelector.Received().GetSerializer(Arg.Is(typeof(float)));
            serializer.SerializerSelector.Received().GetSerializer(Arg.Is(typeof(string)));
        }


        [Theory, AutoDomainData]
        public void ThrowsException_IfReeperPersistentField_HasNoSerializer(string key, ConfigNode config)
        {
            var serializer = Substitute.For<IConfigNodeSerializer>();
            var selector = Substitute.For<ISerializerSelector>();
            var testObject = new TestObjectWithFieldThatHasNoSerializer();
            var fieldQuery = Substitute.For<IGetObjectFields>();
            var objTestObject = (object) testObject;

            fieldQuery.Get(
                Arg.Any<object>())
                    .Returns(
                        typeof (TestObjectWithFieldThatHasNoSerializer).GetFields(BindingFlags.Public |
                                                                                  BindingFlags.Instance));

            selector.GetSerializer(Arg.Any<Type>()).Returns(Maybe<IConfigNodeItemSerializer>.None);
            serializer.SerializerSelector.Returns(selector);

            var sut = new FieldSerializer(Maybe<IConfigNodeItemSerializer>.None, fieldQuery);

            Assert.Throws<NoSerializerFoundException>(
                () => sut.Serialize(typeof (TestObjectWithFieldThatHasNoSerializer), ref objTestObject, key, config, serializer));
            Assert.Throws<NoSerializerFoundException>(
                () =>
                    sut.Deserialize(typeof (TestObjectWithFieldThatHasNoSerializer), ref objTestObject, key, config,
                        serializer));
        }
    }
}
