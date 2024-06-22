using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.ViewModels
{
    public class UsersFormModel
    {
        public List<UserFormModel>? Users { get; set; }
        public UserFormModel NewUser { get; set; }
    }

    public class UserFormModel
    {
        public Guid? Id { get; set; }

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
        Owner = 1,
        Administrator = 2,
        Checker = 3,
        Scanner = 4
    }
}
