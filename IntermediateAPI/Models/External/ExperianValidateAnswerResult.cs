namespace IntermediateAPI.Models.External
{
    public class ExperianValidateAnswerResult
    {
        public bool IsIdentityVerified { get; set; }

        /// <summary>
        /// **NEW** Required to link the B2C Account with MyChartUserId.
        /// </summary>
        public string? MyChartUserId { get; set; }
    }
}
