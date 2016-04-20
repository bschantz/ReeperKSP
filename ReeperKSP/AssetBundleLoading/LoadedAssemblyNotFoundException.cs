using System;
using System.Reflection;

namespace ReeperKSP.AssetBundleLoading
{
    public class LoadedAssemblyNotFoundException : Exception
    {
        public LoadedAssemblyNotFoundException(Assembly target)
            : base(
                string.Format("Could not find target '{0}' in LoadedAssembly list. Has someone messed with that list?",
                    target.GetName().Name))
        {
        }

        public LoadedAssemblyNotFoundException(AssemblyLoader.LoadedAssembly loadedAssembly)
            : base(
                typeof(AssemblyLoader).Name + "." + typeof(AssemblyLoader.LoadedAssembly).Name +
                " does not contain a url for " + loadedAssembly.assembly.GetName().Name)
        {
        }
    }
}