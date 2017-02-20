# VuePage

Implement server-side of ViewModel for `Vue` to old-but-good ASP.NET 4.5 website

```C#
public class PageViewModel : Vue.ViewModel<PageViewModel>
{
    public Username { get; set; }
    public Password { get; set; }
    public Message { get; set; }

    public void Login()
    {
        Message = AuthServie.Login(Username, Password);
    }
}
```

```HTML
<vue:App runat="server" id="vm">
    Username: <input type="text" v-model="Username" /><br/>
    Password: <input type="password" v-model="Password" /><br/>
    <button type="button" v-on:click="Login()">Login</button>
    <hr/>
    {{ Message }}
</vue:App>
```

- Support Vue `Watch` server-side
- Support Lambda expression to convert to `Computed` javascript 
- Supports history navigation (ajax page load)

# TODO for next version (to desktop)
- Implement script factory handler: vue+page+ajaxget+components. Add cache support for production
- VuePage DLL
- Nuget :: "VuePage"
