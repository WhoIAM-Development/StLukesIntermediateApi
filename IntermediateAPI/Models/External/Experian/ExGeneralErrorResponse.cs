namespace IntermediateAPI.Models.External.Experian
{
    public class ExGeneralErrorResponse
    {
        public int? ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? Title { get; set; }
        public string? PositiveAction { get; set; }
        public string? NegativeAction { get; set; }
        public string? NeutralAction { get; set; }
    }
}
