<%@ Page Language="C#" Title="Mixin" %>
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

    <h1>Mixin</h1>

    <div id="app">

        <button @click="ServerClick()">Run from server</button>
        <button @click="ClientClick()">Run from javascript [1]</button>

    </div>

</asp:Content>
<asp:Content ContentPlaceHolderID="footer" runat="server">

<script>

    window["ext"] = {
        methods: {
            ClientClick: function () {
                alert("It´s from client only script [1]");
            }
        }
    }

</script>

</asp:Content>