<%@ Page Language="C#" Title="Upload" %>
<script runat="server">

    public class PageVM : ViewModel
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
<asp:Content ContentPlaceHolderID="body" runat="server">

    <h1>Upload</h1><hr />

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