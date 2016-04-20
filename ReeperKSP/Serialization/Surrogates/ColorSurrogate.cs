using UnityEngine;

namespace ReeperKSP.Serialization.Surrogates
{
    // ReSharper disable once UnusedMember.Global
    public class ColorSurrogate : SingleValueSurrogate<Color>
    {
        protected override string GetFieldContentsAsString(Color instance)
        {
            return ConfigNode.WriteColor(instance);
        }

        protected override Color GetFieldContentsFromString(string value)
        {
            return ConfigNode.ParseColor(value);
        }
    }
}
