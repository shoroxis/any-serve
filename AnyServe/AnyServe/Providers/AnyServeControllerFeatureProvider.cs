using AnyServe.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AnyServe.Providers
{
    /// <summary>
    /// Support to load all availible controllers at runtime
    /// </summary>
    public class AnyServeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// Generate type marked with attribute "GeneratedControllerAttribute" and add it to availible controllers
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="feature"></param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var currentAssembly = typeof(AnyServeControllerFeatureProvider).Assembly;
            var candidates = currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<GeneratedControllerAttribute>().Any());

            foreach (var candidate in candidates)
            {
                feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo());
            }
        }
    }
}
