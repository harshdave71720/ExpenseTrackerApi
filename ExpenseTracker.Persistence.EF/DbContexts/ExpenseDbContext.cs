using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.Entities;

namespace ExpenseTracker.Persistence.DbContexts
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {
        }

        public DbSet<ExpenseEntity> Expenses { get; set; }

        public DbSet<CategoryEntity> Categories { get; set; }
    }
}