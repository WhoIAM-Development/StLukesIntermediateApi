using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class UserProfile
    {
        public string? B2CObjectId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// In resquests:
        /// March 4, 1952
        /// In responses
        /// DateTime
        /// </summary>
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        public string? MyChartActivationCode { get; set; }

    }
}
