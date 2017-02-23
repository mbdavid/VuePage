<%@ Page Language="C#" Title="Edit Grid" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        public List<Item> Items { get; set; } = new List<Item>();

        public void Add()
        {
            Items.Add(new Item { Age = 35, Sex = 'M' });
        }

        public void Delete(int i)
        {
            Items.RemoveAt(i);
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public char Sex { get; set; }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Grid Items Editable</h1><hr />

    <button @click="Add()">+</button>

    <table border="1" v-if="Items.length > 0">
        <tr v-for="(Item, i) in Items">
            <td>
                <input type="text" v-model="Item.Name" />
            </td>
            <td>
                <input type="text" v-model.number="Item.Age" style="width: 50px" />
            </td>
            <td>
                <label><input type="radio" v-model="Item.Sex" :name="'Sex'+i" value="M" />M</label>
                <label><input type="radio" v-model="Item.Sex" :name="'Sex'+i" value="F" />F</label>
            </td>
            <td>
                <button @click="Delete(i)">X</button>
            </td>
        </tr>
    </table>

    <pre>{{$data}}</pre>

</asp:Content>