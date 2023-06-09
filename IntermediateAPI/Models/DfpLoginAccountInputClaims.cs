using System;
using System.Collections.Generic;
using System.Text;

namespace IntermediateAPI.Models
{
    public class DfpLoginAccountInputClaims
    {
        public string? UserId { get; set; }

        public string? Email { get; set; }

        public string? IpAddress { get; set; }

        public string? DeviceContextId { get; set; }
        public string? Locale { get; set; } = "en";

        public bool Validate()
        {
            return !string.IsNullOrEmpty(DeviceContextId)
                && !string.IsNullOrEmpty(UserId)
                && !string.IsNullOrEmpty(Email);
        }
    }
}
