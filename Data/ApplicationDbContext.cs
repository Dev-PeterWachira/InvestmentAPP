using Microsoft.EntityFrameworkCore;
using Investment.API.Data;
using Investment.API.Models;

namespace Investment.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Fine> Fines { get; set; }

        public DbSet<Payment> Payments {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Fine>()
                .Property(f => f.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Contribution>()
                .Property(c => c.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Contribution>()
                .Property(c => c.FineAmount)
                .HasPrecision(18, 2);
        }

    }
}
