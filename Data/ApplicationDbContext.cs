using Microsoft.EntityFrameworkCore;
using Investment.API.Data;
using Investment.API.Models;

namespace Investment.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<User> Users {get; set;}
        public DbSet<Contribution> Contributions {get; set;}
        public DbSet<Fine> Fines {get; set;}
        
        
    }
}
