<%@ Page Language="C#" Title="Less" %>
<script runat="server">

    public class PageVM : ViewModel
    {
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>H1 - Less style</h1>
    <h2>H2 - Used @import from base</h2>


<style scoped>

    @import "~/Content/base.less";


    h1 { color: @red; border-bottom: 2px solid gray; }
    h2 { color: @blue; text-transform: uppercase; }

</style>


</asp:Content>