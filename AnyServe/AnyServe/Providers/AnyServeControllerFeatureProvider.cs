using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AnyServe.Providers
{
    public class AnyServeControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo);

            if (!isController)
            {
                string[] validEndings = new[] { "Foobar", "Controller`1" };

                isController = validEndings.Any(x =>
                    typeInfo.Name.EndsWith(x, StringComparison.OrdinalIgnoreCase));
            }

            Console.WriteLine($"{typeInfo.Name} IsController: {isController}.");

            return isController;
        }
    }
}
