using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    /// <summary>
    /// Define javascript code before call $server() method
    /// </summary>
    public class ScriptAttribute : Attribute
    {
        /// <summary>
        /// Execute script before call $server() method
        /// </summary>
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

    /// <summary>
    /// Define ViewModel id element in page level view model
    /// </summary>
    public class ElementAttribute : Attribute
    {
        public string Id { get; set; }

        public ElementAttribute(string id)
        {
            Id = id;
        }
    }

    public class PropAttribute : Attribute
    {
        public string Name { get; set; }

        /// <summary>
        /// Define name for this property (do not use same name as variable)
        /// </summary>
        public PropAttribute(string name)
        {
            Name = name;
        }
    }

    public class WatchAttribute : Attribute
    {
        public string Name { get; set; }

        /// <summary>
        /// Define variable name to subscribe for changes
        /// </summary>
        public WatchAttribute(string name)
        {
            Name = name;
        }
    }
}
