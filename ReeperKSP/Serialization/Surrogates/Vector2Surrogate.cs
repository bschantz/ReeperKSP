using UnityEngine;

namespace ReeperKSP.Serialization.Surrogates
{
// ReSharper disable once ClassNeverInstantiated.Global
    public class Vector2Surrogate : SingleValueSurrogate<Vector2>
    {
        protected override string GetFieldContentsAsString(Vector2 instance)
        {
            return KSPUtil.WriteVector(instance);
        }

        protected override Vector2 GetFieldContentsFromString(string value)
        {
            return KSPUtil.ParseVector2(value);
        }
    }
}
