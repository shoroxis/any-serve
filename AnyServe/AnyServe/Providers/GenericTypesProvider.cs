using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace AnyServe.Providers
{
    public static class GenericTypesProvider
    {
        public static Dictionary<string, IEnumerable<Type>> GetModelTypes()
        {
            var ownAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("AnyServ"));
            Dictionary<string, IEnumerable<Type>> types = new Dictionary<string, IEnumerable<Type>>();

            foreach (var currentAssembly in ownAssemblies)
            {
                types.Add(currentAssembly.FullName, currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<GeneratedControllerAttribute>().Any()));
            }

            return types;
        }
    }
}
