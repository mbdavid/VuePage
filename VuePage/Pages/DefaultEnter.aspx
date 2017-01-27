<%@ Page Title="Default Enter" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string Clicked { get; set; }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <input type="text" placeholder="Fire B1" @keydown.enter="Clicked='B1 from input'" />
    <input type="button" value="B1" @click="Clicked='B1'" />
    <br />

    <input type="text" autofocus />
    <input type="button" value="B2" @click="Clicked='B2'" />
    <br />

    <input type="text" />
    <input type="button" value="B3" @click="Clicked='B3'" />

    <hr />
    <a href="javascript:;">Cancel</a>
    <input type="button" value="Main Default" v-default-enter @click="Clicked='default-button'" />

    Clicked: {{ Clicked }}

</asp:Content>
