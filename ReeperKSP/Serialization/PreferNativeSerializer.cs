using System;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    /// <summary>
    /// Will use native serialization (IReeperPersistent methods) over anything the decorated
    /// selector will return
    /// </summary>
    public class PreferNativeSerializer : ISerializerSelector
    {
        private readonly ISerializerSelector _decorated;
        public static readonly IConfigNodeItemSerializer NativeSerializer = new NativeSerializer();

        public PreferNativeSerializer(ISerializerSelector decorated)
        {
            if (decorated == null) throw new ArgumentNullException("decorated");

            _decorated = decorated;
        }


        public Maybe<IConfigNodeItemSerializer> GetSerializer(Type target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (!target.IsAbstract && typeof (IReeperPersistent).IsAssignableFrom(target))
                return NativeSerializer.ToMaybe();

            return _decorated.GetSerializer(target);
        }
    }
}
