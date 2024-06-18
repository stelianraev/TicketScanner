using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckIN.Data.Model
{
    public class TitoAccount
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey("CustomerSettingsId")]
        public string CustomerSettingsId { get; set; }

        public CustomerSettings CustomerSettings { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
