using BaseStarterPack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseStarterPack.Infrastructure.Context.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.ToTable("Users");
        e.HasKey(x => x.Id);

        e.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        e.HasIndex(x => x.Email).IsUnique();

        e.Property(x => x.PasswordHash).IsRequired();

        e.Property(x => x.Role)
            .HasMaxLength(32)
            .HasDefaultValue("User");

        e.Property(x => x.CreatedAtUtc).IsRequired();

        // Personal details
        e.Property(x => x.FirstName).HasMaxLength(128).IsRequired();
        e.Property(x => x.MiddleName).HasMaxLength(128);
        e.Property(x => x.LastName).HasMaxLength(128).IsRequired();
        e.Property(x => x.Gender).HasMaxLength(32);
        e.Property(x => x.PhoneNumber).HasMaxLength(64);
        e.Property(x => x.Address).HasMaxLength(256);

        // Navigation
        e.HasMany(x => x.RefreshTokens)
         .WithOne(rt => rt.User)
         .HasForeignKey(rt => rt.UserId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
