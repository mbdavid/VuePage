using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public class VueContext
    {
        private HttpContext _context;
        private Dictionary<string, string> _computed = new Dictionary<string, string>();
        private Dictionary<string, Action<object, object>> _watch = new Dictionary<string, Action<object, object>>();
        private Dictionary<string, Action<object[]>> _methods = new Dictionary<string, Action<object[]>>();

        public VueContext(HttpContext context)
        {
            _context = context;
        }

        #region Watch

        protected virtual void NotifyWatch(string name, object value, object oldValue)
        {
            Action<object, object> action;

            if (_watch.TryGetValue(name, out action))
            {
                action(value, oldValue);
            }
        }

        protected void Watch<TViewModel, TResult>(Expression<Func<TViewModel, TResult>> expr, Action<TResult, TResult> action)
        {
            _watch.Add(expr.KeyPath(), (n, o) => action((TResult)Convert.ChangeType(n, typeof(TResult)), (TResult)Convert.ChangeType(o, typeof(TResult))));
        }

        protected void Watch<TViewModel, TResult>(Expression<Func<TViewModel, TResult>> expr, Action<TResult> action)
        {
            _watch.Add(expr.KeyPath(), (n, o) => action((TResult)Convert.ChangeType(n, typeof(TResult))));
        }

        protected void Watch<TViewModel, TResult>(Expression<Func<TViewModel, TResult>> expr, Action action)
        {
            _watch.Add(expr.KeyPath(), (n, o) => action());
        }

        protected void Watch(string keyPath, Action<object, object> action)
        {
            _watch.Add(keyPath, action);
        }

        #endregion

        #region Computed

        protected void Computed<TViewModel>(string key, Expression<Func<TViewModel, object>> expr)
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

        public void Method(string name, string jsCode)
        {
        }

        public bool IsPost { get; private set; }

        internal void Render(StringBuilder writer)
        {
        }
    }
}
