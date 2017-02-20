using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vue
{
    internal class ViewModelFactory
    {
        /// <summary>
        /// Create new instance based on ViewModel type
        /// </summary>
        public static IViewModel Load(Type viewModelType, HttpContext context)
        {
            var ctor = viewModelType.GetConstructors().First();
            var parameters = new List<object>();

            // commom ctor parameters
            foreach (var par in ctor.GetParameters())
            {
                if (par.ParameterType == typeof(HttpContext)) parameters.Add(context);
                else if (par.ParameterType == typeof(HttpRequest)) parameters.Add(context.Request);
                else if (par.ParameterType == typeof(HttpResponse)) parameters.Add(context.Response);
                else if (par.ParameterType == typeof(NameValueCollection)) parameters.Add(context.Request.Params);
                else if (typeof(IPrincipal).IsAssignableFrom(par.ParameterType)) parameters.Add(context.User);
                else throw new SystemException("ViewModel contains unknown ctor parameter: " + par.Name);
            }

            return (IViewModel)Activator.CreateInstance(viewModelType, parameters.ToArray());
        }
    }
}
