# VueServer

Implement server-side of ViewModel for `Vue` to old-but-good ASP.NET 4.5 website

```C#
public class SimpleViewModel : Vue.ViewModel<SimpleViewModel>
{
    public Username { get; set; }
    public Password { get; set; }
    public Message { get; set; }

    public void Login()
    {
        Message = AuthServie.Login(Username, Password);
    }
}

<vue:App runat="server" id="vm">
    Username: <input type="text" v-model="Username" /><br/>
    Password: <input type="password" v-model="Password" /><br/>
    <button type="button" v-on:click="Login()">Login</button>
    <hr/>
    {{ Message }}
</vue:App>
```

# TODO

- Computed resolution using Linq expression (simple linq expressions, like `x => x.Username + "-" + x.Age`

- Vue property (VueOptions) in ViewModel could be define all new Vue javascript	
- Vue.Update("jsCode");
- Vue.BeforeMount("jsCode");
- Vue.El("el");
- Vue.Directives(jsCode, jsInclude)



# SPA-Mobile
- Load page from ajax (js History)
- Work back button as expected
- MasterPage or Component?

