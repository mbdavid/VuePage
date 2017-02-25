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
using Newtonsoft.Json.Serialization;

namespace Vue
{
    public class ViewModel
    {
        private JavascriptBuilder _js = new JavascriptBuilder();
        private JsonSerializerSettings _serializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ContractResolver = VueContractResolver.Instance
        };

        protected JavascriptBuilder JS { get { return _js; } }

        public ViewModel()
        {
        }

        #region Computed

        /// <summary>
        /// Resolve an expression to convert into a computed field
        /// </summary>
        public static Computed Resolve<T>(Expression<Func<T, object>> expr) where T : ViewModel
        {
            return new Computed
            {
                Code = JavascriptExpressionVisitor.Resolve(expr),
                Value = (object o) => expr.Compile()
            };
        }

        #endregion

        #region Created

        public event EventHandler Created;

        /// <summary>
        /// In page call during initialize. In component, made ajax call when component are created
        /// </summary>
        public virtual void OnCreated()
        {
            if (Created != null)
            {
                Created(this, EventArgs.Empty);
            }
        }

        #endregion

        #region RenderScript

        public virtual string RenderInitialize(string el)
        {
            // created event are called when render initilize
            OnCreated();

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
            props.ForEach((x) => { if(x.Name == x.Prop) throw new ArgumentException("[Vue.Prop] name must be different from view model property"); });

            writer.AppendFormat("Vue.component('{0}', {{\n", name);
            writer.AppendFormat("  name: '{0}',\n", name);
            writer.AppendFormat("  template: '{0}',\n", template);
            writer.AppendFormat("  props: [{0}],\n", string.Join(", ", props.Select(x => "'" + x.Prop + "'")));

            writer.AppendLine("  created: function() {");
            writer.Append(string.Join("\n", props.Select(x => string.Format("    this.{0} = this.{1};", x.Name, x.Prop))));
            writer.AppendLine(_js.ToString());

            // only call Created method if created was override in component
            var created = GetType().GetMethod("OnCreated");

            if(Created != null || created.GetBaseDefinition().DeclaringType != created.DeclaringType)
            {
                writer.AppendLine("    this.$server('OnCreated', [], null, this);");
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

            var computed = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Computed));

            foreach (var c in computed)
            {
                writer.AppendFormat("    '{0}': function() {{\n      return ({1})(this);\n    }},\n",
                    c.Name,
                    ((Computed)c.GetValue(this)).Code);
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

        internal string UpdateModel(string model, string method, JToken[] parameters, IList<HttpPostedFile> files)
        {
            JsonConvert.PopulateObject(model, this, _serializeSettings);

            ExecuteMethod(method, parameters, files);

            return RenderUpdate(model);
        }

        private void ExecuteMethod(string name, JToken[] parameters, IList<HttpPostedFile> files)
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

            OnExecuteMethod(method, pars.ToArray());
        }

        /// <summary>
        /// Call viewmodel method using paramters from client
        /// </summary>
        protected virtual void OnExecuteMethod(MethodInfo method, object[] args)
        {
            method.Invoke(this, args);
        }

        private string RenderUpdate(string model)
        {
            var original = JObject.Parse(model);
            var current = JObject.FromObject(this, new JsonSerializer { ContractResolver = VueContractResolver.Instance });
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

        #region Factory Instance

        /// <summary>
        /// Create new instance based on ViewModel type
        /// </summary>
        internal static ViewModel Load(Type viewModelType, HttpContext context)
        {
            var ctor = viewModelType.GetConstructors().First();
            var parameters = new List<object>();

            // commom ctor parameters
            foreach (var par in ctor.GetParameters())
            {
                if (par.ParameterType == typeof(HttpContext)) parameters.Add(context);
                else if (par.ParameterType == typeof(HttpRequest)) parameters.Add(context.Request);
                else if (par.ParameterType == typeof(HttpResponse)) parameters.Add(context.Response);
                else if (par.ParameterType == typeof(NameValueCollection)) parameters.Add(context.Request.Params);
                else if (typeof(IPrincipal).IsAssignableFrom(par.ParameterType)) parameters.Add(context.User);
                else throw new SystemException("ViewModel contains unknown ctor parameter: " + par.Name);
            }

            return (ViewModel)Activator.CreateInstance(viewModelType, parameters.ToArray());
        }

        #endregion

    }
}
