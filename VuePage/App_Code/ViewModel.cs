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
    public class ViewModel<T> : IViewModel
    {
        private Javascript _js = new Javascript();
        private Dictionary<string, string> _computed = new Dictionary<string, string>();
        private Dictionary<string, Action<object, object>> _watch = new Dictionary<string, Action<object, object>>();
        private JsonSerializerSettings _serializeSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };

        protected Javascript JS { get { return _js; } }
        protected HttpContext Context { get { return HttpContext.Current; } }

        public void Initialize()
        {
            Created();
        }

        protected virtual void Created()
        {
        }

        #region Watch

        protected virtual void NotifyWatch(string name, object value, object oldValue)
        {
            Action<object, object> action;

            if(_watch.TryGetValue(name, out action))
            {
                action(value, oldValue);
            }
        }

        protected void Watch<K>(Expression<Func<T, K>> expr, Action<K, K> action)
        {
            _watch.Add(expr.KeyPath(), (n, o) => action((K)Convert.ChangeType(n, typeof(K)), (K)Convert.ChangeType(o, typeof(K))));
        }

        protected void Watch<K>(Expression<Func<T, K>> expr, Action<K> action)
        {
            _watch.Add(expr.KeyPath(), (n, o) => action((K)Convert.ChangeType(n, typeof(K))));
        }

        protected void Watch<K>(Expression<Func<T, K>> expr, Action action)
        {
            _watch.Add(expr.KeyPath(), (n, o) => action());
        }

        protected void Watch(string keyPath, Action<object, object> action)
        {
            _watch.Add(keyPath, action);
        }

        #endregion

        #region Computed

        protected void Computed(string key, Expression<Func<T, object>> expr)
        {
            var v = new JsExpressionVisitor();
            v.Visit(expr);
            var js = "return " + v.JavaScriptCode + "(this);";

            _computed.Add(key, js);
        }

        protected void Computed(string key, string jsExpression)
        {
            _computed.Add(key, "return " + jsExpression + ";");
        }

        #endregion

        #region RenderScript

        public virtual string RenderScript()
        {
            var writer = new StringBuilder();
            var model = JObject.FromObject(this);

            //writer.AppendLine("(function() {");
            writer.Append("var vm = new Vue({\n");
            writer.Append("  el: '.vue-page-active',\n");
            writer.AppendFormat("  data: {0},\n", JsonConvert.SerializeObject(this, _serializeSettings));
            writer.AppendLine("  methods: { ");

            var methods = this.GetType().GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => !x.IsSpecialName).ToArray();

            foreach (var m in methods)
            {
                // get all parameters without HttpPostFile parameters
                var parameters = m.GetParameters()
                    .Where(x => x.ParameterType != typeof(HttpPostedFile) && x.ParameterType != typeof(List<HttpPostedFile>))
                    .Select(x => x.Name);

                // get if any parameter are file(s)
                var upload = m.GetParameters()
                    .Where(x => x.ParameterType == typeof(HttpPostedFile) || x.ParameterType == typeof(List<HttpPostedFile>))
                    .Select(x => x.Name)
                    .FirstOrDefault() ?? "null";

                writer.AppendFormat("    '{0}': function({1}) {{ this.$server('{0}', [{2}], {3}, this); }},\n", 
                    m.Name,
                    string.Join(", ", m.GetParameters().Select(x => x.Name)),
                    string.Join(", ", parameters),
                    upload);
            }

            writer.Length -= 2;
            writer.AppendLine("\n  },");
            writer.AppendLine("  computed: { ");

            foreach (var c in _computed)
            {
                writer.AppendFormat("    '{0}': function() {{ {1} }},\n",
                    c.Key,
                    c.Value);
            }

            writer.Length -= 2;
            writer.AppendLine("\n  },");
            writer.AppendLine("  watch: { ");

            foreach(var w in _watch.Keys)
            {
                writer.AppendFormat("    '{0}': {{ handler: function(v, o) {{ this.$server('NotifyWatch', ['{0}', v, o], this); }}, deep: true }},\n", 
                    w);
            }

            writer.Length -= 2;

            writer.AppendLine("\n  },");
            writer.AppendLine("  mounted: function() { this.$el.style['visibility'] = 'visible'; }");
            writer.AppendLine("});");

            // add user javascript
            if(_js.Length > 0)
            {
                writer.AppendLine("(function() {");
                writer.AppendLine(_js.ToString());
                writer.AppendLine("}).call(vm)");
            }

            //writer.AppendLine("})();");

            return writer.ToString();
        }

        #endregion

        #region Update Model

        public virtual string UpdateModel(string model, string method, object[] parameters, IList<HttpPostedFile> files)
        {
            JsonConvert.PopulateObject(model, this, _serializeSettings);

            ExecuteMethod(method, parameters, files);

            return RenderUpdate(model);
        }

        protected virtual void ExecuteMethod(string name, object[] parameters, IList<HttpPostedFile> files)
        {
            var method = this.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var pars = new List<object>();
            var index = 0;

            foreach (var p in method.GetParameters())
            {
                if(p.ParameterType == typeof(HttpPostedFile))
                {
                    pars.Add(files.FirstOrDefault());
                }
                else if (p.ParameterType == typeof(List<HttpPostedFile>))
                {
                    pars.Add(new List<HttpPostedFile>(files));
                }
                else
                {
                    pars.Add(Convert.ChangeType(parameters[index++], p.ParameterType));
                }
            }

            method.Invoke(this, pars.ToArray());
        }

        protected string RenderUpdate(string model)
        {
            var original = JObject.Parse(model);
            var current = JObject.FromObject(this);
            var diff = new JObject();

            foreach (var item in current)
            {
                var o = original[item.Key];

                if (original[item.Key] == null && item.Value.HasValues == false) continue;

                if (!JToken.DeepEquals(original[item.Key], item.Value))
                {
                    diff[item.Key] = item.Value;
                }
            }

            var output = new JObject
            {
                { "update", diff },
                { "js", this.JS.ToString() }
            };

            return output.ToString();
        }

        #endregion
    }
}
