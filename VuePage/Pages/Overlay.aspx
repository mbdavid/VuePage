<%@ Page Title="Overlay" Language="C#" %>
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

    <h1>Custom Disabled control in Ajax</h1>

    <input type="button" @click="Wait(2)" value="Wait(2s)" />
    <input type="button" @click="Wait(2)" v-overlay value="Wait(2s) [v-overlay]" />
    <input type="text" @change="Wait(1)" v-overlay placeholder="Wait(1s) [v-overlay]" />

</asp:Content>
