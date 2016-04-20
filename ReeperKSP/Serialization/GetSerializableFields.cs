using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReeperKSP.Serialization
{
    // note: private members inherited by target type will be ignored
    public class GetSerializableFields : IGetObjectFields
    {
        public IEnumerable<FieldInfo> Get(object target)
        {
            if (target == null)
                return Enumerable.Empty<FieldInfo>();
            
            return target.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(
                    fi =>
                        fi.GetCustomAttributes(true).Any(attr => attr is ReeperPersistentAttribute));
        }
    }
}
