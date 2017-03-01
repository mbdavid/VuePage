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

        private ASP.Control _script = null;

        public Page()
        {
            // auto register inner class page viewModel
            Init += (s, e) =>
            {
                var types = this.GetType().GetNestedTypes().Where(x => typeof(ViewModel).IsAssignableFrom(x));
                
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
                    var controls = Master != null ?
                        Master.Controls.Cast<ASP.Control>() :
                        Controls.Cast<ASP.Control>();

                    // find script placeholder (body runat=server, div#id=vuescript, [class=vue-script])
                    _script = controls
                        .Where(x => x is HtmlGenericControl)
                        .Select(x => x as HtmlGenericControl)
                        .Where(x => x.TagName == "body" || x.ID == "vuescript" || (x.Attributes["class"]?.Contains("vue-script") ?? false))
                        .FirstOrDefault();

                    if (_script == null) throw new ArgumentException("Vue script placeholder not found. Use tag <body runat=\"server\"> or <placeholder id=\"vuescript\" runat=\"server\"></placeholder>");

                    // if is ajax, do not load all components
                    if (!isAjax)
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
        public void RegisterViewModel<T>(string id) where T : ViewModel
        {
            if (string.IsNullOrEmpty(id) || id.StartsWith("#")) throw new ArgumentException("ID element could not be null or starts with #");

            _viewModels.Add(id, typeof(T));
        }

        private void RegisterScripts()
        {
            // reigster all scripts at begin tag (thats why are inverted)

            var version = DateTime.Now.Ticks.ToString(); // typeof(VueHandler).Assembly.GetName().Version.ToString();

            _script.Controls.AddAt(0, new ASP.LiteralControl("<script src=\"VueHandler.ashx?_=" + version + "\"></script>\n"));

            _script.Controls.AddAt(0, new ASP.LiteralControl(string.Format("<script src=\"{0}\"></script>\n", ClientScript.GetWebResourceUrl(typeof(Page), "VuePage.Resources.vue-ajaxget.js"))));
            _script.Controls.AddAt(0, new ASP.LiteralControl(string.Format("<script src=\"{0}\"></script>\n", ClientScript.GetWebResourceUrl(typeof(Page), "VuePage.Resources.vue-page.js"))));
            _script.Controls.AddAt(0, new ASP.LiteralControl(string.Format("<script src=\"{0}\"></script>\n", ClientScript.GetWebResourceUrl(typeof(Page), "VuePage.Resources.vue.js"))));
        }

        private void RegisterInitialize()
        {
            foreach (var el in _viewModels.Keys)
            {
                // load viewModel from Page
                using (var vm = ViewModel.Load(_viewModels[el], Context))
                {
                    // initialize vue instance
                    _script.Controls.Add(new ASP.LiteralControl("<script>\n" + vm.RenderInitialize(el) + "\n</script>"));
                }
            }
        }

        private void UpdateModel()
        {
            var name = Request.Form["_name"];
            var model = Request.Form["_model"];
            var method = Request.Form["_method"];
            var parameters = JArray.Parse(Request.Form["_params"]).ToArray();
            var files = Request.Files.GetMultiple("_files");
            var update = string.Empty;

            // load viewModel
            using (var vm = name.StartsWith("#") ?
                ViewModel.Load(_viewModels[name.Substring(1)], Context) :
                ViewModel.Load(Component.All[name].ViewModelType, Context))
            {
                // update model, execute server method and return model changes
                update = vm.UpdateModel(model, method, parameters, files);
            }

            // clear output and write only model updates
            Response.ClearContent();
            Response.ContentType = "text/json";
            Response.Write(update);
            Response.End();
        }
    }
}
