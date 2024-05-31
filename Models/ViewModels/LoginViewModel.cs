using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string? Username { get; set; }

        [EmailAddress]
        public string? Email {  get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
