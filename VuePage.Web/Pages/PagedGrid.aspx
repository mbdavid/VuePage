<%@ Page Language="C#" Title="Paged DataSource" %>
<%@ Import Namespace="VuePage.Web" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        public Paged<Product> Products { get; set; } = new Paged<Product>();

        protected override void OnCreated()
        {
            Products_Watch();
        }

        public void Products_Watch()
        {
            Products.Load(Product.GetData());
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public static IEnumerable<Product> GetData()
        {
            for (var i = 1; i <= 225; i++)
                yield return new Product { Id = i, Name = "Product " + i, Price = i + 22 };
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Paged DataSource</h1><hr />

    <table border="1" style="width: 100%;">
        <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Price</th>
        </tr>

        <tr v-for="Item in Products.Items">
            <td>{{ Item.Id }}</td>
            <td>{{ Item.Name }}</td>
            <td style="text-align: right">{{ Item.Price | currency }}</td>
        </tr>
    </table>
    <button @click="Products.Index = 0" :disabled="Products.Index == 0">|<</button>
    <button @click="Products.Index--" :disabled="Products.Index == 0"><</button>
    Page {{ Products.Index + 1 }}/{{ Products.PageCount }}
    <button @click="Products.Index++" :disabled="Products.Index == Products.PageCount - 1">></button>
    <button @click="Products.Index = Products.PageCount - 1" :disabled="Products.Index == Products.PageCount - 1">>|</button>
    (Total: {{ Products.Count }});

    <select v-model.number="Products.PageSize">
        <option value="10">10</option>
        <option value="25">25</option>
        <option value="100">100</option>
        <option value="1000">1000</option>
    </select>



    <pre>{{$data}}</pre>

</asp:Content>