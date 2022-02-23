using System;
using System.Collections.Generic;
using System.Text;
using ExpenseTracker.Identity.Entities;
using ExpenseTracker.Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Identity.Infrastructure.DbContexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUserDbEntity>
    {
        public IdentityContext(DbContextOptions options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUserDbEntity>(e => e.HasIndex(e => e.Email).IsUnique());
        }
    }
}
