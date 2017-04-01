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
        public string Name { get; set; }
        public string Path { get; set; }

        internal static IEnumerable<VueComponent> FindComponents(HttpContext context)
        {
            var dir = context.Server.MapPath("~/Components/");
            var files = Directory.GetFiles(dir, "*.ascx", SearchOption.TopDirectoryOnly);
            var loader = new UserControl();

            foreach (var name in files.Select(x => System.IO.Path.GetFileNameWithoutExtension(x)))
            {
                var path = "~/Components/" + name + ".ascx";
                var control = loader.LoadControl(path);

                yield return new VueComponent { Name = control.GetType().Name, Path = path };
            }
        }
    }
}
