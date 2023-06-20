namespace IntermediateAPI.Models.UserValidation
{
    public class ValidateUserAnswersInput
    {
        public string? SessionId { get; set; }
        public string? AnswerIndex { get; set; }
        public int ProfileId { get; set; }
    }
}
