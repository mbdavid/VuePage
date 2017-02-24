<%@ Page Language="C#" Title="Watch" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        public List<string> Brands { get; set; }
        public List<KeyValuePair<int, string>> Models { get; set; }

        public string BrandSelected { get; set; }
        public int? ModelSelected { get; set; }

        public PageVM()
        {
            Brands = new List<string>() { "BMW", "Ford", "Mercedes" };
        }

        public void BrandSelected_Watch(string value, string old)
        {
            Models = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1, value + " - Model 1 (old " + old + ")"),
                new KeyValuePair<int, string>(2, value + " - Model 2 (old " + old + ")"),
                new KeyValuePair<int, string>(3, value + " - Model 3 (old " + old + ")")
            };
            ModelSelected = null;
        }

        [Watch("ModelSelected")]
        public void ModelChanged()
        {
            JS.Alert("Model changed");
        }

        public void ShowSelected()
        {
            JS.Alert(ModelSelected.ToString());
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Watch</h1><hr />

    Brand:
    <select v-model="BrandSelected">
        <option v-for="option in Brands" :value="option">{{ option }}</option>
    </select>
    - Selected: {{ BrandSelected }}
    <br /><br />
    Model:
    <select v-model="ModelSelected">
        <option v-for="option in Models" :value="option.Key">{{ option.Value }}</option>
    </select>
    - Selected: {{ ModelSelected }}
    <hr />
    <button @click.prevent="ShowSelected()">ShowSelected</button>

</asp:Content>