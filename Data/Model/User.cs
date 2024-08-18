using CheckIN.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CheckIN.Data.Model
{
    public class User : IdentityUser<Guid>
    {
        public Permission Permision { get; set; }

        public virtual ICollection<UserEvent>? UserEvents { get; set; } = new List<UserEvent>();

        public virtual ICollection<UserCustomer>? UserCustomers { get; set; } = new List<UserCustomer>();
    }
}
