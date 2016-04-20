using System;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    public interface ISurrogateProvider
    {
        Maybe<IConfigNodeItemSerializer> Get(Type toBeSerialized);
    }
}
