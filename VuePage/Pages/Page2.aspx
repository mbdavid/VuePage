<%@ Page Title="Page 2" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string Now => DateTime.Now.ToString();
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <a href="Page1.aspx" data-transition="slide-right"><< Page1</a><hr />

    <h2>Page 2</h2>
    <hr />

    Now From Server: {{ Now }}

    <hr />
    <a href="Page1.aspx#restore">Back to Page1 with restore state</a><br />
    <a href="Page1.aspx">Back to Page1 with no restore</a><br />

</asp:Content>
