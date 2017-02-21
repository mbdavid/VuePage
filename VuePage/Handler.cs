using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using ASP = System.Web.UI;

namespace Vue
{
    /// <summary>
    /// Define javascript code before call $server() method
    /// </summary>
    public class Handler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // register vue/components
                foreach (var component in Component.All.Values)
                {
                    // load control and get viewModel + template
                    var loader = new ASP.UserControl();
                    var control = loader.LoadControl(component.Url);

                    // if component has no viewModelType, auto-detect inside control
                    if (component.ViewModelType == null)
                    {
                        component.ViewModelType = control.GetType().GetNestedTypes().FirstOrDefault(x => typeof(ViewModel).IsAssignableFrom(x));

                        if (component.ViewModelType == null) throw new ArgumentException("ViewModel not found for component in " + component.Url);
                    }

                    var vm = ViewModelFactory.Load(component.ViewModelType, context);
                    var template = GetTemplate(control);

                    // include each component in page inital
                    context.Response.Write("//\n");
                    context.Response.Write("// Component: " + component.Name + "\n");
                    context.Response.Write("//\n");

                    context.Response.Write(vm.RenderComponent(component.Name, template));
                }
            }
            catch (Exception ex)
            {
                context.Response.ClearContent();
                context.Response.Write("alert('VuePage: Error on load compoenent: " + ex.Message);
            }

            context.Response.ContentType = "text/javascript";
            //context.Response.ExpiresAbsolute = DateTime.Now.AddHours(1);
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
