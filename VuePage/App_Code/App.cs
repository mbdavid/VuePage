using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public class App : Control
    {
        private IViewModel _vm = null;

        #region Properties

        private bool IsAjax { get { return Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest"; } }

        private bool IsPost { get { return Page.Request.HttpMethod == "POST"; } }

        /// <summary>
        /// CSS class for vue-container div
        /// </summary>
        public string CssClassContainer { get; set; }

        /// <summary>
        /// CSS class for inner vue-page div
        /// </summary>
        public string CssClassPage { get; set; }

        #endregion

        public App()
        {
            //PreRender += (s, e) =>
            //{
            //    Page.ClientScript.RegisterClientScriptInclude("vue", "/Scripts/vue.js");
            //    Page.ClientScript.RegisterClientScriptInclude("vue-page", "/Scripts/vue-page.js");
            //};
        }

        #region Mount

        public void Mount<T>() where T : IViewModel, new()
        {
            Mount(new T());
        }

        public void Mount(Type type)
        {
            var ctor = type.GetConstructors().First();
            var parameters = new List<object>();

            // commom ctor parameters
            foreach(var par in ctor.GetParameters())
            {
                if (par.ParameterType == typeof(HttpContext)) parameters.Add(Context);
                else if (par.ParameterType == typeof(HttpRequest)) parameters.Add(Context.Request);
                else if (par.ParameterType == typeof(HttpResponse)) parameters.Add(Context.Response);
                else if (par.ParameterType == typeof(NameValueCollection)) parameters.Add(Context.Request.Params);
                else if (typeof(IPrincipal).IsAssignableFrom(par.ParameterType)) parameters.Add(Context.User);
                else throw new SystemException("ViewModel contains unknow ctor paramter: " + par.Name);
            }

            var vm = (IViewModel)Activator.CreateInstance(type, parameters.ToArray());

            Mount(vm);
        }

        public void Mount(IViewModel vm)
        {
            _vm = vm;

            if (!IsPost)
            {
                _vm.Initialize();
            }
        }

        #endregion

        #region Render Rules

        protected override void Render(HtmlTextWriter writer)
        {
            // ajax GET request
            if (IsAjax && !IsPost)
            {
                var sb = new StringBuilder();

                using (var sw = new StringWriter(sb))
                {
                    using (var w = new HtmlTextWriter(sw))
                    {
                        base.Render(w);
                    }
                }

                sb.AppendLine("<script>");
                sb.AppendLine(_vm.RenderScript());
                sb.AppendLine("</script>");

                RenderAjax(sb.ToString());
            }

            // ajax POST UpdateModel
            else if(IsAjax && IsPost)
            {
                var model = Page.Request.Form["_model"];
                var method = Page.Request.Form["_method"];
                var parameters = JArray.Parse(Page.Request.Form["_params"]).Select(x => x.Value<object>()).ToArray();
                var files = Page.Request.Files.GetMultiple("_files");

                // update model, execute server method and return model changes
                var update = _vm.UpdateModel(model, method, parameters, files);

                // clear output and write only model updates
                RenderAjax(update);
            }

            // simple GET/POST request (render template + script)
            else
            {
                writer.WriteLine("<div class=\"vue-container {0}\">", CssClassContainer);
                writer.WriteLine("<div class=\"vue-page vue-page-active {0}\" data-url=\"{1}\">", CssClassPage, Page.Request.Url.AbsoluteUri);
                base.Render(writer);
                writer.WriteLine("</div>");
                writer.WriteLine("</div>");
                writer.WriteLine("<script>");
                writer.WriteLine("document.addEventListener('DOMContentLoaded', function() {");
                writer.WriteLine(_vm.RenderScript());
                writer.WriteLine("}, false);");
                writer.WriteLine("</script>");
            }
        }

        private void RenderAjax(string content)
        {
            Page.Response.ClearContent();
            Page.Response.ContentType = "text/json";
            Page.Response.Write(content);
            Page.Response.End();
        }

        #endregion
    }
}
