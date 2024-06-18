using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckIN.Data.Model
{
    public class CustomerSettings
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("CustomerId")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string TitoToken { get; set; }
        public virtual ICollection<TitoAccount>? TitoAccounts { get; set; }
    }
}
