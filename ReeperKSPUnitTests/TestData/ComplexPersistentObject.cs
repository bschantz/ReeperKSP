using System;
using ReeperKSP.Serialization;
using UnityEngine;

namespace ReeperKSPUnitTests.TestData
{
    public class ComplexPersistentObject : IEquatable<ComplexPersistentObject>
    {
        public class InternalPersistent : IReeperPersistent, IPersistenceLoad, IPersistenceSave
        {
            protected bool Equals(InternalPersistent other)
            {
                return true;
            }


            public void DuringSerialize(IConfigNodeSerializer formatter, ConfigNode node)
            {
                node.AddValue("InternalPersistent", "TestValue");
            }

            public void DuringDeserialize(IConfigNodeSerializer formatter, ConfigNode node)
            {

            }

            public void PersistenceLoad()
            {
                
            }

            public void PersistenceSave()
            {

            }

            public override bool Equals(object obj)
            {
                return true;
            }
        }

        public ComplexPersistentObject()
        {
           
        }
        [ReeperPersistent] public IReeperPersistent MyTestObject = new InternalPersistent();
        [ReeperPersistent] public string MyTestString = "Hello, world";
        [ReeperPersistent] public int MyIntegerValue = 123;
        [ReeperPersistent] public float MyFloatValue = 123.45f;
        [ReeperPersistent] public double MyDoubleValue = 123.45678f;
        [ReeperPersistent] public Vector2 MyTestVector2 = new Vector2(12f, 21f);

        public bool Equals(ComplexPersistentObject other)
        {
            return Equals(MyTestObject, other.MyTestObject) && string.Equals(MyTestString, other.MyTestString) && MyIntegerValue == other.MyIntegerValue && MyFloatValue.Equals(other.MyFloatValue) && MyDoubleValue.Equals(other.MyDoubleValue) && MyTestVector2.Equals(other.MyTestVector2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (MyTestObject != null ? MyTestObject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MyTestString != null ? MyTestString.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MyIntegerValue;
                hashCode = (hashCode * 397) ^ MyFloatValue.GetHashCode();
                hashCode = (hashCode * 397) ^ MyDoubleValue.GetHashCode();
                hashCode = (hashCode * 397) ^ MyTestVector2.GetHashCode();
                return hashCode;
            }
        }

    }
}
