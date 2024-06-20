using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class UserCustomer
    {
        [Key]
        public Guid CustomerUsersId { get; set; }

        public Guid UserId { get; set; }

        public Guid CustomerId { get; set; }

        public User User { get; set; }

        public Customer Customer { get; set; }

        public Guid OwnerId { get; set; }

        public User Owner { get; set; }
    }
}
