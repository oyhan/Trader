using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace General
{
    public static class Registrar
    {
        public static IServiceCollection AddGeneric<T>(this IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            var types = assembly.GetTypes().Where(c =>
            c.GetInterface(typeof(T).Name)
            != null && !c.IsInterface && !c.IsAbstract);

            foreach (var type in types)
            {
                services.AddTransient(typeof(T), type);
            }

            return services;
        }

      
    }

}
