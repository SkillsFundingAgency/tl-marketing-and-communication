using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.DataLoad.PostcodesIo
{
    public class PostcodeLookupResponse
    {
        // ReSharper disable InconsistentNaming
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("result")]
        public PostcodeLookupResultDto Result { get; set; }
    }
}