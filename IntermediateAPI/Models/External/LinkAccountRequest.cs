using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class LinkAccountRequest
    {
        [Required]
        public string MyChartActivationCode { get; set; }

        [Required]
        public string ObjectId { get; set; }
    }
}
