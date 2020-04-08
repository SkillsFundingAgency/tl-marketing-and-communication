using Newtonsoft.Json;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class PostcodeLookupResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("result")]
        public PostcodeLookupResultDto Result { get; set; }
    }
}