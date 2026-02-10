using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parameters.Domain.Entities;

namespace Parameters.Infrastructure.Persistence;

/// <summary>
/// Configuração EF Core para entidade E4
/// </summary>
public class E4Configuration : IEntityTypeConfiguration<E4>
{
    public void Configure(EntityTypeBuilder<E4> builder)
    {
        builder.ToTable("e4");

        builder.HasKey(e => e.E4Stamp);

        builder.Property(e => e.E4Stamp)
            .HasColumnName("e4stamp")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Value)
            .HasColumnName("value")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.AdditionalInfo)
            .HasColumnName("additional_info")
            .HasMaxLength(1000);

        builder.Property(e => e.Sequence)
            .HasColumnName("sequence")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");

        // Indexes
        builder.HasIndex(e => e.Sequence)
            .HasDatabaseName("IX_e4_sequence");
    }
}
