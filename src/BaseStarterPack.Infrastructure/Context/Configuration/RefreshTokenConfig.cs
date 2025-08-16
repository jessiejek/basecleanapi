using BaseStarterPack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseStarterPack.Infrastructure.Context.Configurations;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> e)
    {
        e.ToTable("RefreshTokens");
        e.HasKey(x => x.Id);

        e.Property(x => x.Token)
            .HasMaxLength(256)
            .IsRequired();

        e.HasIndex(x => x.Token).IsUnique();

        e.Property(x => x.ExpiresAtUtc).IsRequired();
        e.Property(x => x.CreatedAtUtc).IsRequired();
        e.Property(x => x.Revoked).IsRequired();

        // relationship defined from UserConfig too; keeping here for clarity
        e.HasOne(x => x.User)
         .WithMany(u => u.RefreshTokens)
         .HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
