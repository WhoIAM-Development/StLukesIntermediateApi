using System.Text.Json.Serialization;

namespace B2CIntermediateAPI.Models.Graph
{
    public class SoftwareOAuthMethodsIds
    {
        [JsonPropertyName("methodsIds")]
        public string[]? MethodsIds { get; set; }
    }
}
