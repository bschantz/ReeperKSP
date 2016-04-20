using System;
using System.Linq;
using ReeperKSP.Gui.Window;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization.Surrogates
{
    public class WindowComponentSurrogateAdapter : IConfigNodeItemSerializer<IWindowComponent>
    {
        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (target == null) return;

            var targetType = target.GetType();

            var typeSerializer = serializer.SerializerSelector.GetSerializer(targetType);

            if (!typeSerializer.Any())
                throw new NoSerializerFoundException(targetType); // presumably all any window component passed to the serializer
                                                                  // should serialize so make some noise if we can't

            typeSerializer.Single().Serialize(targetType, ref target, key, config, serializer);
        }


       
        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (target == null) return;

            var targetType = target.GetType();

            var typeSerializer = serializer.SerializerSelector.GetSerializer(targetType);

            if (!typeSerializer.Any())
                throw new NoSerializerFoundException(targetType); // presumably all any window component passed to the serializer
                                                        // should serialize so make some noise if we can'

            typeSerializer.Single().Deserialize(targetType, ref target, key, config, serializer);
        }
    }
}
