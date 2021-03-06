﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    /// <summary>
    /// Define javascript code before call $server() method
    /// </summary>
    public class PreScriptAttribute : Attribute
    {
        /// <summary>
        /// Execute script before to be call before $server() method
        /// </summary>
        public PreScriptAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    /// <summary>
    /// Define javascript code to be call after $server() method
    /// </summary>
    public class PostScriptAttribute : Attribute
    {
        /// <summary>
        /// Execute script after call $server() method
        /// </summary>
        public PostScriptAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    public class ConfirmAttribute : PreScriptAttribute
    {
        public ConfirmAttribute(string text)
            : base("if (confirm('" + text + "') === false) return false;")
        {
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

    /// <summary>
    /// Test is user are authenticated to call ViewModel decorated with [Authorize]
    /// </summary>
    public class AuthenticateAttribute : Attribute
    {
        public string RedirectTo { get; set; }

        public AuthenticateAttribute()
        {
        }

        /// <summary>
        /// If not authenticated, redirect to this url
        /// </summary>
        public AuthenticateAttribute(string redirectTo)
        {
            RedirectTo = redirectTo;
        }
    }

    /// <summary>
    /// Validate if user contains role for this Method call, using User.IsUserInRole()
    /// </summary>
    public class RoleAttribute : Attribute
    {
        public string[] Roles { get; set; }

        public RoleAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
