<%@ Application Language="C#" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        Vue.Component.Register("inputbox", "~/Components/InputBox.ascx");
        Vue.Component.Register("counter", "~/Components/Counter.ascx");
    }

</script>
