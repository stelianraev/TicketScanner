using Microsoft.AspNetCore.Identity;
namespace CheckIN.Data.Model
{
    public class User : IdentityUser
    {
        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public virtual ICollection<UserEvent>? UserEvents { get; set; }
    }
}
