using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace Vue
{
    /// <summary>
    /// Register new Vue Component
    /// </summary>
    public class Component
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public Type ViewModelType { get; set; }
        public bool Inline { get; set; }

        internal static Dictionary<string, Component> All { get; private set; } = new Dictionary<string, Component>();

        /// <summary>
        /// Register new vue component. Use Global.asax Application_Start section. If viewModelType is null, autodetect in child user control inner class
        /// </summary>
        public static void Register(string name, string url, Type viewModelType = null)
        {
            lock(All)
            {
                All[name] = new Component
                {
                    Name = name,
                    Url = url,
                    ViewModelType = viewModelType,
                    Inline = viewModelType == null
                };
            }
        }
    }
}
