using System;
using NSubstitute;
using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Exceptions;
using ReeperKSPUnitTests.Fixtures;
using ReeperKSPUnitTests.TestData;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public class NativeSerializerTests
    {
        [Theory, AutoDomainData]
        public void Serialize_WithNullParameters_Throws(string key, ConfigNode config)
        {
            object testObject = Substitute.For<IReeperPersistent>();
            var serializer = Substitute.For<IConfigNodeSerializer>();
            var sut = new NativeSerializer();

            Assert.Throws<ArgumentNullException>(() => sut.Serialize(null, ref testObject, key, config, serializer));
            Assert.DoesNotThrow(() => sut.Serialize(testObject.GetType(), ref testObject, key, config, serializer)); // null target shouldn't throw
            Assert.Throws<ArgumentNullException>(
                () => sut.Serialize(testObject.GetType(), ref testObject, null, config, serializer));
            Assert.Throws<ArgumentNullException>(
                () => sut.Serialize(testObject.GetType(), ref testObject, key, null, serializer));
            Assert.Throws<ArgumentNullException>(
                () => sut.Serialize(testObject.GetType(), ref testObject, key, config, null));
        }


        [Theory, AutoDomainData]
        public void Serialize_WithTargetTypeThatDoesNotMatchSpecifiedType_Throws(string key, ConfigNode config,
            IConfigNodeSerializer serializer)
        {
            var testObject = Substitute.For<IReeperPersistent>();
            var sut = new NativeSerializer();
            var wrong = (object)key;

            Assert.Throws<WrongNativeSerializerException>(
                () => sut.Serialize(testObject.GetType(), ref wrong, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Serialize_CallsIReeperPersistent_Serialize_WithNewNode(NativeSerializer sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var testObject = Substitute.For<IReeperPersistent>();
            var objTestObject = (object) testObject;
            sut.Serialize(testObject.GetType(), ref objTestObject, key, config, serializer);

            testObject.Received(1).DuringSerialize(Arg.Is(serializer), Arg.Is<ConfigNode>(cfg => !ReferenceEquals(cfg, config) && cfg.name == key + ":" + NativeSerializer.NativeNodeName));
        }


        [Theory, AutoDomainData]
        public void Serialize_WithTypeThatIsNotIReeperPersistent_Throws(NativeSerializer sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var testObject = Substitute.For<IPersistenceSave>();
            var objTestObject = (object)testObject;

            Assert.Throws<ArgumentException>(() => sut.Serialize(testObject.GetType(), ref objTestObject, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_CallsIReeperPersistent_Deserialize_WithNewNode(NativeSerializer sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var testObject = Substitute.For<IReeperPersistent>();
            var objTestObject = (object)testObject;

            config.AddNode(key + ":" + NativeSerializer.NativeNodeName);

            sut.Deserialize(testObject.GetType(), ref objTestObject, key, config, serializer);

            testObject.Received(1).DuringDeserialize(Arg.Is(serializer), Arg.Is<ConfigNode>(cfg => !ReferenceEquals(cfg, config) && cfg.name == key + ":" + NativeSerializer.NativeNodeName));
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithNoConfigValue_ReturnsUnchangedTarget(NativeSerializer sut, string key,
            ConfigNode config, IConfigNodeSerializer serializer)
        {
            var expected = Substitute.For<IReeperPersistent>();
            var copy = expected;
            var objExpected = (object)expected;
            config.AddNode(key + ":" + NativeSerializer.NativeNodeName);

            sut.Deserialize(expected.GetType(), ref objExpected, key, config, serializer);

            Assert.Same(expected, copy);
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithNullTarget_WithReferenceType_ThrowsWhenNoDefaultConstructor(NativeSerializer sut,
            string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var expectedType = Substitute.For<IReeperPersistent>();
            var objNull = (object) null;

            config.AddNode(key);

            Assert.Throws<NoDefaultValueException>(() => sut.Deserialize(expectedType.GetType(), ref objNull, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithNullTarget_WithDifferentType_Throws(NativeSerializer sut,
            string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var expectedType = Substitute.For<IReeperPersistent>();
            var badType = Substitute.For<IPersistenceLoad>();
            var objBadType = (object) badType;

            config.AddNode(key + ":" + NativeSerializer.NativeNodeName);


            Assert.Throws<WrongNativeSerializerException>(
                // ReSharper disable once ExpressionIsAlwaysNull
                () => sut.Deserialize(expectedType.GetType(), ref objBadType, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithTypeThatIsNotIReeperPersistent_Throws(NativeSerializer sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var testObject = Substitute.For<IConfigNode>();
            var objTestObject = (object) testObject;

            config.AddNode(key + ":" + NativeSerializer.NativeNodeName);

            Assert.Throws<ArgumentException>(() => sut.Deserialize(testObject.GetType(), ref objTestObject, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithNullReferenceType_CreatesNewInstance(NativeSerializer sut, string key,
            ConfigNode config, IConfigNodeSerializer serializer)
        {
            var nullInstance = (object) (DefaultConstructableType) null;
            config.AddNode(key + ":" + NativeSerializer.NativeNodeName);

            sut.Deserialize(typeof(DefaultConstructableType), ref nullInstance, key, config, serializer);

            Assert.NotNull(nullInstance);
            Assert.Same(nullInstance.GetType(), typeof(DefaultConstructableType));
            Assert.True(nullInstance is IReeperPersistent);
        }



    }
}
