using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDrive
{
    public class FormFactory
    {

        public static IServiceProvider _serviceProvide;

        public static Form GetForm(string formName)
        {
            var forms = _serviceProvide.GetService<IEnumerable<IAppForm>>();
            return forms.FirstOrDefault(f => f.FormName?.ToLower() == formName.ToLower()) as Form;
        }

        public static Form GetNext(IAppForm form)
        {
            if (form.FormName == "login") return GetForm("main");
            return null;
        }
    }
}
