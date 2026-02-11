using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parameters.Domain.Entities;

namespace Parameters.Infrastructure.Persistence;

/// <summary>
/// Configuração EF Core para entidade Para1
/// </summary>
public class Para1Configuration : IEntityTypeConfiguration<Para1>
{
    public void Configure(EntityTypeBuilder<Para1> builder)
    {
        builder.ToTable("para1");

        builder.HasKey(p => p.ParaStamp);

        builder.Property(p => p.ParaStamp)
            .HasColumnName("parastamp")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Valor)
            .HasColumnName("valor")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Tipo)
            .HasColumnName("tipo")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Dec)
            .HasColumnName("dec");

        builder.Property(p => p.Tam)
            .HasColumnName("tam");

        // Campos de auditoria PHC
        builder.Property(p => p.OUsrData)
            .HasColumnName("ousrdata")
            .IsRequired();

        builder.Property(p => p.OUsrHora)
            .HasColumnName("ousrhora")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(p => p.OUsrInis)
            .HasColumnName("ousrinis")
            .HasMaxLength(100);

        builder.Property(p => p.UsrData)
            .HasColumnName("usrdata");

        builder.Property(p => p.UsrHora)
            .HasColumnName("usrhora")
            .HasMaxLength(8);

        builder.Property(p => p.UsrInis)
            .HasColumnName("usrinis")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(p => p.Descricao).HasDatabaseName("idx_para1_descricao");
        builder.HasIndex(p => p.OUsrData).HasDatabaseName("idx_para1_ousrdata");
    }
}
