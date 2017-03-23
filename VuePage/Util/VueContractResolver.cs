using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Vue
{
    public class VueContractResolver : DefaultContractResolver
    {
        public static readonly VueContractResolver Instance = new VueContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                .Where(x => x.PropertyType != typeof(Computed))
                .ToList();
        }
    }
}
