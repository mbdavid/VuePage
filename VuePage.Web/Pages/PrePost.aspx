<%@ Page Language="C#" Title="Mixin" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        [PreScript("alert('pre'); console.log('pre this', this);")]
        [PostScript("alert('post'); console.log('post this', this);")]
        public void ServerClick()
        {
            System.Threading.Thread.Sleep(1000);
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <button @click="ServerClick()">Run from server</button>

</asp:Content>
