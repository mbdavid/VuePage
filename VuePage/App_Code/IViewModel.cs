using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public interface IViewModel
    {
        string RenderScript();
        string UpdateModel(string model, string method, JToken[] parameters, IList<HttpPostedFile> files);
    }
}
