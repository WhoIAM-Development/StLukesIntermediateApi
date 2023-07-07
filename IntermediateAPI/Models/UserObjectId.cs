using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models
{
    public class UserObjectId
    {
        [Required]
        public string ObjectId { get; set; }
    }
}
