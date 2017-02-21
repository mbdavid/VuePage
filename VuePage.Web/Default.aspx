﻿<%@ Page Language="C#" Title="Home" %>
<script runat="server">

    public class PageVM : Vue.ViewModel
    {
        public List<string> Files { get; set; }
        public bool Show { get; set; } = true;

        public PageVM(HttpContext ctx)
        {
            var files = System.IO.Directory.GetFiles(ctx.Server.MapPath("~/Pages"), "*.aspx");
            this.Files = new List<string>(files.Select(x => System.IO.Path.GetFileNameWithoutExtension(x)));
        }
    }

</script>
<html>
<head runat="server"></head>
<body runat="server">

    <div id="app">

        <h1>VuePage - Demos</h1><hr />

        You have {{Files.length}} example pages: <br />

        <input type="button" v-on:click="Show = !Show" :value="Show ? 'Hide' : 'Show'" />

        <ul v-show="Show">
            <li v-for="file in Files">
                <a :href="'/Pages/' + file + '.aspx'">{{file}}</a>
            </li>
        </ul>

    </div>

</body>
</html>
