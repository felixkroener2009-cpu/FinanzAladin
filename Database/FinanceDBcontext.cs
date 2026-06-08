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

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User table
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configure Transaction table explicitly
            modelBuilder.Entity<Transaction>().ToTable("Transactions");

            // Configure relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure DateTime is stored as UTC in PostgreSQL
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Date)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<User>()
                .Property(u => u.LastLogin)
                .HasColumnType("timestamp with time zone");
        }
    }
}
