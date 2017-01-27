<%@ Page Title="Page 2" Language="C#" %>
<script runat="server">

    public class VM : Vue.ViewModel<VM>
    {

        public void UploadSingleFile(string n, HttpPostedFile f)
        {
            JS.Alert("Uploaded [" + n + "] file = " + f?.FileName);
        }

        public void UploadMultipleFiles(string n, List<HttpPostedFile> files)
        {
            JS.Alert("Uploaded [" + n + "] file = " + files.Count);
        }

        public void UploadAny(List<HttpPostedFile> files)
        {
            JS.Alert("Any file = " + files.Count);
        }

        public void NoUpload()
        {
            JS.Alert("no upload");
        }
    }

</script>
<asp:Content ID="body" ContentPlaceHolderID="body" Runat="Server">

    <h2>Upload</h2>
    <hr />

    <fieldset>
        <legend>Single File</legend>
        <input type="file" id="f1" />
        <button type="button" @click="UploadSingleFile('u1', '#f1')">Upload file</button>
    </fieldset>

    <fieldset>
        <legend>Multi File</legend>
        <input type="file" multiple="multiple" />
        <button type="button" @click="UploadMultipleFiles('u2', '[multiple]')">Upload files</button>
    </fieldset>

    <button type="button" @click="NoUpload()">NoUpload</button>
    <button type="button" @click="UploadAny('input[type=file]')">Upload from Any</button>


</asp:Content>
