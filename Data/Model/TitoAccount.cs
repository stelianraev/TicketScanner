using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckIN.Data.Model
{
    public class TitoAccount
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey("CustomerSettingsId")]
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
