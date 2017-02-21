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
    public class ViewModel
    {
        private Javascript _js = new Javascript();
        private Dictionary<string, string> _computed = new Dictionary<string, string>();
        private JsonSerializerSettings _serializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        protected Javascript JS { get { return _js; } }

        public ViewModel()
        {
        }

        #region Created

        /// <summary>
        /// In page call during initialize. In component, made ajax call when component are created
        /// </summary>
        public virtual void Created()
        {
        }

        #endregion

        #region Computed

        protected void Computed<T>(string key, Expression<Func<T, object>> expr)
        {
            var v = new JsExpressionVisitor();
            v.Visit(expr);
            var js = "return (" + v.JavaScriptCode + ")(this);";

            _computed.Add(key, js);
        }

        protected void Computed(string key, string jsExpression)
        {
            _computed.Add(key, "return " + jsExpression + ";");
        }

        #endregion

        #region RenderScript

        public virtual string RenderInitialize(string el)
        {
            // created event are called when render initilize
            Created();

            var writer = new StringBuilder();

            writer.AppendLine("new Vue({");
            writer.AppendFormat("  el: '#{0}',\n", el);
            writer.AppendFormat("  name: '#{0}',\n", el);

            writer.AppendLine("  created: function() {");
            writer.AppendLine("     this.$registerPageVM(this);");
            writer.AppendLine(_js.ToString());
            writer.AppendLine("  },");

            RenderBody(writer);

            return writer.ToString();
        }

        public virtual string RenderComponent(string name, string template)
        {
            var writer = new StringBuilder();

            var props = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttribute<PropAttribute>() != null)
                .Select(x => new { Prop = x.GetCustomAttribute<PropAttribute>().Name, Name = x.Name })
                .ToList();

            // checks if prop name are different from viewmodel field
            props.ForEach((x) => { if(x.Name == x.Prop) throw new ArgumentException("Vue.Prop name must be different from view model property"); });

            writer.AppendFormat("Vue.component('{0}', {{\n", name);
            writer.AppendFormat("  name: '{0}',\n", name);
            writer.AppendFormat("  template: '{0}',\n", template);
            writer.AppendFormat("  props: [{0}],\n", string.Join(", ", props.Select(x => "'" + x.Prop + "'")));

            writer.AppendLine("  created: function() {");
            writer.Append(string.Join("\n", props.Select(x => string.Format("    this.{0} = this.{1};", x.Name, x.Prop))));
            writer.AppendLine(_js.ToString());

            // only call Created method if created was override in component
            var created = GetType().GetMethod("Created");

            if(created.GetBaseDefinition().DeclaringType != created.DeclaringType)
            {
                writer.AppendLine("    this.$server('Created', [], null, this);");
            }

            writer.AppendLine("  },");

            RenderBody(writer);

            return writer.ToString();
        }

        private void RenderBody(StringBuilder writer)
        {
            var model = JObject.FromObject(this);

            writer.AppendFormat("  data: function() {{ return {0}; }},\n", JsonConvert.SerializeObject(this, _serializeSettings));
            writer.AppendLine("  methods: {");

            var methods = this.GetType().GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => !x.IsSpecialName).ToArray();

            foreach (var m in methods)
            {
                // checks if method contains Script attribute (will run before call $server)
                var attr = m.GetCustomAttribute<ScriptAttribute>(true);
                var script = attr == null ? null : attr.Code + "\n      ";

                // get all parameters without HttpPostFile parameters
                var parameters = m.GetParameters()
                    .Where(x => x.ParameterType != typeof(HttpPostedFile) && x.ParameterType != typeof(List<HttpPostedFile>))
                    .Select(x => x.Name);

                // get if any parameter are file(s)
                var upload = m.GetParameters()
                    .Where(x => x.ParameterType == typeof(HttpPostedFile) || x.ParameterType == typeof(List<HttpPostedFile>))
                    .Select(x => x.Name)
                    .FirstOrDefault() ?? "null";

                writer.AppendFormat("    '{0}': function({1}) {{\n      {2}this.$server('{0}', [{3}], {4}, this);\n    }},\n", 
                    m.Name,
                    string.Join(", ", m.GetParameters().Select(x => x.Name)),
                    script, 
                    string.Join(", ", parameters),
                    upload);
            }

            writer.Length -= 2;
            writer.AppendLine();
            writer.AppendLine("  },");
            writer.AppendLine("  computed: {");

            foreach (var c in _computed)
            {
                writer.AppendFormat("    '{0}': function() {{\n      {1}\n    }},\n",
                    c.Key,
                    c.Value);
            }

            writer.Length -= 2;
            writer.AppendLine();
            writer.AppendLine("  },");
            writer.AppendLine("  watch: {");

            // get all method marked with [Watch] attribute or ends with _Watch
            var watchs = this.GetType()
                .GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.Name.EndsWith("_Watch", StringComparison.InvariantCultureIgnoreCase) || x.GetCustomAttribute<WatchAttribute>() != null)
                .ToArray();

            foreach (var w in watchs)
            {
                var name = w.GetCustomAttribute<WatchAttribute>()?.Name ?? w.Name.Substring(0, w.Name.LastIndexOf("_"));

                writer.AppendFormat("    '{0}': {{\n      handler: function(v, o) {{ this.$server('{1}', [v, o], null, this); }},\n      deep: true\n    }},\n", 
                    name, w.Name);
            }

            writer.Length -= 2;

            writer.AppendLine();
            writer.AppendLine("  }");
            writer.AppendLine("});");
        }

        #endregion

        #region Update Model

        public virtual string UpdateModel(string model, string method, JToken[] parameters, IList<HttpPostedFile> files)
        {
            JsonConvert.PopulateObject(model, this, _serializeSettings);

            ExecuteMethod(method, parameters, files);

            return RenderUpdate(model);
        }

        protected virtual void ExecuteMethod(string name, JToken[] parameters, IList<HttpPostedFile> files)
        {
            var method = this.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
            var pars = new List<object>();
            var index = 0;

            if (method == null) throw new ArgumentNullException("Method " + name + " not found on " + this.GetType().Name + " view model or are not instance public method");

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
                    var token = parameters[index++];

                    if (token.Type == JTokenType.Object)
                    {
                        var obj = ((JObject)token).ToObject(p.ParameterType);

                        pars.Add(obj);
                    }
                    else
                    {
                        var value = ((JValue)token).Value;

                        pars.Add(Convert.ChangeType(value, p.ParameterType));
                    }
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
