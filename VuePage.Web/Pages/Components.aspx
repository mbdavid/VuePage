<%@ Page Language="C#" Title="Home" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        public int Buttons { get; set; } = 2;
        public int Total { get; set; }
        public string Name { get; set; }

        public PageVM()
        {
            this.Created += (s, e) => Name = DateTime.Now.Second.ToString();
        }

        public void IncrementTotal()
        {
            Total++;
        }

        public void ShowName()
        {
            JS.Alert("Name = " + this.Name);
            this.Name = "";
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Components</h1><hr />

    <input type="text" v-model="Name" />
    <button @click="ShowName()">ShowName</button>
    <hr />
    Clicks: {{ Total }}
    <counter v-for="i in Buttons" @inc="IncrementTotal()"></counter>
    <br />
    <button @click="Buttons++">+</button>
    <button @click="Buttons--">-</button>
    <hr />
    <pre>{{$data}}</pre>

    <inputbox Name="Initial value"></inputbox>
    <hr />
    <inputbox Name="Mauricio"></inputbox>

</asp:Content>