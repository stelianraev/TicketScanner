using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? TitoToken { get; set; }

        public virtual ICollection<UserCustomer>? UserCustomers { get; set; }

        public virtual List<TitoAccount>? TitoAccounts { get; set; }
    }
}
