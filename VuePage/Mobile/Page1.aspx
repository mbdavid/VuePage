<%@ Page Title="Page1" Language="C#" MasterPageFile="~/Mobile/Mobile.master" %>
<script runat="server">

    public class PageViewModel : Vue.ViewModel<PageViewModel>
    {
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <p>Page 1</p>
    <hr />
    <a href="Default.aspx" data-transition="slide-right">Home</a>

</asp:Content>
