using Newtonsoft.Json;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Services
{
    public class JsonConvertor : IJsonConvertor
    {
        public T DeserializeObject<T>(string json)
        {
            var objects = JsonConvert.DeserializeObject<T>(json);
            return objects;
        }

        public string SerializeObject(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return json;
        }
    }
}
