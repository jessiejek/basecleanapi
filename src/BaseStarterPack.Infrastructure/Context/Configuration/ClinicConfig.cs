using BaseStarterPack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseStarterPack.Infrastructure.Context.Configurations;

public class ClinicConfig : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> e)
    {
        e.ToTable("Clinics");
        e.HasKey(x => x.Id);

        e.HasIndex(x => x.ClinicId);

        e.Property(x => x.ClinicNo).HasMaxLength(50);
        e.Property(x => x.Location).HasMaxLength(200);
        e.Property(x => x.Landmark).HasMaxLength(200);

        // If Status is a char in DB, ensure correct column type if needed:
        // e.Property(x => x.Status).HasColumnType("char(1)");
    }
}
