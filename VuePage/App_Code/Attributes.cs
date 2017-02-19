using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public class ScriptAttribute : Attribute
    {
        public ScriptAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    public class ConfirmAttribute : ScriptAttribute
    {
        public ConfirmAttribute(string text)
            : base("if (confirm('" + text + "') === false) return false;")
        {
        }
    }

    public class PropAttribute : Attribute
    {
        public string Name { get; set; }

        public PropAttribute(string name)
        {
            Name = name;
        }
    }
}
