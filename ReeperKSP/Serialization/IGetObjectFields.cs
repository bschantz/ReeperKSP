using System.Collections.Generic;
using System.Reflection;

namespace ReeperKSP.Serialization
{
    public interface IGetObjectFields
    {
        IEnumerable<FieldInfo> Get(object target);
    }
}
