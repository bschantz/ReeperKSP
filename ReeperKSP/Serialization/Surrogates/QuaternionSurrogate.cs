using UnityEngine;

namespace ReeperKSP.Serialization.Surrogates
{
    // ReSharper disable once UnusedMember.Global
    public class QuaternionSurrogate : SingleValueSurrogate<Quaternion>
    {
        protected override string GetFieldContentsAsString(Quaternion instance)
        {
            return KSPUtil.WriteQuaternion(instance);
        }

        protected override Quaternion GetFieldContentsFromString(string value)
        {
            return KSPUtil.ParseQuaternion(value);
        }
    }
}
