using BaseStarterPack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseStarterPack.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Clinic> Clinics => Set<Clinic>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Role).HasMaxLength(32).HasDefaultValue("User");
        });

        b.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.Id);

            e.Property(x => x.Token)
                .HasMaxLength(256)
                .IsRequired();

            e.HasIndex(x => x.Token).IsUnique();

            e.Property(x => x.ExpiresAtUtc).IsRequired();
            e.Property(x => x.CreatedAtUtc).IsRequired(); // NOT NULL in DB
            e.Property(x => x.Revoked).IsRequired();      // BIT NOT NULL in DB

            e.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Clinic>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ClinicId);
            e.Property(x => x.ClinicNo).HasMaxLength(50);
            e.Property(x => x.Location).HasMaxLength(200);
            e.Property(x => x.Landmark).HasMaxLength(200);
        });
    }
}
