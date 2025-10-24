using General;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDrive
{
    internal static class Registrar
    {
        internal static IServiceCollection AddWinForms(this IServiceCollection services)
        {
            return services.AddGeneric<IAppForm>();
        }
    }
}
