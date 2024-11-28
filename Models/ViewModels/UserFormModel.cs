using CheckIN.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CheckIN.Models.ViewModels
{
    public class UserFormModel : IValidatableObject
    {
        public Guid? Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string ConfirmPassword { get; set; }

        public List<Permission> Permissions { get; set; }

        //EditUser to have all possible ticket types to select permission
        public List<SelectListItem>? TicketTypeList { get; set; }

        //TicketType for specific User
        public List<string> TicketTypesPermission { get; set; } = new List<string> ();


        public List<string>? SelectedTicketTypesPermission { get; set; } = new List<string>();

        public bool IsRegistration { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsRegistration && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("The Password field is required.", new[] { "Password" });
            }
        }
    }
}
