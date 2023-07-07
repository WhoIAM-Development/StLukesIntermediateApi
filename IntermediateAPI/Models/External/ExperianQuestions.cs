namespace IntermediateAPI.Models.External
{

    public class ExperianQuestions
    {
        public string? SessionId { get; set; }
        public List<Question>? QuestionSet { get; set; }
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
