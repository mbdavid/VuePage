<%@ Page Title="CtorParam" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string Url { get; set; } = "";
        public string QueryString { get; set; }
        public string User { get; set; }

        public VM(HttpRequest req, System.Security.Principal.IPrincipal user)
        {
            Url = req.Url.ToString();
            QueryString = req.QueryString.ToString();
            User = user.Identity.Name;
        }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    ViewModel using custom Ctor paramters:
    <hr />
    Can use: HttpContext, HttpRequest, HttpResponse, NameValueCollection (Request.Params), IPrincipal
    <hr />

    <pre>{{$data}}</pre>

    <hr />


</asp:Content>
