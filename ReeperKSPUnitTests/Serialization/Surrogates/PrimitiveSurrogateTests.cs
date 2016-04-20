using System;
using System.ComponentModel;
using System.Linq;
using NSubstitute;
using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Exceptions;
using ReeperKSP.Serialization.Surrogates;
using ReeperKSPUnitTests.Fixtures;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests.Surrogates
{
    public class PrimitiveSurrogateTests
    {
        [Theory, AutoDomainData]
        public void Serialize_With_NullData_ReturnsWithoutThrowing_And_DoesNotMakeAnyChanges(PrimitiveSurrogateSerializer sut, ConfigNode config, string key)
        {
            var nullObj = (object) null;

            Assert.DoesNotThrow(
                () => sut.Serialize(typeof(string), ref nullObj, key, config, Substitute.For<IConfigNodeSerializer>()));

            Assert.False(config.HasData);
        }


        [Theory, AutoDomainData]
        public void Serialize_With_NullParams_ExceptData_Throws(PrimitiveSurrogateSerializer sut, float data, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var dataObj = (object) data;
            Assert.Throws<ArgumentNullException>(() => sut.Serialize(data.GetType(), ref dataObj, null, config, serializer));
            Assert.Throws<ArgumentNullException>(() => sut.Serialize(data.GetType(), ref dataObj, key, null, serializer));
            Assert.Throws<ArgumentNullException>(() => sut.Serialize(data.GetType(), ref dataObj, key, config, null));
        }


        [Theory, AutoDomainData]
        public void Serialize_With_ExistingKey_Throws(PrimitiveSurrogateSerializer sut, float data, string key,
            ConfigNode config, IConfigNodeSerializer serializer)
        {
            config.AddValue(key, data);
            var dataObj = (object)data;

            Assert.Throws<ConfigNodeDuplicateKeyException>(
                () => sut.Serialize(data.GetType(), ref dataObj, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithInvalidData_Throws(PrimitiveSurrogateSerializer sut, float data, string key,
            ConfigNode config, IConfigNodeSerializer serializer)
        {
            config.AddValue(key, "cantconverttofloat");
            var dataObj = (object)data;

            // throw by ConvertFromInvariantString
            Assert.Throws<Exception>(() => sut.Deserialize(data.GetType(), ref dataObj, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_With_NullData_DoesNotThrow(PrimitiveSurrogateSerializer sut, ConfigNode config, string key)
        {
            var dataObj = (object)null;

            Assert.DoesNotThrow(
                () => sut.Deserialize(typeof(string), ref dataObj, key, config, Substitute.For<IConfigNodeSerializer>()));
        }


        [Theory, AutoDomainData]
        public void Deserialize_With_NullParams_ExceptData_Throws(PrimitiveSurrogateSerializer sut, float data, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var objData = (object) data;

            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(data.GetType(), ref objData, null, config, serializer));
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(data.GetType(), ref objData, key, null, serializer));
            Assert.Throws<ArgumentNullException>(() => sut.Deserialize(data.GetType(), ref objData, key, config, null));
        }
    }


    public abstract class PrimitiveSurrogateTests<T>
    {
        private class UnsupportedTestObject
        {
            
        }

        [Theory, AutoDomainData]
        public void Serialize_With_SupportedTypes_AddsSingleKeyToConfigNode(PrimitiveSurrogateSerializer sut, T data, ConfigNode config, string key)
        {
            var dataObj = (object)data;

            sut.Serialize(typeof(T), ref dataObj, key, config, Substitute.For<IConfigNodeSerializer>());

            Assert.True(config.HasData);
            Assert.True(config.HasValue(key));
        }


        [Theory, AutoDomainData]
        public void Deserialize_With_SupportedTypes_ResultsInExpectedValue(PrimitiveSurrogateSerializer sut, T data, ConfigNode config)
        {
            var serializer = Substitute.For<IConfigNodeSerializer>();
            var tc = TypeDescriptor.GetConverter(typeof(T));
            config.AddValue("key", tc.ConvertToInvariantString(data));
            var expected = data;
            var dataObj = (object)default(T);

            sut.Deserialize(typeof(T), ref dataObj, "key", config, serializer);


            Assert.True(tc.CanConvertTo(typeof(string)));
            serializer.DidNotReceive().WriteObjectToConfigNode(ref dataObj, Arg.Any<ConfigNode>());
        }


        [Theory, AutoDomainData]
        public void Serialize_WithUnsupportedType_Throws(PrimitiveSurrogateSerializer sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var data = (object)new UnsupportedTestObject();

            Assert.Throws<WrongSerializerException>(
                () => sut.Serialize(typeof (UnsupportedTestObject), ref data, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Deserialize_WithUnsupportedType_Throws(PrimitiveSurrogateSerializer sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var data = (object)new UnsupportedTestObject();

            Assert.Throws<WrongSerializerException>(
                () => sut.Deserialize(typeof(UnsupportedTestObject), ref data, key, config, serializer));
        }


        [Theory, AutoDomainData]
        public void Serializer_With_SupportedTypes_ThenDeserialize_ReturnsEquivalentValues(PrimitiveSurrogateSerializer sut,
            T data, string key, ConfigNode config)
        {
            var serializer = Substitute.For<IConfigNodeSerializer>();
            var expected = data;
            var objData = (object) data;

            sut.Serialize(typeof (T), ref objData, key, config, serializer);

            Assert.True(config.HasData);
            Assert.True(config.HasValue(key));
            serializer.DidNotReceive().WriteObjectToConfigNode(ref objData, Arg.Any<ConfigNode>());
            serializer.DidNotReceive().CreateConfigNodeFromObject(objData);
            objData = default(T);

            sut.Deserialize(typeof (T), ref objData, key, config, serializer);

            Assert.Equal(expected, (T)objData);
            serializer.DidNotReceive().LoadObjectFromConfigNode(ref objData, Arg.Any<ConfigNode>());
        }


        [Theory, AutoDomainData]
        public void GetSupportedTypes_ReturnsCorrectResults(PrimitiveSurrogateSerializer sut)
        {
            var result = sut.GetSupportedTypes().ToList();

            Assert.NotEmpty(result);
            Assert.Contains(typeof(T), result);
        }
    }


    public class StringSurrogate : PrimitiveSurrogateTests<string>
    {
    }


    public class IntSurrogate : PrimitiveSurrogateTests<int>
    {
    }


    public class FloatSurrogate : PrimitiveSurrogateTests<float>
    {
    }

    public class BooleanSurrogate : PrimitiveSurrogateTests<bool>
    {

    }
}
