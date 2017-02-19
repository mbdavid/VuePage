<%@ Control Language="C#" ClassName="InputBox" %>
<script runat="server">

    public class InputBoxVM : Vue.ViewModel<InputBox>
    {
        [Vue.Prop("Name")]
        public string FirstName { get; set; }

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
