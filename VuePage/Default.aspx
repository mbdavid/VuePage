<%@ Page Title="Default" Language="C#" %>
<script runat="server">

    public class PageViewModel : Vue.ViewModel<PageViewModel>
    {
        public List<string> Files { get; set; }
        public bool Show { get; set; } = true;

        public PageViewModel(HttpContext ctx)
        {
            if(!IsPost)
            {
                var files = System.IO.Directory.GetFiles(ctx.Server.MapPath("~/Pages"), "*.aspx");
                this.Files = new List<string>(files.Select(x => System.IO.Path.GetFileNameWithoutExtension(x)));
                //JS.Code("console.log('THIS (in created())', this);");
            }
        }

        public void FromServer()
        {
            JS.Code("console.log('THIS (in method)', this);");
        }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    You have {{Files.length}} example pages: <br />

    <input type="button" v-on:click="Show = !Show" :value="Show ? 'Hide' : 'Show'" />

    <ul v-show="Show">
        <li v-for="file in Files">
            <a :href="'/Pages/' + file + '.aspx'">{{file}}</a>
        </li>
    </ul>

    <hr />
    <button @click.prevent="FromServer()">"this" FromServer()</button>

</asp:Content>
