using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGOFWS.Domains.Models;

namespace SGOFWS.Persistence.Contexts;

public class TMPCTX : DbContext
{
	public virtual DbSet<VeiculosPorRegularizar> VeiculosPorRegularizar { get; set; }

	public TMPCTX()
	{
	}

	public TMPCTX(DbContextOptions<TMPCTX> options)
		: base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer("Server=SRV05\\SQLDEV2022;Database=BILENE_DESENV;User Id=denilson.sibinde;password=1Icor1031;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False");
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
		modelBuilder.Entity(delegate(EntityTypeBuilder<VeiculosPorRegularizar> entity)
		{
			entity.HasKey((VeiculosPorRegularizar e) => e.VeiculosPorRegularizarstamp).HasName("pk_u_fer10088").IsClustered(clustered: false);
			entity.ToTable("u_fer10088");
			entity.Property((VeiculosPorRegularizar e) => e.VeiculosPorRegularizarstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer10088stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((VeiculosPorRegularizar e) => e.Coderr).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("coderr")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Descerr).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("descerr")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Marcada).HasColumnName("marcada");
			entity.Property((VeiculosPorRegularizar e) => e.Novg).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("novg")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((VeiculosPorRegularizar e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Regularizado).HasColumnName("regularizado");
			entity.Property((VeiculosPorRegularizar e) => e.Stamppedido).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamppedido")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Tipo).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((VeiculosPorRegularizar e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculosPorRegularizar e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
	}
}
