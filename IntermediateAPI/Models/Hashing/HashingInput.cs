using System.Security;

namespace IntermediateAPI.Models.Hashing
{
    public class HashingInput
    {

        public string Password { get; set; }

        public string Salt { get; set; }

        public string? Locale { get; set; } = "en";
    }
}
