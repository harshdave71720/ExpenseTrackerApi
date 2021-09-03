using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.Entities;

namespace ExpenseTracker.Persistence.DbContexts
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ExpenseEntity> Expenses { get; set; }
    }
}