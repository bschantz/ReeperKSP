using System;
using System.Reflection;

namespace ReeperKSP.Extensions
{
    public static class AssemblyExtensions
    {
        public static bool DisablePlugin(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            for (int idx = 0; idx < AssemblyLoader.loadedAssemblies.Count; ++idx)
                if (ReferenceEquals(assembly, AssemblyLoader.loadedAssemblies[idx].assembly))
                {
                    AssemblyLoader.loadedAssemblies.RemoveAt(idx);
                    return true;
                }

            return false;
        }
    }
}
