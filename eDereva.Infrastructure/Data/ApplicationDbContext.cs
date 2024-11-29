using eDereva.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<Otp> Otps { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Role> Roles { get; set; }
    public required DbSet<Permission> Permissions { get; set; }
    public required DbSet<Venue> Venues { get; set; }
    public required DbSet<Session> Sessions { get; set; }
    public required DbSet<Contingency> Contingencies { get; set; }
    public required DbSet<Region> Regions { get; set; }
    public required DbSet<District> Districts { get; set; }
    public required DbSet<VenueExemption> VenueExemptions { get; set; }
    public required DbSet<QuestionBank> QuestionBanks { get; set; }
    public required DbSet<Question> Questions { get; set; }
    public required DbSet<Choice> Choices { get; set; }
    public required DbSet<Answer> Answers { get; set; }
    public required DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply the composite unique constraint
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.FirstName, u.MiddleName, u.LastName })
            .IsUnique();
    }
}