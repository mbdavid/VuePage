<%@ Control Language="C#" ClassName="inputbox" %>
<script runat="server">

    public class InputBoxVM : ViewModel
    {
        [Prop("Name")]
        public string FirstName { get; set; }

        [Confirm("Confirm click me?")]
        public void ClickMe()
        {
            JS.Alert(FirstName);
        }
    }

</script>
<div class="inputbox">
    <input type="text" v-model="FirstName" />
    <button @click="ClickMe()">ClickMe</button>
    <pre>{{ $data }}</pre>
</div>
<style>
    .inputbox {
        border: 1px solid silver;
        background-color: #f3f3f3;
        padding: 5px;
        button {
            font-weight: bold;
        }
    }
</style>