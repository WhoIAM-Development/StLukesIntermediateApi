namespace IntermediateAPI.Models.UserValidation
{
    public class ExperianData 
    {
        public string? SessionId { get; set; }
        public List<Question>? QuestionSet { get; set; }
    }
    public class ExperianDataDebug: ExperianData
    {
        // Incorporated for testing and debugging purposes only. Use this class if answers are also to be received from the upstream API. To be removed when we replace the mock API with the actual api.
        public List<int>? Answers { get; set; }
    }
    public class Question
    {
        public int QuestionType { get; set; }
        public string? QuestionText { get; set; }
        public QuestionSelect? QuestionSelect { get; set; }

    }
    public class QuestionSelect
    {
        public IEnumerable<string>? QuestionChoice { get; set; }
    }
}
