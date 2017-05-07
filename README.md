> VuePage now is DotVue - https://github.com/mbdavid/DotVue

# VuePage

Implement server-side of ViewModel for `Vue` on WebForm page.

```C#
public class PageVM : Vue.ViewModel
{
    public Username { get; set; }
    public Password { get; set; }
    public Message { get; set; }

    public void Login()
    {
        Message = AuthServie.Login(Username, Password);
    }
    
    public void Username_Watch(string value, string old)
    {
        // observe changes from client
    }
}
```

```HTML
<vue:App runat="server">
    Username: <input type="text" v-model.lazy="Username" /><br/>
    Password: <input type="password" v-model="Password" /><br/>
    <button type="button" v-on:click="Login()">Login</button>
    <hr/>
    {{ Message }}
    
    <style scoped>
        button { ... }
    </style>
    <script>
    
        // vue mixin
        return {
            methods: {
                showError: function() {
                   // ..
                }
            }
        }
    
    </script>
    
    
</vue:App>
```

- Support Vue `Watch` server-side
- Support lambda expression to be converted to `Computed` javascript 
    `Test(x => x.Items.Where(z => z.Done).Count() > 3, "x.Items.filter(function(z) { return z.Done; }).length > 3;");`
- Support history navigation (ajax-get) between ASPX pages
- Support UserControl as Vue.Component
- Support multi vue instances
- Support file upload in view model
- Support MasterPage
- Works with no `<form runat="server">`


# TODO
- ExecuteMethod test paramteres values
- Cache ViewModel definition (methods, watch, ...)
- Cache handler
- [Authorize] attribute - checks HttpContext.User.Identity.IsAuthenticated. Support redirect???
- [Role("admin", "user")] - checkes HttpContext.User.IsInRole()

- Permitir escrever, em javascript (e não no server) codigo pra ViewModel. Tipo, quero escrever o metodo:

[Mixin("vm")]
public class PageVM : ViewModel
{
}

window["vm"] = {
    methods: {
        PermiteExcluir: function(item) { .... }
    }
}

    
# VuePage no .NET Core

- Rodar em MVC > VueCore | VueMvc | VueAspNet
- Controller padrão?
- Como criar rotas

\Views
    - .cshtml
\ViewModels
    - .cs
    


app.UseVue()
