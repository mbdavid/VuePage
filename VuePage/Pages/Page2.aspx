<%@ Page Title="Page 2" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string Now => DateTime.Now.ToString();

        public VM()
        {
            //if(!IsPost) System.Threading.Thread.Sleep(2000);
        }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="server">

    <a href="Page1.aspx" data-transition="slide-right"><< Back To Page1</a><hr />

    <h2>Page 2</h2>
    <hr />

    Now From Server: {{ Now }}

    <hr />
    <a href="Page1.aspx#restore" data-transition="slide-right">Back to Page1 with restore state (with Transition)</a><br />
    <a href="Page1.aspx#restore">Back to Page1 with restore state (without Transition)</a><br />
    <a href="Page1.aspx">Back to Page1 with no restore</a><br />

</asp:Content>
