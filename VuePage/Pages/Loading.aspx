<%@ Page Language="C#" Title="Loading" %>
<script runat="server">

    public class PageVM : Vue.ViewModel<PageVM>
    {
        public void Wait(int s)
        {
            System.Threading.Thread.Sleep(s * 1000);
        }
    }

</script>
<html>
<head runat="server"></head>
<body runat="server">

    <style>
        .is-loading { 
            position: relative;
        }
        .is-loading:before {
            position: absolute;
            left: 0; right: 0; top: 0; bottom: 0;
            background-color: rgba(255, 0, 0, .5);
            content: ' ';
        }

    </style>
    <script>
        // Global overlay (if not preset in element)
        Vue.$loading.register(500, function (target) {
            target.classList.add('is-loading');
            target.disabled = true;
            return function () {
                target.classList.remove('is-loading');
                target.disabled = false;
            }
        });

    </script>

    <div id="app">

        <h1>Loading</h1>

        <form @submit.prevent="Wait(5)" style="border: 1px solid blue; padding: 10px;">

            <button type="button" @click="Wait(3)">Wait(3s)</button>
            <input type="text" @change="Wait(3)" placeholder="OnChange Wait(3s)" />
            <br /><br />
            <button type="submit">Submit</button>

        </form>

    </div>

</body>
</html>
