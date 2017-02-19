<%@ Application Language="C#" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        Vue.Page.Component("inputbox", "~/Components/InputBox.ascx");
        Vue.Page.Component("counter", "~/Components/Counter.ascx");
    }

</script>
