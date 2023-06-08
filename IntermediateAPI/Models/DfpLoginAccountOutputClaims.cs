using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models
{
    public class DfpLoginAccountOutputClaims : DfpResponseEnhancements
    {
        public string CorrelationId { get; set; }

        public string LoginId { get; set; }

        public string Decision { get; set; }

        public int BotScore { get; set; }

        public int RiskScore { get; set; }
        public string TransactionReferenceId { get; set; }
    }
}
