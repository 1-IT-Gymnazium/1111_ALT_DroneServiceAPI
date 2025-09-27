using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DroneService.Data.Entities;
using DroneService.Data.Entities.Identity;

namespace DroneService.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Field> Fields { get; set; }
    public DbSet<EmailMessage> Emails { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ServiceGoal> ServiceGoals { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<IdentityUserRole<Guid>>();
        modelBuilder.Ignore<IdentityRole<Guid>>();
        modelBuilder.Ignore<IdentityUserLogin<Guid>>();
        modelBuilder.Ignore<IdentityUserToken<Guid>>();
        modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
    }
}

