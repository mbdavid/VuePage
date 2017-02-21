<%@ Page Language="C#" Title="Page 1" %>
<script runat="server">

    [Vue.Element("app1")]
    public class Page1VM : Vue.ViewModel
    {
        public string Text { get; set; } = "Initial 1";
    }

    [Vue.Element("app2")]
    public class Page2VM : Vue.ViewModel
    {
        public string Text { get; set; } = "Initial 2";

        public void Info()
        {
            JS.Alert("Text = " + Text);
        }
    }

    [Vue.Element("app3")]
    public class Page3VM : Vue.ViewModel
    {
        public string Text { get; set; } = "John Doe 3";
    }

</script>
<html>
<head runat="server"></head>
<body runat="server">

    <div id="app1">
        <h1>Context 1</h1><hr />
        <input type="text" v-model="Text" />{{ Text }}
    </div>

    <div id="app2">
        <h1>Context 2</h1><hr />
        <input type="text" v-model="Text" />{{ Text }}<br />
        <button @click="Info()">Info</button>
    </div>

    <div id="app3">
        <h1>Context 3</h1><hr />
        <input type="text" v-model="Text" />{{ Text }}
    </div>

</body>
</html>