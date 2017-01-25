<%@ Page Title="Page 1" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string Now => DateTime.Now.ToString();
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <h2>Page 2</h2>
    <hr />

    Now From Server: {{ Now }}

    <hr />
    <a href="Page1.aspx#restore">Back to Page1 with restore state</a><br />
    <a href="Page1.aspx">Back to Page1 with no restore</a><br />

</asp:Content>
