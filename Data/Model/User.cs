﻿using CheckIN.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
namespace CheckIN.Data.Model
{
    public class User : IdentityUser
    {
        public string CustomerId { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public Permission Permision { get; set; }

        public virtual ICollection<UserEvent>? UserEvents { get; set; }
    }
}
