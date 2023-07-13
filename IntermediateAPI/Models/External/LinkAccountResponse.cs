using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class LinkAccountResponse
    {
        public bool HasMyChartBeenLinked { get; set; }

        [Required]
        public string? MyChartUserId { get; set; }
    }
}
