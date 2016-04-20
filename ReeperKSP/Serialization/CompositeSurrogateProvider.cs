using System;
using System.Linq;
using ReeperCommon.Containers;

namespace ReeperKSP.Serialization
{
    public class CompositeSurrogateProvider : ISurrogateProvider
    {
        private readonly ISurrogateProvider[] _providers;

        public CompositeSurrogateProvider(params ISurrogateProvider[] providers)
        {
            if (providers == null) throw new ArgumentNullException("providers");

            _providers = providers;
        }

        public Maybe<IConfigNodeItemSerializer> Get(Type toBeSerialized)
        {
            foreach (var p in _providers)
            {
                var result = p.Get(toBeSerialized);

                if (result.Any())
                    return result;
            }

            return Maybe<IConfigNodeItemSerializer>.None;
        }
    }
}
