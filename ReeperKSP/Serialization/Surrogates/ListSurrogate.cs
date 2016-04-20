using System;
using System.Collections.Generic;
using System.Linq;
using ReeperCommon.Containers;
using ReeperKSP.Serialization.Exceptions;

namespace ReeperKSP.Serialization.Surrogates
{
    /// <summary>
    /// Note to self: ListSurrogate's item type MUST be serializable so we'll be using the item serializer selector.
    /// This is because any item that doesn't have a serializer (such as a basic object with [ReeperPersistent] fields)
    /// won't be fully initializable... a problem when we'll be creating them
    /// </summary>
    /// <typeparam name="TListItemType"></typeparam>
    public class ListSurrogate<TListItemType> : IConfigNodeItemSerializer<List<TListItemType>>
    {
        private const string ListItemNodeName = "item";

        public void Serialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", "key");

            if (!typeof(List<TListItemType>).IsAssignableFrom(type))
                throw new WrongSerializerException(target != null ? target.GetType() : type, typeof(List<TListItemType>));

            if (config.HasNode(key))
                throw new ConfigNodeDuplicateKeyException(key, config);

            var list = target as List<TListItemType>;
            if (list == null)
                if (target == null)
                    return;
                else throw new WrongSerializerException(target.GetType(), typeof (List<TListItemType>));

            var itemSerializer = serializer.SerializerSelector.GetSerializer(typeof (TListItemType));

            if (!itemSerializer.Any())
                throw new NoSerializerFoundException(typeof(TListItemType));

            var scopeNode = config.AddNode(key);

            foreach (var item in list)
            {
                var objItem = (object) item;

                itemSerializer.Single().Serialize(typeof(TListItemType), ref objItem, typeof(TListItemType).FullName, scopeNode.AddNode(ListItemNodeName), serializer);
            }
        }



        public void Deserialize(Type type, ref object target, string key, ConfigNode config, IConfigNodeSerializer serializer)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (key == null) throw new ArgumentNullException("key");
            if (config == null) throw new ArgumentNullException("config");
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", "key");

            if (!typeof(List<TListItemType>).IsAssignableFrom(type))
                throw new WrongSerializerException(target != null ? target.GetType() : type, typeof(List<TListItemType>));

            var list = target as List<TListItemType>;
            var itemSerializer = serializer.SerializerSelector.GetSerializer(typeof(TListItemType));

            if (!itemSerializer.Any())
                throw new NoSerializerFoundException(typeof(TListItemType));

            if (!config.HasNode(key))
            {
                list.Do(l => l.Clear());
                return;
            }

            list = list ?? new List<TListItemType>();
            list.Clear();

            foreach (var itemNode in config.GetNode(key).GetNodes(ListItemNodeName))
            {
                var item = CreateDefaultListItem();
                var objItem = (object) item;

                itemSerializer.Single()
                    .Deserialize(typeof (TListItemType), ref objItem, typeof (TListItemType).FullName, itemNode, serializer);

                item = (TListItemType) objItem;

                list.Add(item);
            }

            target = list;
        }


        private static TListItemType CreateDefaultListItem()
        {
            var tlt = typeof(TListItemType);

            if (tlt.IsValueType)
                return default(TListItemType);

            if (typeof(string) == tlt)
                return (TListItemType)(object)string.Empty; // no default constructor for string

            if (!tlt.IsAbstract && tlt.GetConstructors().Any(c => c.GetParameters().Length == 0))
                return Activator.CreateInstance<TListItemType>();

            throw new ArgumentException("No suitable default constructor for " + tlt.Name);
        }
    }
}
