using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Vue
{
    public class VueWriter
    {
        public VueWriter(IViewModel vm)
        {
            this.ViewModel = vm;
        }

        public IViewModel ViewModel { get; set; }

        public string Element { get; set; }

        public void Computed<TViewModel>()
        {

        }
    }
}
