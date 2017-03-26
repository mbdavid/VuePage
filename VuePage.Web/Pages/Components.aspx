<%@ Page Language="C#" Title="Home" %>
<script runat="server">

    public class PageVM : ViewModel
    {
        public int Buttons { get; set; } = 2;
        public int Total { get; set; }
        public string Name { get; set; }

        protected override void OnCreated()
        {
            Name = DateTime.Now.Second.ToString();
        }

        public void IncrementTotal()
        {
            Total++;
        }

        public void ShowName()
        {
            JS.Alert("Name = " + this.Name);
            this.Name = "";
        }
    }

</script>
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Components</h1><hr />

    <input type="text" v-model="Name" />
    <button @click="ShowName()">ShowName</button>
    <hr />
    Clicks: {{ Total }}
    <counter v-for="i in Buttons" @inc="IncrementTotal()" />
    <br />
    <button @click="Buttons++">+</button>
    <button @click="Buttons--">-</button>
    <hr />
    <pre>{{$data}}</pre>

    <inputbox Name="Initial value" />
    <hr />
    <inputbox Name="Mauricio" />

    <script>

        function loadComponent(name) {
            return function (resolve, reject) {
                var xhr = new XMLHttpRequest();

                xhr.onload = function () {
                    if (xhr.status < 200 || xhr.status >= 400) {
                        alert('Error on load component: ' + name);
                        return;
                    }

                    var c = new Function(xhr.responseText);

                    resolve(c());
                };

                //log('$loadComponent ("' + name + '")');

                xhr.open('GET', location.pathname + '?component=' + name, true);
                xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
                xhr.send();
            }
        }


        return {
            components: {
                counter: loadComponent('Counter'),
                inputbox: loadComponent('InputBox')
            }
        }



    </script>


</asp:Content>