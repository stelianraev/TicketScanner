using CheckIN.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CheckIN.Data.Model
{
    public class User : IdentityUser<Guid>
    {
        public Permission Permision { get; set; }

        public virtual ICollection<UserEvent>? UserEvents { get; set; }

        public virtual ICollection<UserCustomer>? UserCustomers { get; set; } = null!;
    }
}
