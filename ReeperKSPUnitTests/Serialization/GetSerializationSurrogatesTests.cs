using System;
using System.Linq;
using ReeperKSP.Serialization;
using ReeperKSP.Serialization.Surrogates;
using Xunit;

// ReSharper disable once CheckNamespace
namespace ReeperCommon.Serialization.Tests
{
    public class GetSerializationSurrogatesTests
    {
        [Fact()]
        public void Get_WithNullArg_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new GetSerializationSurrogates(null));
        }


        [Fact()]
        public void Get_WithReeperCommonAssembly_LocatesSerializationSurrogates()
        {
            var sut = new GetSerializationSurrogates(new GetSurrogateSupportedTypes());

            var results = sut.Get(typeof(IGetSerializationSurrogates).Assembly).ToList();

            Assert.NotEmpty(results);
            Assert.Contains(typeof (ListSurrogate<>), results);
        }
    }
}
