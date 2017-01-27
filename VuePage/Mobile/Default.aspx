<%@ Page Title="Default" Language="C#" MasterPageFile="~/Mobile/Mobile.master" %>
<script runat="server">

    public class PageViewModel : Vue.ViewModel<PageViewModel>
    {
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <div class="m-header">
        toolbar
    </div>

    <div class="m-content">
        toolbar
    </div>
    <div class="m-toolbar">
        toolbar
    </div>

</asp:Content>
