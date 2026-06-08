using FinanzAladin.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanzAladin.Database
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure DateTime is stored as UTC in PostgreSQL
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Date)
                .HasColumnType("timestamp with time zone");
        }
    }
}