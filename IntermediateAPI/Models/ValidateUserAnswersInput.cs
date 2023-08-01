

namespace IntermediateAPI.Models
{
    public class ValidateUserAnswersInput
    {
        public string? SessionId { get; set; }
        public string? AnswerIndex { get; set; }
        public string? B2CObjectId { get; set; }
    }
}
