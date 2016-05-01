using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace SecuritySystemService.Helpers
{
    public static class JsonHelper
    {
        public static async Task<string> Serialize(object obj)
        {
            var serializer = JsonSerializer.CreateDefault();
            var ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            serializer.Serialize(tw, obj);
            await tw.FlushAsync();
            ms.Position = 0;
            TextReader tr = new StreamReader(ms);
            return await tr.ReadToEndAsync();
        }

        public static JsonSerializerSettings Settings
        {
            get
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                settings.Converters.Add(new StringEnumConverter(true));
                return settings;
            }
        }
    }
}
