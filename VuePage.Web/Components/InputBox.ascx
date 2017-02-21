<%@ Control Language="C#" ClassName="InputBox" %>
<script runat="server">

    public class InputBoxVM : Vue.ViewModel
    {
        [Vue.Prop("Name")]
        public string FirstName { get; set; }

        [Vue.Confirm("Confirm click me?")]
        public void ClickMe()
        {
            JS.Alert(FirstName);
        }
    }

</script>
<div style="border: 1px solid red; padding: 10px;">
    <input type="text" v-model="FirstName" />
    <button @click="ClickMe()">ClickMe</button>
    <pre>{{ $data }}</pre>
</div>
