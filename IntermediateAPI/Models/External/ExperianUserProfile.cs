using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class ExperianUserProfile
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// "dateOfBirth":"1976-06-17"
        /// </summary>
        [Required]
        public string DateOfBirth { get; set; }
        public string? Gender { get; set; }
        [Required]
        public string Street { get; set; }
        public string? Street2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
