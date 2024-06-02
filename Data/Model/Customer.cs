using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class Customer
    {
        [Key]
        public string CanonicalId { get; set; } = null!;

        public string Name { get; set; } = null!;
       
        public virtual ICollection<User>? Users { get; set; }

        public virtual ICollection<Event>? Events { get; set; }
    }
}
