

namespace IntermediateAPI.Models
{
    public class ValidateUserAnswersInput
    {
        public string? SessionId { get; set; }
        public string? AnswerIndex { get; set; }

        public string? ObjectId { get; set; }
        public int ProfileId { get; set; }
    }
}
