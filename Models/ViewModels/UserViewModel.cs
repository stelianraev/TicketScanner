using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.ViewModels
{
    public class UsersViewModel
    {
        public List<UserViewModel>? Users { get; set; }
        public UserViewModel NewUser { get; set; }
    }

    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string ConfirmPassword { get; set; }

        public Permission Permission { get; set; }

        //public string CustomerId { get; set; }
    }

    public enum Permission
    {
        Admin = 1,
        Checker = 2,
        Scanner = 3
    }
}
