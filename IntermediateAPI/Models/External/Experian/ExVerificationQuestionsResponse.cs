namespace IntermediateAPI.Models.External.Experian
{
    public class ExVerificationQuestionsResponse
    {
        public string? SessionId { get; set; }
        public List<ExQuestion>? QuestionSet { get; set; }
    }
    public class ExDataDebug : ExVerificationQuestionsResponse
    {
        // Incorporated for testing and debugging purposes only. Use this class if answers are also to be received from the upstream API. To be removed when we replace the mock API with the actual api.
        public List<int>? Answers { get; set; }
    }
    public class ExQuestion
    {
        public int QuestionType { get; set; }
        public string? QuestionText { get; set; }
        public ExQuestionSelect? QuestionSelect { get; set; }

    }
    public class ExQuestionSelect
    {
        public IEnumerable<string>? QuestionChoice { get; set; }
    }
}
