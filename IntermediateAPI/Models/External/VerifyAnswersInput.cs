using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class VerifyAnswersInput
    {
        [Required]
        public string ObjectId { get; set; }
        public string? SessionId { get; set; }
        public List<int>? AnswerIndex { get; set; }
        public int ProfileId { get; set; }
    }
}
