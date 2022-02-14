using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.Models.Dto
{
    public class PostcodeLookupResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("result")]
        public PostcodeLookupResultDto Result { get; set; }
    }
}