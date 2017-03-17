<%@ Page Language="C#" Title="Mixin2" %>
<script runat="server">

    [Mixin("ext")]
    public class PageVM : ViewModel
    {
        public void ServerClick()
        {
            JS.Alert("From Server");
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Mixin [2] - Reusing same variable name</h1>

    <div id="app">

        <button @click="ServerClick()">Run from server</button>
        <button @click="ClientClick()">Run from javascript [2]</button>

    </div>

</asp:Content>
<asp:Content ContentPlaceHolderID="footer" runat="server">

    <script>

        window["ext"] = {
            methods: {
                ClientClick: function () {
                    alert("It´s from client only script [2]");
                }
            }
        }

    </script>

</asp:Content>