using System.Text.Json.Serialization;

namespace B2CIntermediateAPI.Models.Graph
{
    public class SoftwareOAuthMethods
    {
        [JsonPropertyName("@odata.context")]
        public string? Context { get; set; }

        [JsonPropertyName("value")]
        public IEnumerable<SoftwareOAuthMethod>? Value { get; set; }
    }
}