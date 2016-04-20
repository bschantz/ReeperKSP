using System;
using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using ReeperCommon.Containers;
using ReeperKSP.Serialization;
using ReeperKSPUnitTests.TestData;
using UnityEngine;
using Random = System.Random;

namespace ReeperKSPUnitTests.Fixtures
{
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(new Fixture().Customize(new DomainCustomization()))
        {
            var rnd = new Random();

            Fixture.Register(() => new ConfigNode("root"));
            Fixture.Register(() => new SimplePersistentObject());
            Fixture.Register(() => new ComplexPersistentObject());
            Fixture.Register(() => new GetSerializableFields());
            Fixture.Register(() => new Rect(0f, 0f, 100f, 100f));
            Fixture.Register(() => new GetSurrogateSupportedTypes());
            Fixture.Register(() => new NativeSerializer());

            Fixture.Register(() =>
            {
                var serializer = new ConfigNodeSerializer(
                    new SerializerSelectorDecorator(
                        new PreferNativeSerializer(
                            new SerializerSelector(
                                new SurrogateProvider(
                                    new GetSerializationSurrogates(new GetSurrogateSupportedTypes()),
                                    new GetSurrogateSupportedTypes(),
                                    AppDomain.CurrentDomain.GetAssemblies()
                                        .Where(a => a.GetName().Name.StartsWith("ReeperCommon")).ToArray()))),
                    result => Maybe<IConfigNodeItemSerializer>.With(new FieldSerializer(result, new GetSerializableFields()))));

                return serializer;
            });

            Fixture.Register(
                () => new Quaternion((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
        }
    }
    
}
