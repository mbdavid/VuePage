<%@ Page Title="Page 1" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {
        public string CurrentText { get; set; } = "";
        public string Filtro { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        public VM()
        {
            Computed("First", x => x.Items.Count == 0 ? "[none]" : x.Items[0].Text);
            Computed("FirstSelected", x => x.Items.Where(z => z.Done).Count() == 0 ? "[none]" : x.Items.Where(z => z.Done).First().Text);
            Computed("FirstSel", x => x.Items.First(z => z.Done));

            // Initial values (will be replaced when post)
            Items.Add(new Todo { Text = "My first demo" });
            Items.Add(new Todo { Text = "Was done", Done = true });

            //Watch(x => x.CurrentText, () => JS.Alert("alterado: " + CurrentText));

            Computed("Filtrado", x => x.Items.Where(z => z.Text.Contains(x.Filtro)));

        }

        public void Add()
        {
            Items.Add(new Todo { Text = CurrentText, Done = false });
            CurrentText = "";
        }

        public void Remove(int index)
        {
            Items.RemoveAt(index);
        }

        [Vue.Confirm("Clear all items?")]
        public void Clear()
        {
            Items.Clear();
        }
    }

    public class Todo
    {
        public string Text { get; set; }
        public bool Done { get; set; }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="server">

    <a href="Page2.aspx" data-transition="slide-left">Move To Page2 >></a><hr />

    <h2>Page 1</h2>
    <hr />

    <input type="text" v-model="CurrentText" autofocus/>
    <button v-on:click="Add()" :disabled="!CurrentText" type="button" v-default-enter>Add</button>
    <button v-on:click="Clear()" type="button">Clear</button>
    <hr />
    <ul>
        <li v-for="(Item, i) in Items">
            <input type="checkbox" v-model="Item.Done" />
            <span :style="{ 'text-decoration': Item.Done ? 'line-through' : 'none' }" @click="Item.Done = !Item.Done">
                {{ Item.Text }}
            </span>
            <button v-on:click.prevent="Remove(i)" :disabled="Item.Done" type="submit">X</button>
        </li>
    </ul>
    <hr />
    Filtrado:
    <input type="text" v-model="Filtro" />
    <ul>
        <li v-for="(Item, i) in Filtrado">{{ Item.Text | uppercase }}</li>
    </ul>


    <pre style="border: 1px solid red; padding: 5px; margin: 15px;">{{$data}}</pre>
    First: {{First}}<br />
    FirstSelected: {{FirstSelected}}<br />
    FirstSelected: {{FirstSel}}<br />

</asp:Content>
