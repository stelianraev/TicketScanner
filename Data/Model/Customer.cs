﻿using System.ComponentModel.DataAnnotations;

namespace CheckIN.Data.Model
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? TitoToken { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public virtual ICollection<UserCustomer>? UserCustomers { get; set; } = new List<UserCustomer>();

        public virtual List<TitoAccount>? TitoAccounts { get; set; } = new List<TitoAccount>();
    }
}
