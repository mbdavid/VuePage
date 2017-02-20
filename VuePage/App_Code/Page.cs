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
        private Dictionary<string, Type> _viewModels = new Dictionary<string, Type>();

        public Page()
        {
            // auto register inner class page viewModel
            Init += (s, e) =>
            {
                var types = this.GetType().GetNestedTypes().Where(x => typeof(IViewModel).IsAssignableFrom(x));
                
                foreach(var type in types)
                {
                    var el = type.GetCustomAttribute<ElementAttribute>()?.Id ?? "app";

                    if (el.StartsWith("#")) throw new ArgumentException("Vue.Element id could not starts with #");

                    _viewModels[el] = type;
                }
            };

            InitComplete += (s, e) =>
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

        /// <summary>
        /// Manual register new view model factory
        /// </summary>
        public void RegisterViewModel<T>(string id) where T : IViewModel
        {
            if (string.IsNullOrEmpty(id) || id.StartsWith("#")) throw new ArgumentException("ID element could not be null or starts with #");

            _viewModels.Add(id, typeof(T));
        }

        private void RegisterScripts()
        {
            // register vue/vue-page scripts
            Header.Controls.Add(new ASP.LiteralControl("\n<script src=\"/Scripts/vue.js\"></script>\n"));
            Header.Controls.Add(new ASP.LiteralControl("<script src=\"/Scripts/vue-page.js\"></script>\n"));
            Header.Controls.Add(new ASP.LiteralControl("<script src=\"/Scripts/vue-ajaxget.js\"></script>\n"));
            Header.Controls.Add(new ASP.LiteralControl("<script src=\"VueHandler.ashx\"></script>\n"));
        }

        private void RegisterInitialize()
        {
            var controls = Master != null ?
                Master.Controls.Cast<ASP.Control>() :
                Controls.Cast<ASP.Control>();

            // get body html control
            var body = controls
                .Where(x => x is HtmlGenericControl)
                .Select(x => x as HtmlGenericControl)
                .Where(x => x.TagName == "body")
                .FirstOrDefault();

            if (body == null) throw new ArgumentException("Tag body run be runat=\"server\"");

            foreach(var el in _viewModels.Keys)
            {
                // load viewModel from Page
                var vm = ViewModelFactory.Load(_viewModels[el], Context);

                // initialize vue instance
                body.Controls.Add(new ASP.LiteralControl("<script>\n" + vm.RenderInitialize(el) + "\n</script>"));
            }
        }

        private void UpdateModel()
        {
            var name = Request.Form["_name"];
            var model = Request.Form["_model"];
            var method = Request.Form["_method"];
            var parameters = JArray.Parse(Request.Form["_params"]).ToArray();
            var files = Request.Files.GetMultiple("_files");

            // load viewModel
            var vm = name.StartsWith("#") ?
                ViewModelFactory.Load(_viewModels[name.Substring(1)], Context) :
                ViewModelFactory.Load(Component.All[name].ViewModelType, Context);

            // update model, execute server method and return model changes
            var update = vm.UpdateModel(model, method, parameters, files);

            // clear output and write only model updates
            Response.ClearContent();
            Response.ContentType = "text/json";
            Response.Write(update);
            Response.End();
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
