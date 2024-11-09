using eDereva.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
