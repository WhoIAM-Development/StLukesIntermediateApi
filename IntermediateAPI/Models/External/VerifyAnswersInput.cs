using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class VerifyAnswersInput
    {
        public string? SessionId { get; set; }
        public List<int>? AnswerIndex { get; set; }
        public string? B2CObjectId { get; set; }
    }
}
