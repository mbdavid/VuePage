<%@ Page Language="C#" Title="Loading" %>
<script runat="server">

    public class PageVM : Vue.ViewModel
    {
        public void Wait(int s)
        {
            System.Threading.Thread.Sleep(s * 1000);
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Loading</h1>

    <form @submit.prevent="Wait(5)" style="border: 1px solid blue; padding: 10px;">

        <button type="button" @click="Wait(3)">Wait(3s)</button>
        <input type="text" @change="Wait(3)" placeholder="OnChange Wait(3s)" />
        <br /><br />
        <button type="submit">Submit</button>

    </form>

</asp:Content>
