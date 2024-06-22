﻿using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.ViewModels
{
    public class UserRegistrationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public Guid CustomerId { get; set; }
    }
}
