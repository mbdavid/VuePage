<%@ Page Language="C#" %>
<script runat="server">

    public class PageViewModel : Vue.ViewModel<PageViewModel>
    {
        public List<string> Files { get; set; }
        public bool Show { get; set; }

        public PageViewModel()
        {
            var files = System.IO.Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Pages"), "*.aspx");

            Files = new List<string>(files.Select(x => System.IO.Path.GetFileNameWithoutExtension(x)));
            Show = true;
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

</asp:Content>
