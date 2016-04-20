//using System;
//using System.Linq;
//using NSubstitute;
//using ReeperCommon.Serialization;
//using ReeperCommonUnitTests.Fixtures;
//using Xunit;
//using Xunit.Extensions;

//namespace ReeperCommonUnitTests.Serialization
//{
//    public class ReeperPersistentMethodCallerTests
//    {
//        [Fact()]
//        public void ReeperPersistent_Constructor_ThrowsOnNull()
//        {
//            Assert.Throws<ArgumentNullException>(() => new ReeperPersistentMethodCaller(null));
//        }


//        [Theory, AutoDomainData]
//        public void Serialize_DoesNotCallIPersistenceSave_OnNull(ReeperPersistentMethodCaller sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
//        {
//            var target = Substitute.For<IPersistenceSave>();

//            sut.Serialize(target.GetType(), null, key, config, serializer);

//            target.DidNotReceive().PersistenceSave();
//        }


//        [Theory, AutoDomainData]
//        public void Serialize_CallsIReeperPersistenceSave_OnNotNull(ReeperPersistentMethodCaller sut, string key, ConfigNode config,
//            IConfigNodeSerializer serializer)
//        {
//            var target = Substitute.For<IPersistenceSave>();

//            sut.Serialize(target.GetType(), target, key, config, serializer);

//            target.Received(1).PersistenceSave();
//        }


//        [Theory, AutoDomainData]
//        public void Deserialize_DoesNotCallIPersistenceLoad_OnNull(ReeperPersistentMethodCaller sut, string key, ConfigNode config, IConfigNodeSerializer serializer)
//        {
//            var target = Substitute.For<IPersistenceLoad>();

//            sut.Deserialize(target.GetType(), null, key, config, serializer);

//            target.DidNotReceive().PersistenceLoad();
//        }


//        [Theory, AutoDomainData]
//        public void Deserialize_CallsIPersistenceLoad_OnNotNull(string key, ConfigNode config,
//            IConfigNodeSerializer serializer)
//        {
//            var target = Substitute.For<IPersistenceLoad>();
//            var itemSerializer = Substitute.For<IConfigNodeItemSerializer>();
//            itemSerializer.Deserialize(Arg.Any<Type>(), Arg.Any<object>(), Arg.Any<string>(), Arg.Any<ConfigNode>(),
//                Arg.Any<IConfigNodeSerializer>())
//                .Returns(ci => target);

//            var sut = new ReeperPersistentMethodCaller(itemSerializer);
            

//            sut.Deserialize(target.GetType(), target, key, config, serializer);

//            target.Received(1).PersistenceLoad();
//        }
//    }
//}
