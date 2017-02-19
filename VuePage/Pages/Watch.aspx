<%@ Page Language="C#" Title="Page 1" %>
<script runat="server">

    public class PageVM : Vue.ViewModel<PageVM>
    {
        public List<string> Brands { get; set; }
        public List<KeyValuePair<int, string>> Models { get; set; }

        public string BrandSelected { get; set; }
        public int? ModelSelected { get; set; }

        public PageVM()
        {
            Brands = new List<string>() { "BMW", "Ford", "Mercedes" };

            Watch(x => x.BrandSelected, (v, o) =>
            {
                Models = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(1, v + " - Model 1 (old " + o + ")"),
                    new KeyValuePair<int, string>(2, v + " - Model 2 (old " + o + ")"),
                    new KeyValuePair<int, string>(3, v + " - Model 3 (old " + o + ")")
                };
                ModelSelected = null;
            });
        }

        public void ShowSelected()
        {
            JS.Alert(ModelSelected.ToString());
        }
    }

</script>
<html>
<head runat="server"></head>
<body runat="server">

    <div id="app">

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

    </div>

</body>
</html>
