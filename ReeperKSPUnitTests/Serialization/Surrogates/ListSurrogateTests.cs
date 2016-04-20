using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using ReeperCommon.Containers;
using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Exceptions;
using ReeperKSP.Serialization.Surrogates;
using ReeperKSPUnitTests.Fixtures;
using ReeperKSPUnitTests.Serialization.Complex;
using ReeperKSPUnitTests.Serialization.Surrogates;
using ReeperKSPUnitTests.TestData;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace ReeperKSP.Serialization.Tests.Surrogates
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public abstract class ListSurrogateTests<T>
    {

        [Theory, AutoDomainData]
        public void Serialize_ParameterCheck(ListSurrogate<T> sut, IEnumerable<T> dataList, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var badType = (object) sut;
            var data = new List<T>(dataList);
            var objData = (object) data;
            var defaultList = (object)default(List<T>);

            Assert.Throws<ArgumentNullException>(() => sut.Serialize(null, ref objData, key, config, serializer));
            Assert.Throws<WrongSerializerException>(
                () => sut.Serialize(typeof (ListSurrogateTests<T>), ref objData, key, config, serializer));
            Assert.Throws<WrongSerializerException>(
                () => sut.Serialize(typeof(List<T>), ref badType, key, config, serializer));

            Assert.DoesNotThrow(() => sut.Serialize(typeof(List<T>), ref defaultList, key, config, serializer));
            Assert.Throws<ArgumentNullException>(() => sut.Serialize(typeof (T), ref objData, null, config, serializer));
            Assert.Throws<ArgumentNullException>(
                () => sut.Serialize(typeof (List<T>), ref objData, key, null, serializer));
            Assert.Throws<ArgumentNullException>(() => sut.Serialize(typeof (List<T>), ref objData, key, config, null));
        }
        
        [Theory, AutoDomainData]
        public void Serialize_UsesSerializerSelector_ToChooseSerializer(ListSurrogate<T> sut, IEnumerable<T> listData,
            string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var list = new List<T>(listData);
            serializer.SerializerSelector.GetSerializer(Arg.Any<Type>())
                .Returns(Maybe<IConfigNodeItemSerializer>.With(Substitute.For<IConfigNodeItemSerializer>()));

            var objData = (object)list;
            sut.Serialize(typeof(List<T>), ref objData, key, config, serializer);

            serializer.SerializerSelector.Received().GetSerializer(Arg.Is(typeof (T)));
            serializer.DidNotReceive().CreateConfigNodeFromObject(Arg.Any<object>());
            serializer.DidNotReceive().WriteObjectToConfigNode(ref objData, Arg.Any<ConfigNode>());
        }


        [Theory, AutoDomainData]
        public void Deserialize_UsesSerializerSelector_ToChooseSerializer(ListSurrogate<T> sut, IEnumerable<T> listData,
            string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            var list = new List<T>(listData);
            serializer.SerializerSelector.GetSerializer(Arg.Any<Type>())
                .Returns(Maybe<IConfigNodeItemSerializer>.With(Substitute.For<IConfigNodeItemSerializer>()));

            var objData = (object)list;
            sut.Deserialize(typeof(List<T>), ref objData, key, config, serializer);

            serializer.SerializerSelector.Received().GetSerializer(Arg.Is(typeof(T)));
            serializer.DidNotReceive().LoadObjectFromConfigNode(ref objData, Arg.Any<ConfigNode>());
        }


        [Theory, AutoDomainData]
        public void Serialize_UsingLiveSerializer_CreatesData(ListSurrogate<T> sut, IEnumerable<T> listData, string key, ConfigNode config, ConfigNodeSerializer serializer)
        {
            var list = new List<T>(listData);
            var objList = (object)list;
            sut.Serialize(typeof (List<T>), ref objList, key, config, serializer);

            Assert.True(config.HasData);
            Assert.True(config.HasNode(key));
            Assert.Equal(1, config.CountNodes); // one sub node to contain all items should be created...
            Assert.Equal(list.Count, config.nodes[0].CountNodes); // each item in list should have corresponding item {} ConfigNode
        }


        [Theory, AutoDomainData]
        public void Deserialize_WorksCorrectly(ListSurrogate<T> sut, IEnumerable<T> listData, string key, ConfigNode config, ConfigNodeSerializer serializer)
        {
            var listDataValues = listData.ToList();
            var items = config.AddNode(key);
            var list = new List<T>();
            var objList = (object) list;

            foreach (var item in listDataValues)
            {
                var itemValue = item;
                var itemNode = items.AddNode("item");
                serializer.WriteObjectToConfigNode(ref itemValue, itemNode);
            }

            sut.Deserialize(typeof (List<T>), ref objList, key, config, serializer);
            list = (List<T>) objList;

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            
            if (typeof (T) != typeof (ConfigNode))
            {
                foreach (var originalValue in listDataValues)
                    Assert.True(list.Contains(originalValue));
            }
            else
            {
                var configNodeList = list.Cast<ConfigNode>().ToList();
                var originalValueList = listDataValues.Cast<ConfigNode>().ToList();

                //originalValueList.First().Write("D:\\original.cfg", string.Empty);
                //configNodeList.First().Write("D:\\deserialized.cfg", string.Empty);

                foreach (var originalNode in originalValueList)
                    Assert.True(configNodeList.Any(t => ConfigNodeComparer.Similar(t, originalNode)));
            }
        }



    }


    public class ListSurrogateStringTests : ListSurrogateTests<string>
    {

    }

    public class ListSurrogateIntTests : ListSurrogateTests<int>
    {

    }

    public class ListSurrogateConfigNodeTests : ListSurrogateTests<ConfigNode>
    {

    }

    public class ListSurrogateWithNativeSerializableTypeTests : ListSurrogateTests<NativeSerializableType>
    {

    }

    public class ListSurrogateWithSurrogateSerializableTypeTests : ListSurrogateTests<ComplexPersistentObject>
    {

    }


    public class ListSurrogateLiveTests
    {
        [Fact]
        public void Test_ComplexObject_Serialization()
        {
            var serializer =
                new DefaultConfigNodeSerializer(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.GetName().Name.StartsWith("Reeper"))
                        .ToArray());

            var testObject = new SerializeObjectWithComplexFieldsAndNative();
            var result = serializer.CreateConfigNodeFromObject(testObject);
            
            Assert.True(result.HasData);
            Assert.True(result.HasValue("HelloWorldField"));
            Assert.True(result.HasNode("SimpleConfigNodeField"));
            Assert.True(result.HasNode("FloatListField"));
            Assert.True(result.HasNode("StringListField"));
            Assert.True(result.HasNode("ConfigNodeListField"));
            Assert.True(result.HasNode("InnerPersistentListField"));
            Assert.True(
                result.HasNode("ReeperCommonUnitTests.Serialization.Complex.SerializeObjectWithComplexFieldsAndNative:" + NativeSerializer.NativeNodeName));
        }


        [Fact]
        public void Test_ComplexObject_Deserialization()
        {
            var serializer =
                new DefaultConfigNodeSerializer(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.GetName().Name.StartsWith("Reeper"))
                        .ToArray());

            var testObject = new SerializeObjectWithComplexFieldsAndNative();
            var config = new ConfigNode();

            config.AddValue("HelloWorldField", "Small world");
            config.AddNode("SimpleConfigNodeField").AddNode("SimpleConfigNodeName");
            config.AddNode("FloatListField").AddNode("item").AddValue("System.Single", 13f);
            config.AddNode("StringListField").AddNode("item").AddValue("System.String", "TestString");
            config.AddNode("ConfigNodeListField").AddNode("item").AddNode("ConfigNode").AddNode("ConfigNodeListItem");
            config.AddNode("InnerPersistentListField").AddNode("item").AddValue("SomeStringField", "SomeStringValue");
            config.AddNode("ReeperCommonUnitTests.Serialization.Complex.SerializeObjectWithComplexFieldsAndNative:" + NativeSerializer.NativeNodeName);

            serializer.LoadObjectFromConfigNode(ref testObject, config);

            Assert.True(config.HasValue("HelloWorldField"));
            Assert.Same("Small world", config.GetValue("HelloWorldField"));

            Assert.Same("SimpleConfigNodeName", testObject.SimpleConfigNodeField.name);
            Assert.NotEmpty(testObject.FloatListField);
            Assert.Contains(13f, testObject.FloatListField);
            Assert.Equal(1, testObject.FloatListField.Count);

            Assert.NotEmpty(testObject.StringListField);
            Assert.Contains("TestString", testObject.StringListField);
            Assert.Equal(1, testObject.StringListField.Count);

            Assert.NotEmpty(testObject.ConfigNodeListField);
            Assert.Contains("ConfigNodeListItem", testObject.ConfigNodeListField.Select(c => c.name));
            Assert.Equal(1, testObject.ConfigNodeListField.Count);

            Assert.NotEmpty(testObject.InnerPersistentListField);
            Assert.Equal("SomeStringValue", testObject.InnerPersistentListField.Single().SomeStringField);
        }
    }
}
