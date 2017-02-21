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
<div id="app">
    Username: <input type="text" v-model.lazy="Username" /><br/>
    Password: <input type="password" v-model="Password" /><br/>
    <button type="button" v-on:click="Login()">Login</button>
    <hr/>
    {{ Message }}
</div>
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
- Cache ViewModel definition (methods, watch, ...)
- Cache handler
- [Authorize] attribute - checks HttpContext.User.Identity.IsAuthenticated
- [Role("admin", "user")] - checkes HttpContext.User.IsInRole()
- Capture error handler, like Vue.ViewModel.Register<Exception>((e, vm) => vm.JS.Alert('Error here: ' + e.Message));
