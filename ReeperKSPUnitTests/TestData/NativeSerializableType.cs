using System;
using ReeperKSP.Serialization;
using UnityEngine;

namespace ReeperKSPUnitTests.TestData
{
    public class NativeSerializableType : IReeperPersistent, IEquatable<NativeSerializableType>
    {
        [ReeperPersistent] public float FloatField = -1f;


        public void DuringSerialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
            node.AddValue("SomeCustomValue", "EqualsThis");
        }

        public void DuringDeserialize(IConfigNodeSerializer formatter, ConfigNode node)
        {

        }

        public override int GetHashCode()
        {
            return FloatField.GetHashCode();
        }


        public bool Equals(NativeSerializableType other)
        {
            return other != null && Mathf.Approximately(FloatField, other.FloatField);
        }

        bool IEquatable<NativeSerializableType>.Equals(NativeSerializableType other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NativeSerializableType);
        }
    }
}