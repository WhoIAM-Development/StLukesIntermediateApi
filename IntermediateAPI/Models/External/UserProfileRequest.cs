﻿using System.ComponentModel.DataAnnotations;

namespace IntermediateAPI.Models.External
{
    public class UserProfileRequest
    {
        [Required]
        public string ObjectId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
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
