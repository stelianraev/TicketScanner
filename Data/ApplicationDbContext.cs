using CheckIN.Data.Model;
using CheckIN.Services.Customer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
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
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<TitoAccount> TitoAccounts { get; set; }
        public DbSet<UserCustomer> UserCustomer { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId);

            builder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId);

            builder.Entity<UserCustomer>()
               .HasOne(ue => ue.User)
               .WithMany(u => u.UserCustomers)
               .HasForeignKey(ue => ue.UserId);

            builder.Entity<UserCustomer>()
                .HasOne(ue => ue.Customer)
                .WithMany(e => e.UserCustomers)
                .HasForeignKey(ue => ue.CustomerId);

            builder.Entity<TitoAccount>()
                .HasKey(t => t.Id);

            builder.Entity<Event>()
                .HasOne(a => a.TitoAccount)
                .WithMany(ac => ac.Events)
                .HasForeignKey(a => a.TitoAccountId);

            builder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Ticket>()
                .Property(t => t.TotalPaid)
                .HasColumnType("decimal(18, 2)");

            base.OnModelCreating(builder);
        }
    }
}
