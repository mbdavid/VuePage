<%@ Page Language="C#" Title="Mixin" %>
<script runat="server">

    [Mixin("ext")]
    public class PageVM : ViewModel
    {
        public void ServerClick()
        {
            JS.Alert("From Server");
        }
    }

</script>
<html>
<head runat="server"></head>
<body>

    <a href="/">Home</a><hr />

    <h1>Mixin [2] - Reusing same variable name</h1><hr />

    <div id="app">

        <button @click="ServerClick()">Run from server</button>
        <button @click="ClientClick()">Run from javascript [2]</button>

    </div>


    <script>

        window["ext"] = {
            methods: {
                ClientClick: function () {
                    alert("It´s from client only script [2]");
                }
            }
        }

    </script>

</body>
</html>