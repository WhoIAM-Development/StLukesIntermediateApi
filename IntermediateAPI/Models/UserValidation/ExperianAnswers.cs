namespace IntermediateAPI.Models.UserValidation
{
    public class ExperianAnswers
    {
        public string? SessionId { get; set; }
        public List<int>? AnswerIndex { get; set; }
        public int ProfileId { get; set; }
    }

    public static class ExperianAnswersExtension
    {
        public static ExperianAnswers ToExperianAnswers(this ValidateUserAnswersInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return new ExperianAnswers
            {
                SessionId = input.SessionId,
                AnswerIndex = input.AnswerIndex?.Split(',').Select(int.Parse).ToList(),
                ProfileId = input.ProfileId
            };
        }
    }
}
