using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace Vue
{
    public class VueComponent
    {
        private static Dictionary<string, VueComponent> _components = null;

        public string Name { get; set; }
        public string Path { get; set; }

        public static void Register(string name, string virtualPath)
        {
            if (_components == null) Discovery();

            _components[name] = new VueComponent
            {
                Name = name,
                Path = virtualPath
            };
        }

        internal static IEnumerable<VueComponent> FindComponents(HttpContext context)
        {
            if (_components == null) Discovery();

            return _components.Values;
        }

        public static void Discovery(string virtualRoot = "~/Components/")
        {
            if (_components == null) _components = new Dictionary<string, VueComponent>();

            var server = HttpContext.Current.Server;
            var path = server.MapPath(virtualRoot);
            var files = Directory.GetFiles(path, "*.ascx", SearchOption.AllDirectories);
            var loader = new UserControl();

            foreach (var file in files)
            {
                var vpath = file.Replace(server.MapPath("~/"), "~/").Replace(@"\", "/");
                var control = loader.LoadControl(vpath);

                if (control == null) throw new FileNotFoundException($"Control {vpath} not found");

                var component = new VueComponent
                {
                    Name = control.GetType().Name,
                    Path = vpath
                };

                _components.Add(component.Name, component);
            }
        }
    }
}
