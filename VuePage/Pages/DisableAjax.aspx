<%@ Page Title="Page 2" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public void Wait(int s)
        {
            System.Threading.Thread.Sleep(s * 1000);
        }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <button type="button" @click="Wait(0)">Wait(0)</button>
    <button type="button" @click="Wait(1)">Wait(1s)</button>
    <button type="button" @click="Wait(2)">Wait(2s)</button>
    <input type="text" @change="Wait(1)" placeholder="onchange(1)" />


</asp:Content>
