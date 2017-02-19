using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ASP = System.Web.UI;

namespace Vue
{
    public class Page : ASP.Page
    {
        private static Dictionary<string, string> _components = new Dictionary<string, string>();

        #region Properties

        /// <summary>
        /// Get/Set vue element to render as app
        /// </summary>
        public string Element { get; set; } = "#app";

        #endregion

        public Page()
        {
            Load += (s, e) =>
            {
                var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (isAjax && Request.HttpMethod == "POST")
                {
                    // update model in ajax/post call
                    UpdateModel();
                }
                else
                {
                    // if is ajax, do not load all components
                    if(!isAjax)
                    {
                        RegisterScripts();
                    }

                    // always register initializer
                    RegisterInitialize();
                }
            };
        }

        private void RegisterScripts()
        {
            // register vue/vue-page scripts
            Header.Controls.Add(new ASP.LiteralControl("\n<script src='/Scripts/vue.js'></script>\n"));
            Header.Controls.Add(new ASP.LiteralControl("<script src='/Scripts/vue-page.js'></script>\n"));
            Header.Controls.Add(new ASP.LiteralControl("<script src='/Scripts/vue-ajaxget.js'></script>\n"));

            // register vue/components
            foreach (var key in _components.Keys)
            {
                // load control and get viewModel + template
                var control = LoadControl(_components[key]);

                var vm = LoadViewModel(control);
                var template = GetTemplate(control);

                // include each component in page inital
                Header.Controls.Add(new ASP.LiteralControl("<script>\n" + vm.RenderComponent(key, template) + "\n</script>\n"));
            }
        }

        private void RegisterInitialize()
        {
            // get body html control
            var body = Controls.Cast<ASP.Control>()
                .Where(x => x is HtmlGenericControl)
                .Select(x => x as HtmlGenericControl)
                .Where(x => x.TagName == "body")
                .FirstOrDefault();

            if (body == null) throw new ArgumentException("Tag body run be runat=\"server\"");

            // load viewModel from Page
            var vmp = LoadViewModel(this);

            // initialize vue instance
            body.Controls.Add(new ASP.LiteralControl("<script>\n" + vmp.RenderInitialize(Element) + "\n</script>"));
        }

        private void UpdateModel()
        {
            var name = Request.Form["_name"];
            var model = Request.Form["_model"];
            var method = Request.Form["_method"];
            var parameters = JArray.Parse(Request.Form["_params"]).ToArray();
            var files = Request.Files.GetMultiple("_files");

            // load viewModel from control or from page
            var vm = name == null ?
                LoadViewModel(this) :
                LoadViewModel(LoadControl(_components[name]));

            // update model, execute server method and return model changes
            var update = vm.UpdateModel(model, method, parameters, files);

            // clear output and write only model updates
            Response.ClearContent();
            Response.ContentType = "text/json";
            Response.Write(update);
            Response.End();
        }

        private IViewModel LoadViewModel(ASP.Control control)
        {
            var viewModelType = control.GetType().GetNestedTypes().FirstOrDefault(x => typeof(IViewModel).IsAssignableFrom(x));

            if (viewModelType == null) throw new ApplicationException("ViewModel class not found in " + control.GetType().FullName);

            var ctor = viewModelType.GetConstructors().First();
            var parameters = new List<object>();

            // commom ctor parameters
            foreach(var par in ctor.GetParameters())
            {
                if (par.ParameterType == typeof(HttpContext)) parameters.Add(Context);
                else if (par.ParameterType == typeof(HttpRequest)) parameters.Add(Context.Request);
                else if (par.ParameterType == typeof(HttpResponse)) parameters.Add(Context.Response);
                else if (par.ParameterType == typeof(NameValueCollection)) parameters.Add(Context.Request.Params);
                else if (typeof(IPrincipal).IsAssignableFrom(par.ParameterType)) parameters.Add(Context.User);
                else if (typeof(Page).IsAssignableFrom(par.ParameterType)) parameters.Add(this.Page);
                else throw new SystemException("ViewModel contains unknown ctor parameter: " + par.Name);
            }

            return (IViewModel)Activator.CreateInstance(viewModelType, parameters.ToArray());
        }

        public static void Component(string name, string url)
        {
            _components[name] = url;
        }

        private string GetTemplate(ASP.Control control)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var w = new ASP.HtmlTextWriter(sw))
                {
                    control.RenderControl(w);
                }
            }

            return sb.ToString().Trim().Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("'", "\\'");
        }
    }
}
