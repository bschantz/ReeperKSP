using System;

namespace ReeperKSP.Serialization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ReeperPersistentAttribute : Attribute
    {
    }
}
