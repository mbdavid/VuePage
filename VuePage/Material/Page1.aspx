<%@ Page Title="Page1" Language="C#" MasterPageFile="~/Material/Material.master" %>
<script runat="server">

    public class PageViewModel : Vue.ViewModel<PageViewModel>
    {
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <p>Page 1</p>
    <hr />
    <div style="text-align: center">
        <a class="button is-primary" href="Default.aspx#restore" data-transition="slide-right">Home</a>
    </div>

</asp:Content>
