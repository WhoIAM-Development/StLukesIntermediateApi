using System.Text.Json.Serialization;

namespace B2CIntermediateAPI.Models.Graph
{
    public class SoftwareOAuthMethod
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("secretKey")]
        public string? SecretKey { get; set; }
    }
}