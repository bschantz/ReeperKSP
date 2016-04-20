using System;
using System.Linq;
using ReeperCommon.Containers;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization
{
    public class FieldSerializer : IConfigNodeItemSerializer
    {
        private readonly Maybe<IConfigNodeItemSerializer> _decorated;
        private readonly IGetObjectFields _fields;

        public FieldSerializer(Maybe<IConfigNodeItemSerializer> decorated, IGetObjectFields fields)
        {
            if (fields == null) throw new ArgumentNullException("fields");

            _decorated = decorated;
            _fields = fields;
        }


        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");

            if (target != null)
                foreach (var field in _fields.Get(target))
                {
                    var fieldSerializer = serializer.SerializerSelector.GetSerializer(field.FieldType);
                    if (!fieldSerializer.Any()) throw new NoSerializerFoundException(field.FieldType);

                    var value = field.GetValue(target);

                    fieldSerializer.Single().Serialize(field.FieldType, ref value, field.Name, config, serializer);
                }

            if (_decorated.Any())
                _decorated.Single().Serialize(type, ref target, key, config, serializer);
        }


        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");

            // deserialize first, in case the target is null (or is going to be null)
            if (_decorated.Any())
                _decorated.Single().Deserialize(type, ref target, key, config, serializer);

            if (target != null)
                foreach (var field in _fields.Get(target))
                {
                    var fieldSerializer = serializer.SerializerSelector.GetSerializer(field.FieldType);
                    if (!fieldSerializer.Any()) throw new NoSerializerFoundException(field.FieldType);

                    var value = field.GetValue(target);

                    fieldSerializer.Single().Deserialize(field.FieldType, ref value, field.Name, config, serializer);
                    field.SetValue(target, value);
                }
        }
    }
}
