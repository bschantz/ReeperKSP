//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Ploeh.AutoFixture;
//using Ploeh.AutoFixture.Xunit;
//using ReeperCommon.Containers;
//using ReeperCommon.Serialization;
//using ReeperCommon.Serialization.Surrogates;
//using ReeperCommonUnitTests.Fixtures;
//using Xunit;
//using Xunit.Extensions;

//namespace ReeperCommonUnitTests.Serialization
//{
//    public abstract class ValueTypeSerializationTests<TValueType> where TValueType : struct
//    {

//        [Theory, AutoDomainData]
//        public void Serialize_WithValueType_Passes(PrimitiveSurrogateSerializer sut, bool data, string key)
//        {

//        }

//        [Theory, AutoDomainData]
//        public void WhatGoesIn_MustComeOut(IConfigNodeSerializer serializer, [Frozen] string key, IEnumerable<TValueType> values, ConfigNode config)
//        {
//            foreach (var v in values)
//            {
//                var startingValue = v;
//                var defaultValue = default(TValueType);

                

//                var itemSerializer = serializer.ConfigNodeItemSerializerSelector.GetSerializer(typeof (TValueType));
//                itemSerializer.Do(s => s.Serialize(typeof (TValueType), startingValue, key, config, serializer));

//                Assert.NotEmpty(itemSerializer);
//                Assert.True(config.HasData);

//                var finalValue =
//                    itemSerializer.SingleOrDefault().Return<IConfigNodeItemSerializer, object>(
//                        s => s.Deserialize(typeof(TValueType), defaultValue, key, config, serializer),
//                        default(TValueType));

//                Assert.Equal(startingValue, finalValue);
//                config.ClearData();
//            }
//        }
//    }


//    public class ValueTypeBoolTest : ValueTypeSerializationTests<bool>
//    {
        
//    }

//    public class ValueTypeFloatTest : ValueTypeSerializationTests<float>
//    {

//    }

//    public class ValueTypeIntTest : ValueTypeSerializationTests<int>
//    {

//    }
//}
