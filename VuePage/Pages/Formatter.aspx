<%@ Page Title="Overlay" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public decimal Money { get; set; } = 1345.99m;
        public DateTime Now { get; set; } = DateTime.Now;
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    Money: {{ Money }}<br />
    Date: {{ Now | date }}

    <hr />

    Inputdate: <input-date v-model="Now" placeholder="DateTime" />


</asp:Content>
