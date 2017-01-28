<%@ Page Title="Page 2" Language="C#" %>
<script runat="server">

    public void Page_Load(object sender, EventArgs e)
    {
        vm.Mount<VM>();
    }

    public class VM : Vue.ViewModel<VM>
    {
        public void Wait(int s)
        {
            System.Threading.Thread.Sleep(s * 1000);
        }
    }

</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title><%= Page.Title %></title>
    <link rel="stylesheet" href="/Layout/vue.css" />
</head>
<body>

    <vue:App runat="server" ID="vm" Delay="10" ShowOverlay="false">

        <h1>Custom Disabled control in Ajax</h1>

        <button type="button" @click="Wait(0)">Wait(0)</button>
        <button type="button" @click="Wait(1)">Wait(1s)</button>
        <button type="button" @click="Wait(2)">Wait(2s)</button>
        <input type="text" @change="Wait(1)" placeholder="onchange(1)" />

    </vue:App>

    <script src="/Scripts/vue.js"></script>
    <script src="/Scripts/vue-page.js?_=<%= new Random().Next(0, 10000) %>"></script>

    <script>
        Vue.$loading(function (target, el) {
            if (target) target.disabled = true;
            return function () {
                if (target) target.disabled = false;
            }

        });

    </script>
</body>
</html>