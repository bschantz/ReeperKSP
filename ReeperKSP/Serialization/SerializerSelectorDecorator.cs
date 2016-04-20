using System;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    public class SerializerSelectorDecorator : ISerializerSelector
    {
        private readonly ISerializerSelector _decorated;
        private readonly Func<Maybe<IConfigNodeItemSerializer>, Maybe<IConfigNodeItemSerializer>> _decorator;

        public SerializerSelectorDecorator(
            ISerializerSelector decorated, 
            Func<Maybe<IConfigNodeItemSerializer>, Maybe<IConfigNodeItemSerializer>> decorator)
        {
            if (decorated == null) throw new ArgumentNullException("decorated");
            if (decorator == null) throw new ArgumentNullException("decorator");
            _decorated = decorated;
            _decorator = decorator;
        }


        public Maybe<IConfigNodeItemSerializer> GetSerializer(Type target)
        {
            return _decorator(_decorated.GetSerializer(target));
        }
    }
}
