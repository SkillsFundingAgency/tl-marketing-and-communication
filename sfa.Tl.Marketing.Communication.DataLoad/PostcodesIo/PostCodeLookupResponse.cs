using System.Text.Json.Serialization;

namespace sfa.Tl.Marketing.Communication.DataLoad.PostcodesIo
{
    public class PostcodeLookupResponse
    {
        // ReSharper disable InconsistentNaming
        [JsonPropertyName("status")]
        // ReSharper disable once UnusedMember.Global
        public int Status { get; set; }

        [JsonPropertyName("result")]
        public PostcodeLookupResultDto Result { get; set; }
    }
}