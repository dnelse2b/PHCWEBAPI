using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parameters.Domain.Entities;

namespace Parameters.Infrastructure.Persistence;

/// <summary>
/// Configuração EF Core para entidade E1
/// </summary>
public class E1Configuration : IEntityTypeConfiguration<E1>
{
    public void Configure(EntityTypeBuilder<E1> builder)
    {
        builder.ToTable("e1");

        builder.HasKey(e => e.E1Stamp);

        builder.Property(e => e.E1Stamp)
            .HasColumnName("e1stamp")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Active)
            .HasColumnName("active")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Relationship with E4 (1:1)
        builder.HasOne(e => e.E4)
            .WithOne(e => e.E1)
            .HasForeignKey<E4>(e => e.E4Stamp)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_e1_code");

        builder.HasIndex(e => e.Active)
            .HasDatabaseName("IX_e1_active");
    }
}
