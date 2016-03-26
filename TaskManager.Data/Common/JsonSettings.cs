using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Common
{
    public class JsonSettings
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
#if DEBUG
            Formatting = Formatting.Indented,
#else
                    Formatting = Formatting.None,
#endif
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>()
                    {
                        new StringEnumConverter { CamelCaseText = true }
                    }
        };
    }
}
