using System.Linq.Expressions;
using System.Web;

namespace Vue
{
    public static class ExpressionExtension
    {
        public static string KeyPath(this Expression expr)
        {
            var str = expr.ToString(); // gives you: "o => o.Whatever"
            var firstDelim = str.IndexOf('.'); // make sure there is a beginning property indicator; the "." in "o.Whatever" -- this may not be necessary?
            var path = firstDelim < 0 ? str : str.Substring(firstDelim + 1).TrimEnd(')');

            return path;
        }
    }
}
