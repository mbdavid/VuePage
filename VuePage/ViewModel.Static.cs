using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace Vue
{
    public partial class ViewModel : IDisposable
    {
        #region Computed

        /// <summary>
        /// Resolve an expression to convert into a computed field
        /// </summary>
        public static Computed Resolve<T>(Expression<Func<T, object>> expr) where T : ViewModel
        {
            return new Computed
            {
                Code = JavascriptExpressionVisitor.Resolve(expr),
                Value = (object o) => expr.Compile()
            };
        }

        #endregion

        /// <summary>
        /// Create new instance based on ViewModel type
        /// </summary>
        internal static ViewModel Load(Type viewModelType, HttpContext context)
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

            var vm = (ViewModel)Activator.CreateInstance(viewModelType, parameters.ToArray());

            vm.Context = context;

            vm.OnInit();

            return vm;
        }

        internal static string ParseTemplate(string content, out string script, out string style)
        {
            var reScript = new Regex(@"<script?\b[^>]*>([\s\S]*?)<\/script>", RegexOptions.IgnoreCase);
            var reStyle = new Regex(@"<style\s*(scoped)?\b[^>]*>([\s\S]*?)<\/style>", RegexOptions.IgnoreCase);
            var reTag = new Regex(@"^\s*<\w+");
            var isScoped = false;

            var code = new StringBuilder();
            var css = new StringBuilder();

            content = reScript.Replace(content, (m) =>
            {
                code.Append(m.Groups[1].Value);
                return "";
            });

            content = reStyle.Replace(content, (m) =>
            {
                if (m.Groups[1].Value.Length > 0) isScoped = true;

                css.Append(m.Groups[2].Value);
                return "";
            });

            // add scoped style
            if (isScoped)
            {
                var scope = "s" + Guid.NewGuid().ToString("n").Substring(0, 5);
                css.Insert(0, $"[data-{scope}] {{ ");
                css.Append(" }");

                content = reTag.Replace(content, (m) =>
                {
                    return m.Value + $" data-{scope} ";
                });
            }

            script = code.ToString();
            style = css.ToString();

            return JavascriptBuilder.Encode(content.Trim());
        }
    }
}
