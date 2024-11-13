using eDereva.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<Otp> Otps { get; set; }
        public required DbSet<User> Users { get; set; }
        public required DbSet<Role> Roles { get; set; }
        public required DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply the composite unique constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.FirstName, u.MiddleName, u.LastName })
                .IsUnique();
        }
    }
}
