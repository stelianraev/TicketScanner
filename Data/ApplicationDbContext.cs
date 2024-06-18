using CheckIN.Data.Model;
using CheckIN.Services.Customer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly ICustomerProvider _customerProvider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICustomerProvider customerProvider)
            : base(options)
        {
            _customerProvider = customerProvider;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<CustomerSettings> CustomerSettings { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<TitoAccount> TitoAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {         
            // Set up tenant-specific filters
            //builder.Entity<User>().HasQueryFilter(u => u.CustomerId == _tenantProvider.GetTenantId());
            //builder.Entity<Event>().HasQueryFilter(e => e.CustomerId == _tenantProvider.GetTenantId());

            builder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId);

            builder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId);

            builder.Entity<TitoAccount>()
            .HasKey(t => t.Id);

            builder.Entity<CustomerSettings>()
                .HasKey(c => c.Id);

            builder.Entity<TitoAccount>()
            .HasOne(t => t.CustomerSettings)
            .WithMany(c => c.TitoAccounts)
            .HasForeignKey(t => t.CustomerSettingsId);

            builder.Entity<CustomerSettings>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId);

            builder.Entity<Event>()
                .HasOne(a => a.TitoAccount)
                .WithMany(ac => ac.Events)
                .HasForeignKey(a => a.TitoAccountId);

            base.OnModelCreating(builder);
        }
    }
}
