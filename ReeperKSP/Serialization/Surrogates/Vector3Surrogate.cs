using UnityEngine;

namespace ReeperKSP.Serialization.Surrogates
{
    // ReSharper disable once UnusedMember.Global
    public class Vector3Surrogate : SingleValueSurrogate<Vector3>
    {
        protected override string GetFieldContentsAsString(Vector3 instance)
        {
            return KSPUtil.WriteVector(instance);
        }

        protected override Vector3 GetFieldContentsFromString(string value)
        {
            return KSPUtil.ParseVector3(value);
        }
    }
}
