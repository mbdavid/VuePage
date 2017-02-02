<%@ Page Title="Script Block" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string Now => DateTime.Now.ToString();
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="server">

    <h2>Script Block</h2>
    <hr />

    See console :: {{ Now }}

    <script>
        console.log('From page inside an script block');
    </script>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.1.1.min.js">content-jquery</script>
    <script>
        console.log('jQuery is installed?', $);
    </script>

    <style>
        h2 { color: red; }
    </style>

</asp:Content>
