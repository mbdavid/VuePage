<%@ Page Title="Overlay" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public int Seconds { get; set; } = 1;
        public int Counter { get; set; }

        protected override void Created()
        {
            System.Threading.Thread.Sleep(2000);
        }

        public void Wait()
        {
            Counter++;
            System.Threading.Thread.Sleep(Seconds * 1000);
        }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    This page took 2 seconds to load first time.<br /><br />

    Counter: {{ Counter }}

    <br /><br />

    <input v-model.number="Seconds" size="3" />
    <button @click="Wait()">Click and wait {{ Seconds }} seconds</button>

</asp:Content>
