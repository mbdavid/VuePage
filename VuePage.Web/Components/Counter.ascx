<%@ Control Language="C#" ClassName="counter" %>
<script runat="server">

    public class CounterVM : ViewModel
    {
        public int Counter { get; set; }

        protected override void OnCreated()
        {
            Counter = DateTime.Now.Second;
            JS.Code("this.$el.classList.remove('hidden');");
        }

        public void Increment()
        {
            Counter++;
            JS.Emit("inc");
        }
    }

</script>
<button data-scope-counter @click="Increment()" class="hidden">{{ Counter }}</button>
<style>
    button[data-scope-counter] { color: red; background-color: yellow; }
</style>