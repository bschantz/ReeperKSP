using System;
using System.Linq;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    public class CompositeSerializerSelector : ISerializerSelector
    {
        private readonly ISerializerSelector[] _selectors;

        public CompositeSerializerSelector(params ISerializerSelector[] selectors)
        {
            if (selectors == null) throw new ArgumentNullException("selectors");
            _selectors = selectors;

            if (!_selectors.Any())
                throw new ArgumentException("Must supply at least one item");
        }


        public Maybe<IConfigNodeItemSerializer> GetSerializer(Type target)
        {
            foreach (var s in _selectors)
            {
                var serializer = s.GetSerializer(target);

                if (serializer.Any())
                    return serializer;
            }

            return Maybe<IConfigNodeItemSerializer>.None;
        }
    }
}
