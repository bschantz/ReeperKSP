using System.Linq;
using ReeperCommon.Containers;

namespace ReeperKSPUnitTests.Serialization.Surrogates
{
    public static class ConfigNodeComparer
    {
        public static bool Similar(ConfigNode x, ConfigNode y)
        {
            var firstValues = x.values.Cast<ConfigNode.Value>().ToList();

            if (!firstValues.All(v => GetValue(y, v).Any()))
                return false;

            var firstNodes = x.nodes.Cast<ConfigNode>().ToList();

            return firstNodes.All(n => y.HasNode(n.name) && Similar(n, y.GetNode(n.name)));
        }


        private static Maybe<ConfigNode.Value> GetValue(ConfigNode toSearch, ConfigNode.Value searchFor)
        {
            foreach (ConfigNode.Value value in toSearch.values)
                if (value.name == searchFor.name)
                    if (value.value == searchFor.value)
                        return Maybe<ConfigNode.Value>.With(value);

            return Maybe<ConfigNode.Value>.None;
        }
    }
}
