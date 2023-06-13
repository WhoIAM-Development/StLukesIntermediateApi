namespace IntermediateAPI.Models.UserValidation
{
    public class ExperianVerificationResponse
    {
        public bool IsIdentityVerified { get; set; }
    }
    public class ExperianErrorResponse
    {
        public int? ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? Title { get; set; }
        public string? PositiveAction { get; set; }
        public string? NegativeAction { get; set; }
        public string? NeutralAction { get; set; }
    }
}
