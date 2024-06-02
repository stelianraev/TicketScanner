using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.ViewModels
{
    public class CustomerRegistrationViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassowrd { get; set; }
    }
}
