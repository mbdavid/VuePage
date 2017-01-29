<%@ Page Title="Default" Language="C#" MasterPageFile="~/Mobile/Mobile.master" %>
<script runat="server">

    public class PageViewModel : Vue.ViewModel<PageViewModel>
    {
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <div class="m-header">
        Pagina 1: 
        <button class="button is-primary">Enviar</button>
        <a href="#" class="button is-danger">Enviar</a>
    </div>

    <div class="m-content">

        <div style="text-align: center">
            <a class="button is-primary" href="Page1.aspx">Pagina 1</a>
        </div>

        content: <a href="Page1.aspx">Page 1</a>
        1<br />2<br />3<br />4<br />5<br />6<br />7<br />8<br />9<br />10<br />
        1<br />2<br />3<br />4<br />5<br />6<br />7<br />8<br />9<br />10<br />
        1<br />2<br />3<br />4<br />5<br />6<br />7<br />8<br />9<br />10<br />
        1<br />2<br />3<br />4<br />5<br />6<br />7<br />8<br />9<br />10<br />
        content: <a href="Page1.aspx">Page 1</a>
    </div>


</asp:Content>
