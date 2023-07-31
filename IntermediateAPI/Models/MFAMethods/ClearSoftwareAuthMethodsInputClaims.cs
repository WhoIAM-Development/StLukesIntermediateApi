using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace B2CIntermediateAPI.Models.MFAMethods
{
    public class ClearSoftwareAuthMethodsInputClaims 
    {

        public string? CorrelationId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [JsonPropertyName("methodsIds")]
        public string[]? MethodsIds { get; set; }
    }
}