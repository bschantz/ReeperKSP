using System;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    public delegate Maybe<IConfigNodeItemSerializer> SurrogateFactoryMethod();

    public interface ISerializerSelector
    {
        Maybe<IConfigNodeItemSerializer> GetSerializer(Type target);
    }
}
