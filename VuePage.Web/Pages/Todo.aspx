<%@ Page Language="C#" Title="Todo" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        public string CurrentText { get; set; } = "";
        public string Filtro { get; set; } = "";
        public List<Todo> Items { get; set; } = new List<Todo>();

        public Computed First = Resolve<PageVM>(x => x.Items.Count == 0 ? "[none]" : x.Items[0].Text);
        public Computed FirstSelected = Resolve<PageVM>(x => x.Items.Where(z => z.Done).Count() == 0 ? "[none]" : x.Items.Where(z => z.Done).First().Text);
        public Computed FirstSel = Resolve<PageVM>(x => x.Items.First(z => z.Done));
        public Computed Filtrado = Resolve<PageVM>(x => x.Items.Where(z => z.Text.Contains(x.Filtro)));

        public PageVM()
        {
            // Initial values (will be replaced when post)
            Items.Add(new Todo { Text = "My first demo" });
            Items.Add(new Todo { Text = "Was done", Done = true });
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

        [Confirm("Clear all items?")]
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
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Todo List</h1><hr />

    <input type="text" v-model="CurrentText" autofocus/>
    <button v-on:click="Add()" :disabled="!CurrentText" type="button">Add</button>
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
        <li v-for="(Item, i) in Filtrado">{{ Item.Text }}</li>
    </ul>


    <pre>{{$data}}</pre>
    First: {{First}}<br />
    FirstSelected: {{FirstSelected}}<br />
    FirstSelected: {{FirstSel}}<br />

</asp:Content>