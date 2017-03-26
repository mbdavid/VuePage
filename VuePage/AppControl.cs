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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ASP = System.Web.UI;

namespace Vue
{
    public class AppControl : Control
    {
        public AppControl()
        {
            Init += (s, e) =>
            {
                var isAjax = Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                var component = Page.Request.QueryString["component"];

                if (isAjax && Page.Request.HttpMethod == "POST")
                {
                    // update model in ajax/post call
                    UpdateModel();
                }
                else if(isAjax && component != null)
                {
                    var control = ViewModel.FindComponent(component);
                    var sb = new StringBuilder();

                    using (var sw = new StringWriter(sb))
                    {
                        using (var w = new ASP.HtmlTextWriter(sw))
                        {
                            control.RenderControl(w);
                        }
                    }

                    var viewModelType = GetViewModelType(control);
                    var vm = ViewModel.Load(viewModelType, Context);

                    Page.Response.Write(vm.RenderComponent(component, sb.ToString()));
                    Page.Response.End();
                }
                else if(!isAjax)
                {
                    // full GET page, register vue
                    RegisterScripts();
                }
            };
        }

        private void RegisterScripts()
        {
            // reigster all scripts at begin tag (thats why are inverted)
            Page.Header.Controls.AddAt(0, new ASP.LiteralControl(string.Format("<script src=\"{0}\"></script>\n", Page.ClientScript.GetWebResourceUrl(typeof(AppControl), "VuePage.Scripts.vue-ajaxget.js"))));
            Page.Header.Controls.AddAt(0, new ASP.LiteralControl(string.Format("<script src=\"{0}\"></script>\n", Page.ClientScript.GetWebResourceUrl(typeof(AppControl), "VuePage.Scripts.vue-page.js"))));
            Page.Header.Controls.AddAt(0, new ASP.LiteralControl(string.Format("<script src=\"{0}\"></script>\n", Page.ClientScript.GetWebResourceUrl(typeof(AppControl), "VuePage.Scripts.vue.js"))));
        }

        protected override void Render(ASP.HtmlTextWriter writer)
        {
            writer.WriteLine("<div id='" + ClientID + "'></div>");
            writer.WriteLine("<script>");
            writer.WriteLine("(function() {");

            using (var vm = ViewModel.Load(GetViewModelType(this.Page), Context))
            {
                var script = vm.RenderControl(ClientID, GetContent());

                writer.WriteLine(script);
            }

            writer.WriteLine("})();");
            writer.WriteLine("</script>");
        }

        private void UpdateModel()
        {
            var request = Page.Request;
            var response = Page.Response;

            var name = request.Form["_name"];
            var model = request.Form["_model"];
            var method = request.Form["_method"];
            var parameters = JArray.Parse(request.Form["_params"]).ToArray();
            var files = request.Files.GetMultiple("_files");
            var update = string.Empty;

            var control = ViewModel.FindComponent(name) ?? this.Page;
            var viewModelType = GetViewModelType(control);

            // load viewModel
            using (var vm = ViewModel.Load(viewModelType, Context))
            {
                // update model, execute server method and return model changes
                update = vm.UpdateModel(model, method, parameters, files);
            }

            // clear output and write only model updates
            response.ClearContent();
            response.ContentType = "text/json";
            response.Write(update);
            response.End();
        }

        private Type GetViewModelType(Control control)
        {
            var viewModelType = control.GetType()
                .GetNestedTypes()
                .Where(x => typeof(ViewModel).IsAssignableFrom(x))
                .FirstOrDefault();

            if (viewModelType == null) throw new Exception("ViewModel not found");

            return viewModelType;
        }

        private string GetContent()
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var w = new ASP.HtmlTextWriter(sw))
                {
                    base.Render(w);
                }
            }

            return sb.ToString();
        }
    }
}
