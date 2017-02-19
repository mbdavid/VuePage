<%@ Control Language="C#" ClassName="Counter" %>
<script runat="server">

    public class CounterVM : Vue.ViewModel<CounterVM>
    {
        public int Counter { get; set; }

        public CounterVM(HttpContext ctx)
        {
            Created(() => JS.ConsoleLog("HttpContext works here? " + (ctx != null)));
            Created(() => Counter = DateTime.Now.Second);
        }

        public void Increment()
        {
            Counter++;
            JS.Emit("inc");
        }
    }

</script>
<button @click="Increment()">{{ Counter }}</button>
