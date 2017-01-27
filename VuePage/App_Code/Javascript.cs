using System.Text;

namespace Vue
{
    public class Javascript
    {
        private StringBuilder _sb = new StringBuilder();

        public Javascript ConsoleLog(string text)
        {
            return Code("console.log('{0}');", text);
        }

        public Javascript Alert(string text)
        {
            return Code("alert('{0}');", text);
        }

        public Javascript Focus(string id)
        {
            return Clouser("try {{ var f = document.querySelector('.vue-page-active #{0}'); if (f) {{ f.focus(); }} }} catch(e) {{ }}", id);
        }

        public Javascript NavigateTo(string url)
        {
            return Code("navToPage('{0}');", url);
        }

        public Javascript RedirectTo(string url)
        {
            return Code("location.href = '{0}';", url);
        }

        public Javascript Clouser(string code)
        {
            _sb.Append("(function(){");
            _sb.Append(code);
            _sb.Append("})();");
            return this;
        }

        public Javascript Clouser(string format, object args)
        {
            _sb.Append("(function(){");
            _sb.AppendFormat(format, args);
            _sb.Append("})();");
            return this;
        }

        public Javascript Code(string code)
        {
            _sb.Append(code);
            return this;
        }

        public Javascript Code(string format, params object[] args)
        {
            _sb.AppendFormat(format, args);
            return this;
        }

        public override string ToString()
        {
            if(_sb.Length == 0) return "";

            return "(function(){" + _sb.ToString() + "})();";
        }
    }
}
