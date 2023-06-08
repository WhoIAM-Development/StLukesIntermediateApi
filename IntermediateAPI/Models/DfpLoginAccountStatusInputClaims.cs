using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models
{
    public class DfpLoginAccountStatusInputClaims
    {
        public string LoginId { get; set; }

        public string StatusType { get; set; }

        public string UserId { get; set; }

        public string ReasonType { get; set; }

        public string ChallengeType { get; set; }

        public string Locale { get; set; } = "en";

        public bool Validate()
        {
            return !string.IsNullOrEmpty(LoginId)
                && !string.IsNullOrEmpty(StatusType)
                && !string.IsNullOrEmpty(UserId);
        }
    }
}
