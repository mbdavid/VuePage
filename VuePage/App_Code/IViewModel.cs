using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public interface IViewModel
    {
        string RenderInitialize(string el);
        string RenderComponent(string name, string template);

        string UpdateModel(string model, string method, JToken[] parameters, IList<HttpPostedFile> files);
    }
}
