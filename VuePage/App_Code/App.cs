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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public class App : Control
    {
        private static Regex _re = new Regex(@"(<script?\b[^>]*>([\s\S]*?)<\/script>)|(<style[^>]*>([\s\S]*?)<\/style>)", RegexOptions.Compiled);
        private IViewModel _vm = null;

        #region Properties

        /// <summary>
        /// Get/Set view model class type to be created in Load (if empty, search for nested paged class view model)
        /// </summary>
        public string ViewModelType { get; set; }

        /// <summary>
        /// CSS class for vue-page div
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Define default transition between pages
        /// </summary>
        public string DefaultTransition { get; set; }

        /// <summary>
        /// Define back transition when users call back browser button
        /// </summary>
        public string BackTransition { get; set; }

        /// <summary>
        /// Keep, in browser, history viewmodel to preserve restore
        /// </summary>
        public int HistoryLength { get; set; } = 3;

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

        /// <summary>
        /// Load viewmodel on OnInit to avoid all Page lifecycle
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var viewModelType = string.IsNullOrEmpty(ViewModelType) ?
                Page.GetType().GetNestedTypes().FirstOrDefault(x => typeof(IViewModel).IsAssignableFrom(x)) :
                Type.GetType(ViewModelType, true, true);

            this.Mount(viewModelType);
        }

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
                else throw new SystemException("ViewModel contains unknown ctor parameter: " + par.Name);
            }

            _vm = (IViewModel)Activator.CreateInstance(type, parameters.ToArray());
        }

        public void Mount(IViewModel vm)
        {
            _vm = vm;
        }

        #endregion

        #region Render Rules

        protected override void Render(HtmlTextWriter writer)
        {
            var isAjax = Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var isPost = Page.Request.HttpMethod == "POST";

            // ajax GET request
            if (isAjax && !isPost)
            {
                var sb = new StringBuilder();

                RenderControl(sb);

                sb.AppendLine("<script>");
                sb.AppendLine("(function() {");
                sb.AppendLine("document.title = '" + Page.Title + "';");
                sb.AppendLine(_vm.RenderScript());
                sb.AppendLine("})();");
                sb.AppendLine("</script>");

                RenderAjax(sb.ToString());
            }

            // ajax POST UpdateModel
            else if(isAjax && isPost)
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
                var options = new { history = HistoryLength, defaultTransition = DefaultTransition, backTransition = BackTransition };
                var w = new StringBuilder();
                var scripts = new StringBuilder();
                var styles = new StringBuilder();

                RenderControl(w);

                // remove script tags
                var html = _re.Replace(w.ToString(), (m) =>
                {
                    scripts.AppendLine(m.Groups[1].Value.Trim());
                    styles.AppendLine(m.Groups[3].Value.Trim());
                    return "";
                });

                writer.WriteLine("<div class='vue-page vue-page-active {0}' data-url='{1}' data-options='{2}'>", CssClass, Page.Request.Url.AbsoluteUri, JsonConvert.SerializeObject(options));
                //base.Render(writer);
                writer.WriteLine(html);
                writer.WriteLine("</div>");
                writer.WriteLine("<script>");
                writer.WriteLine("document.addEventListener('DOMContentLoaded', function() {");
                writer.WriteLine(_vm.RenderScript());
                writer.WriteLine("}, false);");
                writer.WriteLine("</script>");
                writer.WriteLine(scripts.ToString());
                writer.WriteLine(styles.ToString());
            }
        }

        private void RenderAjax(string content)
        {
            Page.Response.ClearContent();
            Page.Response.ContentType = "text/json";
            Page.Response.Write(content);
            Page.Response.End();
        }

        private void RenderControl(StringBuilder sb)
        {
            using (var sw = new StringWriter(sb))
            {
                using (var w = new HtmlTextWriter(sw))
                {
                    base.Render(w);
                }
            }
        }

        #endregion
    }
}
