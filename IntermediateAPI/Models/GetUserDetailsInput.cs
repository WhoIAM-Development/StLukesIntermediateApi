using IntermediateAPI.Models.External.Experian;

namespace IntermediateAPI.Models
{
    public class GetUserDetailsInput
    {
        public string DateofBirth { get; set; }

        public string ActivationCode { get; set; }
    }
}
