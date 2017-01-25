using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private bool IsAjax { get { return Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest"; } }

        private bool IsPost { get { return Page.Request.HttpMethod == "POST"; } }

        public App()
        {
            //PreRender += (s, e) =>
            //{
            //    Page.ClientScript.RegisterClientScriptInclude("vue", "/Scripts/vue.js");
            //    Page.ClientScript.RegisterClientScriptInclude("vue-page", "/Scripts/vue-page.js");
            //};
        }

        public void Mount<T>() where T : IViewModel, new()
        {
            Mount(new T());
        }

        public void Mount(IViewModel vm)
        {
            _vm = vm;

            if (!IsPost)
            {
                _vm.Initialize();
            }
        }

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

                // update model, execute server method and return model changes
                var update = _vm.UpdateModel(model, method, parameters);

                // clear output and write only model updates
                RenderAjax(update);
            }

            // simple GET/POST request (render template + script)
            else
            {
                writer.WriteLine("<div class=\"vue-page vue-page-active\" data-url=\"{0}\">", Page.Request.Url.AbsoluteUri);
                base.Render(writer);
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
            Page.Response.Clear();
            Page.Response.ContentType = "text/json";
            Page.Response.Write(content);
            Page.Response.End();
        }
    }
}
