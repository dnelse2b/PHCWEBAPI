using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using SGOFWS.Domains.Models.CfmMain;

namespace SGOFWS.Persistence.Contexts;

public class CFMMAINCONTEXT : DbContext
{
	public virtual DbSet<Bo> Bo { get; set; }

	public virtual DbSet<Bo3> Bo3 { get; set; }

	public virtual DbSet<Bi> Bi { get; set; }

	public virtual DbSet<Bo2> Bo2 { get; set; }

	public virtual DbSet<Cl> Cl { get; set; }

	public virtual DbSet<UBovg> UBovg { get; set; }

	public CFMMAINCONTEXT()
	{
	}

	public CFMMAINCONTEXT(DbContextOptions<CFMMAINCONTEXT> options)
		: base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString("CFMGERALConnStr"));
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bo> entity)
		{
			entity.HasKey((Bo e) => new { e.Ndos, e.Obrano, e.Boano }).HasName("pk_bo").IsClustered(clustered: false);
			entity.ToTable("bo");
			entity.HasIndex((Bo e) => new { e.Nmdos, e.Obrano, e.Dataobra, e.Nome, e.No, e.Totaldeb, e.Etotaldeb, e.Bostamp }, "in_bo_bolist").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Cxstamp, "in_bo_cxstamp").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Mastamp, "in_bo_mastamp").HasFillFactor(80);
			entity.HasIndex((Bo e) => new { e.Ndos, e.Obrano, e.Boano }, "in_bo_ndos_ano").HasFillFactor(80);
			entity.HasIndex((Bo e) => new { e.Ndos, e.No, e.Obrano }, "in_bo_ndos_no").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.No, "in_bo_no").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Obrano, "in_bo_obrano").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Pastamp, "in_bo_pastamp").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Snstamp, "in_bo_snstamp").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Ssstamp, "in_bo_ssstamp").HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Bostamp, "in_bo_stamp").IsUnique().HasFillFactor(80);
			entity.HasIndex((Bo e) => e.Tpstamp, "in_bo_tpstamp").HasFillFactor(80);
			entity.Property((Bo e) => e.Ndos).HasColumnType("numeric(3, 0)").HasColumnName("ndos");
			entity.Property((Bo e) => e.Obrano).HasColumnType("numeric(10, 0)").HasColumnName("obrano");
			entity.Property((Bo e) => e.Boano).HasColumnType("numeric(4, 0)").HasColumnName("boano");
			entity.Property((Bo e) => e.Alldescli).HasColumnName("alldescli");
			entity.Property((Bo e) => e.Alldesfor).HasColumnName("alldesfor");
			entity.Property((Bo e) => e.Aprovado).HasColumnName("aprovado");
			entity.Property((Bo e) => e.Bo11Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo11_bins");
			entity.Property((Bo e) => e.Bo11Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo11_iva");
			entity.Property((Bo e) => e.Bo12Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo12_bins");
			entity.Property((Bo e) => e.Bo12Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo12_iva");
			entity.Property((Bo e) => e.Bo1tvall).HasColumnType("numeric(18, 5)").HasColumnName("bo_1tvall");
			entity.Property((Bo e) => e.Bo21Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo21_bins");
			entity.Property((Bo e) => e.Bo21Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo21_iva");
			entity.Property((Bo e) => e.Bo22Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo22_bins");
			entity.Property((Bo e) => e.Bo22Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo22_iva");
			entity.Property((Bo e) => e.Bo2tdesc1).HasColumnType("numeric(18, 5)").HasColumnName("bo_2tdesc1");
			entity.Property((Bo e) => e.Bo2tdesc2).HasColumnType("numeric(18, 5)").HasColumnName("bo_2tdesc2");
			entity.Property((Bo e) => e.Bo2tvall).HasColumnType("numeric(18, 5)").HasColumnName("bo_2tvall");
			entity.Property((Bo e) => e.Bo31Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo31_bins");
			entity.Property((Bo e) => e.Bo31Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo31_iva");
			entity.Property((Bo e) => e.Bo32Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo32_bins");
			entity.Property((Bo e) => e.Bo32Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo32_iva");
			entity.Property((Bo e) => e.Bo41Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo41_bins");
			entity.Property((Bo e) => e.Bo41Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo41_iva");
			entity.Property((Bo e) => e.Bo42Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo42_bins");
			entity.Property((Bo e) => e.Bo42Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo42_iva");
			entity.Property((Bo e) => e.Bo51Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo51_bins");
			entity.Property((Bo e) => e.Bo51Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo51_iva");
			entity.Property((Bo e) => e.Bo52Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo52_bins");
			entity.Property((Bo e) => e.Bo52Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo52_iva");
			entity.Property((Bo e) => e.Bo61Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo61_bins");
			entity.Property((Bo e) => e.Bo61Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo61_iva");
			entity.Property((Bo e) => e.Bo62Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo62_bins");
			entity.Property((Bo e) => e.Bo62Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo62_iva");
			entity.Property((Bo e) => e.BoTotp1).HasColumnType("numeric(18, 5)").HasColumnName("bo_totp1");
			entity.Property((Bo e) => e.BoTotp2).HasColumnType("numeric(18, 5)").HasColumnName("bo_totp2");
			entity.Property((Bo e) => e.Boclose).HasColumnName("boclose");
			entity.Property((Bo e) => e.Boid).HasColumnType("numeric(12, 0)").ValueGeneratedOnAdd()
				.HasColumnName("boid");
			entity.Property((Bo e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Ccusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ccusto")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Cobranca).HasMaxLength(22).IsUnicode(unicode: false)
				.HasColumnName("cobranca")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Codpost).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("codpost")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Custo).HasColumnType("numeric(18, 5)").HasColumnName("custo");
			entity.Property((Bo e) => e.Cxstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("cxstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Cxusername).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("cxusername")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Datafecho).HasColumnType("datetime").HasColumnName("datafecho")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.Datafinal).HasColumnType("datetime").HasColumnName("datafinal")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.Dataobra).HasColumnType("datetime").HasColumnName("dataobra")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.Dataopen).HasColumnType("datetime").HasColumnName("dataopen")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.Descc).HasColumnType("numeric(18, 5)").HasColumnName("descc");
			entity.Property((Bo e) => e.Dtclose).HasColumnType("datetime").HasColumnName("dtclose")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.Ean).HasMaxLength(35).IsUnicode(unicode: false)
				.HasColumnName("ean")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Ebo11Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo11_bins");
			entity.Property((Bo e) => e.Ebo11Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo11_iva");
			entity.Property((Bo e) => e.Ebo12Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo12_bins");
			entity.Property((Bo e) => e.Ebo12Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo12_iva");
			entity.Property((Bo e) => e.Ebo1tvall).HasColumnType("numeric(19, 6)").HasColumnName("ebo_1tvall");
			entity.Property((Bo e) => e.Ebo21Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo21_bins");
			entity.Property((Bo e) => e.Ebo21Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo21_iva");
			entity.Property((Bo e) => e.Ebo22Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo22_bins");
			entity.Property((Bo e) => e.Ebo22Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo22_iva");
			entity.Property((Bo e) => e.Ebo2tdes1).HasColumnType("numeric(19, 6)").HasColumnName("ebo_2tdes1");
			entity.Property((Bo e) => e.Ebo2tdes2).HasColumnType("numeric(19, 6)").HasColumnName("ebo_2tdes2");
			entity.Property((Bo e) => e.Ebo2tvall).HasColumnType("numeric(19, 6)").HasColumnName("ebo_2tvall");
			entity.Property((Bo e) => e.Ebo31Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo31_bins");
			entity.Property((Bo e) => e.Ebo31Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo31_iva");
			entity.Property((Bo e) => e.Ebo32Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo32_bins");
			entity.Property((Bo e) => e.Ebo32Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo32_iva");
			entity.Property((Bo e) => e.Ebo41Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo41_bins");
			entity.Property((Bo e) => e.Ebo41Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo41_iva");
			entity.Property((Bo e) => e.Ebo42Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo42_bins");
			entity.Property((Bo e) => e.Ebo42Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo42_iva");
			entity.Property((Bo e) => e.Ebo51Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo51_bins");
			entity.Property((Bo e) => e.Ebo51Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo51_iva");
			entity.Property((Bo e) => e.Ebo52Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo52_bins");
			entity.Property((Bo e) => e.Ebo52Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo52_iva");
			entity.Property((Bo e) => e.Ebo61Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo61_bins");
			entity.Property((Bo e) => e.Ebo61Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo61_iva");
			entity.Property((Bo e) => e.Ebo62Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo62_bins");
			entity.Property((Bo e) => e.Ebo62Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo62_iva");
			entity.Property((Bo e) => e.EboTotp1).HasColumnType("numeric(19, 6)").HasColumnName("ebo_totp1");
			entity.Property((Bo e) => e.EboTotp2).HasColumnType("numeric(19, 6)").HasColumnName("ebo_totp2");
			entity.Property((Bo e) => e.Ecusto).HasColumnType("numeric(19, 6)").HasColumnName("ecusto");
			entity.Property((Bo e) => e.Edescc).HasColumnType("numeric(19, 6)").HasColumnName("edescc");
			entity.Property((Bo e) => e.Edi).HasColumnName("edi");
			entity.Property((Bo e) => e.Emconf).HasColumnName("emconf");
			entity.Property((Bo e) => e.Esdeb1).HasColumnType("numeric(19, 6)").HasColumnName("esdeb1");
			entity.Property((Bo e) => e.Esdeb2).HasColumnType("numeric(19, 6)").HasColumnName("esdeb2");
			entity.Property((Bo e) => e.Esdeb3).HasColumnType("numeric(19, 6)").HasColumnName("esdeb3");
			entity.Property((Bo e) => e.Esdeb4).HasColumnType("numeric(19, 6)").HasColumnName("esdeb4");
			entity.Property((Bo e) => e.Estab).HasColumnType("numeric(3, 0)").HasColumnName("estab");
			entity.Property((Bo e) => e.Estot1).HasColumnType("numeric(19, 6)").HasColumnName("estot1");
			entity.Property((Bo e) => e.Estot2).HasColumnType("numeric(19, 6)").HasColumnName("estot2");
			entity.Property((Bo e) => e.Estot3).HasColumnType("numeric(19, 6)").HasColumnName("estot3");
			entity.Property((Bo e) => e.Estot4).HasColumnType("numeric(19, 6)").HasColumnName("estot4");
			entity.Property((Bo e) => e.Etotal).HasColumnType("numeric(19, 6)").HasColumnName("etotal");
			entity.Property((Bo e) => e.Etotaldeb).HasColumnType("numeric(19, 6)").HasColumnName("etotaldeb");
			entity.Property((Bo e) => e.Evqtt21).HasColumnType("numeric(19, 6)").HasColumnName("evqtt21");
			entity.Property((Bo e) => e.Evqtt22).HasColumnType("numeric(19, 6)").HasColumnName("evqtt22");
			entity.Property((Bo e) => e.Evqtt23).HasColumnType("numeric(19, 6)").HasColumnName("evqtt23");
			entity.Property((Bo e) => e.Evqtt24).HasColumnType("numeric(19, 6)").HasColumnName("evqtt24");
			entity.Property((Bo e) => e.Fechada).HasColumnName("fechada");
			entity.Property((Bo e) => e.Fref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("fref")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Iecacodisen).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("iecacodisen")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Iemail).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("iemail")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Iiva).HasColumnName("iiva");
			entity.Property((Bo e) => e.Impresso).HasColumnName("impresso");
			entity.Property((Bo e) => e.Infref).HasColumnName("infref");
			entity.Property((Bo e) => e.Inome).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("inome")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Itotais).HasColumnName("itotais");
			entity.Property((Bo e) => e.Itotaisiva).HasColumnName("itotaisiva");
			entity.Property((Bo e) => e.Iunit).HasColumnName("iunit");
			entity.Property((Bo e) => e.Iunitiva).HasColumnName("iunitiva");
			entity.Property((Bo e) => e.Lang).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("lang")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Lifref).HasColumnName("lifref");
			entity.Property((Bo e) => e.Local).HasMaxLength(43).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Logi1).HasColumnName("logi1");
			entity.Property((Bo e) => e.Logi2).HasColumnName("logi2");
			entity.Property((Bo e) => e.Logi3).HasColumnName("logi3");
			entity.Property((Bo e) => e.Logi4).HasColumnName("logi4");
			entity.Property((Bo e) => e.Logi5).HasColumnName("logi5");
			entity.Property((Bo e) => e.Logi6).HasColumnName("logi6");
			entity.Property((Bo e) => e.Logi7).HasColumnName("logi7");
			entity.Property((Bo e) => e.Logi8).HasColumnName("logi8");
			entity.Property((Bo e) => e.Maquina).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("maquina")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Marca).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("marca")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Bo e) => e.Mastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("mastamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Memissao).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("memissao")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Moeda).HasMaxLength(11).IsUnicode(unicode: false)
				.HasColumnName("moeda")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Moetotal).HasColumnType("numeric(13, 3)").HasColumnName("moetotal");
			entity.Property((Bo e) => e.Morada).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("morada")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Ncont).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ncont")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Ncusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ncusto")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Nmdos).HasMaxLength(24).IsUnicode(unicode: false)
				.HasColumnName("nmdos")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.No).HasColumnType("numeric(10, 0)").HasColumnName("no");
			entity.Property((Bo e) => e.Nome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Nome2).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Nomquina).HasColumnType("numeric(6, 0)").HasColumnName("nomquina");
			entity.Property((Bo e) => e.Nopat).HasColumnType("numeric(10, 0)").HasColumnName("nopat");
			entity.Property((Bo e) => e.Obranome).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("obranome")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Obs).HasMaxLength(67).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Obstab2).HasColumnType("text").HasColumnName("obstab2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Ocupacao).HasColumnType("numeric(1, 0)").HasColumnName("ocupacao");
			entity.Property((Bo e) => e.Origem).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("origem")
				.HasDefaultValueSql("('BO')");
			entity.Property((Bo e) => e.Orinopat).HasColumnType("numeric(10, 0)").HasColumnName("orinopat");
			entity.Property((Bo e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Pastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("pastamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Period).HasColumnType("numeric(4, 0)").HasColumnName("period");
			entity.Property((Bo e) => e.Pno).HasColumnType("numeric(3, 0)").HasColumnName("pno");
			entity.Property((Bo e) => e.Pnome).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("pnome")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Quarto).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("quarto")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Sdeb1).HasColumnType("numeric(18, 5)").HasColumnName("sdeb1");
			entity.Property((Bo e) => e.Sdeb2).HasColumnType("numeric(18, 5)").HasColumnName("sdeb2");
			entity.Property((Bo e) => e.Sdeb3).HasColumnType("numeric(18, 5)").HasColumnName("sdeb3");
			entity.Property((Bo e) => e.Sdeb4).HasColumnType("numeric(18, 5)").HasColumnName("sdeb4");
			entity.Property((Bo e) => e.Segmento).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("segmento")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Serie).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("serie")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Series).HasColumnType("text").HasColumnName("series")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Series2).HasColumnType("text").HasColumnName("series2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Site).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("site")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Situacao).HasColumnType("numeric(1, 0)").HasColumnName("situacao");
			entity.Property((Bo e) => e.Smoe1).HasColumnType("numeric(13, 3)").HasColumnName("smoe1");
			entity.Property((Bo e) => e.Smoe2).HasColumnType("numeric(13, 3)").HasColumnName("smoe2");
			entity.Property((Bo e) => e.Smoe3).HasColumnType("numeric(13, 3)").HasColumnName("smoe3");
			entity.Property((Bo e) => e.Smoe4).HasColumnType("numeric(13, 3)").HasColumnName("smoe4");
			entity.Property((Bo e) => e.Snstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("snstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Sqtt11).HasColumnType("numeric(13, 3)").HasColumnName("sqtt11");
			entity.Property((Bo e) => e.Sqtt12).HasColumnType("numeric(13, 3)").HasColumnName("sqtt12");
			entity.Property((Bo e) => e.Sqtt13).HasColumnType("numeric(13, 3)").HasColumnName("sqtt13");
			entity.Property((Bo e) => e.Sqtt14).HasColumnType("numeric(13, 3)").HasColumnName("sqtt14");
			entity.Property((Bo e) => e.Sqtt21).HasColumnType("numeric(13, 3)").HasColumnName("sqtt21");
			entity.Property((Bo e) => e.Sqtt22).HasColumnType("numeric(13, 3)").HasColumnName("sqtt22");
			entity.Property((Bo e) => e.Sqtt23).HasColumnType("numeric(13, 3)").HasColumnName("sqtt23");
			entity.Property((Bo e) => e.Sqtt24).HasColumnType("numeric(13, 3)").HasColumnName("sqtt24");
			entity.Property((Bo e) => e.Ssstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ssstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Ssusername).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ssusername")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Statuspda).HasMaxLength(1).IsUnicode(unicode: false)
				.HasColumnName("statuspda")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Stot1).HasColumnType("numeric(18, 5)").HasColumnName("stot1");
			entity.Property((Bo e) => e.Stot2).HasColumnType("numeric(18, 5)").HasColumnName("stot2");
			entity.Property((Bo e) => e.Stot3).HasColumnType("numeric(18, 5)").HasColumnName("stot3");
			entity.Property((Bo e) => e.Stot4).HasColumnType("numeric(18, 5)").HasColumnName("stot4");
			entity.Property((Bo e) => e.Tabela1).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tabela1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Tabela2).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tabela2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Tecnico).HasColumnType("numeric(4, 0)").HasColumnName("tecnico");
			entity.Property((Bo e) => e.Tecnnm).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tecnnm")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Tipo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Total).HasColumnType("numeric(18, 5)").HasColumnName("total");
			entity.Property((Bo e) => e.Totaldeb).HasColumnType("numeric(18, 5)").HasColumnName("totaldeb");
			entity.Property((Bo e) => e.Tpdesc).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("tpdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Tpstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tpstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo e) => e.Trab1).HasMaxLength(67).IsUnicode(unicode: false)
				.HasColumnName("trab1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Trab2).HasMaxLength(67).IsUnicode(unicode: false)
				.HasColumnName("trab2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Trab3).HasMaxLength(67).IsUnicode(unicode: false)
				.HasColumnName("trab3")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Trab4).HasMaxLength(67).IsUnicode(unicode: false)
				.HasColumnName("trab4")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Trab5).HasMaxLength(67).IsUnicode(unicode: false)
				.HasColumnName("trab5")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UAnocm).HasColumnType("numeric(4, 0)").HasColumnName("u_anocm");
			entity.Property((Bo e) => e.UAnulada).HasColumnName("u_anulada");
			entity.Property((Bo e) => e.UBancoff).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_bancoff")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBoact).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("u_boact")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBocam).HasColumnType("numeric(12, 2)").HasColumnName("u_bocam");
			entity.Property((Bo e) => e.UBocont).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_bocont")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBoempr).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("u_boempr")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBoiva).HasColumnType("numeric(12, 2)").HasColumnName("u_boiva");
			entity.Property((Bo e) => e.UBomora).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_bomora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBonots).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_bonots")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBonuit).HasColumnType("numeric(10, 0)").HasColumnName("u_bonuit");
			entity.Property((Bo e) => e.UBoobs).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_boobs")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_bostamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UBotel).HasColumnType("numeric(12, 0)").HasColumnName("u_botel");
			entity.Property((Bo e) => e.UBotot).HasColumnType("numeric(12, 2)").HasColumnName("u_botot");
			entity.Property((Bo e) => e.UCm).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_cm")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UContado).HasColumnType("numeric(10, 0)").HasColumnName("u_contado");
			entity.Property((Bo e) => e.UContaff).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_contaff")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UCotstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_cotstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UDaprova).HasColumnType("datetime").HasColumnName("u_daprova")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.UDtctt).HasColumnType("datetime").HasColumnName("u_dtctt")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.UEstcod).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_estcod")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UEstdesc).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_estdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UFact).HasColumnType("numeric(16, 0)").HasColumnName("u_fact");
			entity.Property((Bo e) => e.UHaprova).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("u_haprova")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UImo).HasColumnType("numeric(10, 0)").HasColumnName("u_imo");
			entity.Property((Bo e) => e.UIva).HasColumnType("numeric(12, 2)").HasColumnName("u_iva");
			entity.Property((Bo e) => e.ULeiact).HasColumnType("numeric(14, 0)").HasColumnName("u_leiact");
			entity.Property((Bo e) => e.UManif).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_manif")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UMcam).HasColumnType("numeric(16, 2)").HasColumnName("u_mcam");
			entity.Property((Bo e) => e.UMntfixo).HasColumnType("numeric(16, 2)").HasColumnName("u_mntfixo");
			entity.Property((Bo e) => e.UNaprova).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("u_naprova")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UNavio).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_navio")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UNcontref).HasColumnName("u_ncontref");
			entity.Property((Bo e) => e.UNguia).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_nguia")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UOlcodigo).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("u_olcodigo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UReentry).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_reentry")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.URefctt).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_refctt")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.UTdndoc).HasColumnType("numeric(3, 0)").HasColumnName("u_tdndoc");
			entity.Property((Bo e) => e.UTotiva).HasColumnType("numeric(12, 2)").HasColumnName("u_totiva");
			entity.Property((Bo e) => e.UValoradm).HasColumnType("numeric(16, 5)").HasColumnName("u_valoradm");
			entity.Property((Bo e) => e.Ultfact).HasColumnType("datetime").HasColumnName("ultfact")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo e) => e.Userimpresso).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("userimpresso")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Vendedor).HasColumnType("numeric(4, 0)").HasColumnName("vendedor");
			entity.Property((Bo e) => e.Vendnm).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("vendnm")
				.HasDefaultValueSql("('')");
			entity.Property((Bo e) => e.Vqtt21).HasColumnType("numeric(18, 5)").HasColumnName("vqtt21");
			entity.Property((Bo e) => e.Vqtt22).HasColumnType("numeric(18, 5)").HasColumnName("vqtt22");
			entity.Property((Bo e) => e.Vqtt23).HasColumnType("numeric(18, 5)").HasColumnName("vqtt23");
			entity.Property((Bo e) => e.Vqtt24).HasColumnType("numeric(18, 5)").HasColumnName("vqtt24");
			entity.Property((Bo e) => e.Zona).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("zona")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bo2> entity)
		{
			entity.HasKey((Bo2 e) => e.Bo2stamp).HasName("pk_bo2").IsClustered(clustered: false);
			entity.ToTable("bo2");
			entity.HasIndex((Bo2 e) => new { e.Bo2stamp, e.Anulado }, "in_bo2_anulado").HasFillFactor(80);
			entity.Property((Bo2 e) => e.Bo2stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bo2stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Adjbostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("adjbostamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Adjudicado).HasColumnName("adjudicado");
			entity.Property((Bo2 e) => e.Alvstamp1).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("alvstamp1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Alvstamp2).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("alvstamp2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Anulado).HasColumnName("anulado");
			entity.Property((Bo2 e) => e.Ar2mazem).HasColumnType("numeric(5, 0)").HasColumnName("ar2mazem");
			entity.Property((Bo2 e) => e.Area).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("area")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Armazem).HasColumnType("numeric(5, 0)").HasColumnName("armazem");
			entity.Property((Bo2 e) => e.Assinatura).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("assinatura")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Atcodeid).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("atcodeid")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Autobostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("autobostamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Autono).HasColumnType("numeric(10, 0)").HasColumnName("autono");
			entity.Property((Bo2 e) => e.Autoper).HasColumnType("numeric(5, 0)").HasColumnName("autoper");
			entity.Property((Bo2 e) => e.Autos).HasColumnName("autos");
			entity.Property((Bo2 e) => e.Autotipo).HasColumnType("numeric(1, 0)").HasColumnName("autotipo");
			entity.Property((Bo2 e) => e.Bo71Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo71_bins");
			entity.Property((Bo2 e) => e.Bo71Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo71_iva");
			entity.Property((Bo2 e) => e.Bo72Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo72_bins");
			entity.Property((Bo2 e) => e.Bo72Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo72_iva");
			entity.Property((Bo2 e) => e.Bo81Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo81_bins");
			entity.Property((Bo2 e) => e.Bo81Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo81_iva");
			entity.Property((Bo2 e) => e.Bo82Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo82_bins");
			entity.Property((Bo2 e) => e.Bo82Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo82_iva");
			entity.Property((Bo2 e) => e.Bo91Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo91_bins");
			entity.Property((Bo2 e) => e.Bo91Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo91_iva");
			entity.Property((Bo2 e) => e.Bo92Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo92_bins");
			entity.Property((Bo2 e) => e.Bo92Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo92_iva");
			entity.Property((Bo2 e) => e.Calistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("calistamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Cambio).HasColumnType("numeric(20, 12)").HasColumnName("cambio");
			entity.Property((Bo2 e) => e.Cambiofixo).HasColumnName("cambiofixo");
			entity.Property((Bo2 e) => e.Carga).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Cladrsdesc).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("cladrsdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Cladrsstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("cladrsstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Cladrszona).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("cladrszona")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Codpost).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("codpost")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Consfinal).HasColumnName("consfinal");
			entity.Property((Bo2 e) => e.Contacto).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("contacto")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Crpstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("crpstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Custototaldif).HasColumnType("numeric(18, 5)").HasColumnName("custototaldif");
			entity.Property((Bo2 e) => e.Custototalorc).HasColumnType("numeric(18, 5)").HasColumnName("custototalorc");
			entity.Property((Bo2 e) => e.Custototalreorc).HasColumnType("numeric(18, 5)").HasColumnName("custototalreorc");
			entity.Property((Bo2 e) => e.Descar).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("descar")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Descnegocio).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("descnegocio")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Descobra).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("descobra")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Diasate).HasColumnType("numeric(4, 0)").HasColumnName("diasate");
			entity.Property((Bo2 e) => e.Diasde).HasColumnType("numeric(4, 0)").HasColumnName("diasde");
			entity.Property((Bo2 e) => e.Dpedidopv).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("dpedidopv")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Dprocesso).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("dprocesso")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ebo71Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo71_bins");
			entity.Property((Bo2 e) => e.Ebo71Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo71_iva");
			entity.Property((Bo2 e) => e.Ebo72Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo72_bins");
			entity.Property((Bo2 e) => e.Ebo72Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo72_iva");
			entity.Property((Bo2 e) => e.Ebo81Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo81_bins");
			entity.Property((Bo2 e) => e.Ebo81Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo81_iva");
			entity.Property((Bo2 e) => e.Ebo82Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo82_bins");
			entity.Property((Bo2 e) => e.Ebo82Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo82_iva");
			entity.Property((Bo2 e) => e.Ebo91Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo91_bins");
			entity.Property((Bo2 e) => e.Ebo91Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo91_iva");
			entity.Property((Bo2 e) => e.Ebo92Bins).HasColumnType("numeric(19, 6)").HasColumnName("ebo92_bins");
			entity.Property((Bo2 e) => e.Ebo92Iva).HasColumnType("numeric(19, 6)").HasColumnName("ebo92_iva");
			entity.Property((Bo2 e) => e.Ecustototaldif).HasColumnType("numeric(19, 6)").HasColumnName("ecustototaldif");
			entity.Property((Bo2 e) => e.Ecustototalorc).HasColumnType("numeric(19, 6)").HasColumnName("ecustototalorc");
			entity.Property((Bo2 e) => e.Ecustototalreorc).HasColumnType("numeric(19, 6)").HasColumnName("ecustototalreorc");
			entity.Property((Bo2 e) => e.EftaxamtA).HasColumnType("numeric(19, 6)").HasColumnName("eftaxamt_a");
			entity.Property((Bo2 e) => e.EftaxamtB).HasColumnType("numeric(19, 6)").HasColumnName("eftaxamt_b");
			entity.Property((Bo2 e) => e.Email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("email")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Emargemtotaldif).HasColumnType("numeric(19, 6)").HasColumnName("emargemtotaldif");
			entity.Property((Bo2 e) => e.Emargemtotalorc).HasColumnType("numeric(19, 6)").HasColumnName("emargemtotalorc");
			entity.Property((Bo2 e) => e.Emargemtotalreorc).HasColumnType("numeric(19, 6)").HasColumnName("emargemtotalreorc");
			entity.Property((Bo2 e) => e.Encm).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("encm")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Encmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("encmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Etotalciva).HasColumnType("numeric(19, 6)").HasColumnName("etotalciva");
			entity.Property((Bo2 e) => e.Etotiva).HasColumnType("numeric(19, 6)").HasColumnName("etotiva");
			entity.Property((Bo2 e) => e.Ettecoval).HasColumnType("numeric(19, 6)").HasColumnName("ettecoval");
			entity.Property((Bo2 e) => e.Ettecoval2).HasColumnType("numeric(19, 6)").HasColumnName("ettecoval2");
			entity.Property((Bo2 e) => e.Ettieca).HasColumnType("numeric(19, 6)").HasColumnName("ettieca");
			entity.Property((Bo2 e) => e.Excm).HasColumnType("numeric(2, 0)").HasColumnName("excm");
			entity.Property((Bo2 e) => e.Excmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("excmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Exportado).HasColumnName("exportado");
			entity.Property((Bo2 e) => e.FtaxamtA).HasColumnType("numeric(18, 5)").HasColumnName("ftaxamt_a");
			entity.Property((Bo2 e) => e.FtaxamtB).HasColumnType("numeric(18, 5)").HasColumnName("ftaxamt_b");
			entity.Property((Bo2 e) => e.Horasl).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("horasl")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Identificacao1).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("identificacao1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Identificacao2).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("identificacao2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Idserie).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("idserie")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Iectisento).HasColumnName("iectisento");
			entity.Property((Bo2 e) => e.Local).HasMaxLength(43).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Bo2 e) => e.Margemorcperc).HasColumnType("numeric(15, 2)").HasColumnName("margemorcperc");
			entity.Property((Bo2 e) => e.Margemreorcperc).HasColumnType("numeric(15, 2)").HasColumnName("margemreorcperc");
			entity.Property((Bo2 e) => e.Margemtotaldif).HasColumnType("numeric(18, 5)").HasColumnName("margemtotaldif");
			entity.Property((Bo2 e) => e.Margemtotalorc).HasColumnType("numeric(18, 5)").HasColumnName("margemtotalorc");
			entity.Property((Bo2 e) => e.Margemtotalreorc).HasColumnType("numeric(18, 5)").HasColumnName("margemtotalreorc");
			entity.Property((Bo2 e) => e.Mcomercial).HasColumnType("numeric(6, 2)").HasColumnName("mcomercial");
			entity.Property((Bo2 e) => e.Mensaldia).HasColumnType("numeric(2, 0)").HasColumnName("mensaldia");
			entity.Property((Bo2 e) => e.Morada).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("morada")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Mtotalciva).HasColumnType("numeric(13, 3)").HasColumnName("mtotalciva");
			entity.Property((Bo2 e) => e.Mtotiva).HasColumnType("numeric(19, 6)").HasColumnName("mtotiva");
			entity.Property((Bo2 e) => e.Mttieca).HasColumnType("numeric(19, 6)").HasColumnName("mttieca");
			entity.Property((Bo2 e) => e.Ncin).HasColumnName("ncin");
			entity.Property((Bo2 e) => e.Ncout).HasColumnName("ncout");
			entity.Property((Bo2 e) => e.Ndosmanual).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ndosmanual")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Negocio).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("negocio")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ngstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ngstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ngstatus).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("ngstatus")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Nocts).HasColumnType("numeric(10, 0)").HasColumnName("nocts");
			entity.Property((Bo2 e) => e.Nomects).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nomects")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Nopkng).HasColumnType("numeric(8, 0)").HasColumnName("nopkng");
			entity.Property((Bo2 e) => e.Ntcm).HasColumnType("numeric(2, 0)").HasColumnName("ntcm");
			entity.Property((Bo2 e) => e.Obranomanual).HasColumnType("numeric(10, 0)").HasColumnName("obranomanual");
			entity.Property((Bo2 e) => e.Obranoorcamento).HasColumnType("numeric(10, 0)").HasColumnName("obranoorcamento");
			entity.Property((Bo2 e) => e.Orcamento).HasColumnName("orcamento");
			entity.Property((Bo2 e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo2 e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Pdtipo).HasColumnType("numeric(1, 0)").HasColumnName("pdtipo");
			entity.Property((Bo2 e) => e.Planeamento).HasColumnName("planeamento");
			entity.Property((Bo2 e) => e.Processo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("processo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Pscm).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("pscm")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Pscmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("pscmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Pscmori).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("pscmori")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Pscmoridesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("pscmoridesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ptcm).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("ptcm")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ptcmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("ptcmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Reorcamento).HasColumnName("reorcamento");
			entity.Property((Bo2 e) => e.Stamporcamento).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamporcamento")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Subproc).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("subproc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Sujrvp).HasColumnName("sujrvp");
			entity.Property((Bo2 e) => e.Szzstamp1).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("szzstamp1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Szzstamp2).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("szzstamp2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tbrsemmed).HasColumnName("tbrsemmed");
			entity.Property((Bo2 e) => e.Telefone).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("telefone")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tipoobra).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("tipoobra")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tiposaft).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("tiposaft")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhcarr).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tkhcarr")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhdata).HasColumnType("datetime").HasColumnName("tkhdata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo2 e) => e.Tkhdid).HasColumnType("numeric(10, 0)").HasColumnName("tkhdid");
			entity.Property((Bo2 e) => e.Tkhhora).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("tkhhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhid).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tkhid")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhlpnt).HasColumnType("numeric(15, 5)").HasColumnName("tkhlpnt");
			entity.Property((Bo2 e) => e.Tkhlref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tkhlref")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhltyp).HasColumnType("numeric(3, 0)").HasColumnName("tkhltyp");
			entity.Property((Bo2 e) => e.Tkhodo).HasColumnType("numeric(8, 0)").HasColumnName("tkhodo");
			entity.Property((Bo2 e) => e.Tkhopid).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tkhopid")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhpan).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("tkhpan")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhposcstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tkhposcstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tkhref")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhshf).HasColumnType("numeric(6, 0)").HasColumnName("tkhshf");
			entity.Property((Bo2 e) => e.Tkhsttnr).HasMaxLength(6).IsUnicode(unicode: false)
				.HasColumnName("tkhsttnr")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Tkhtyp).HasColumnType("numeric(1, 0)").HasColumnName("tkhtyp");
			entity.Property((Bo2 e) => e.Totalciva).HasColumnType("numeric(18, 5)").HasColumnName("totalciva");
			entity.Property((Bo2 e) => e.Totiva).HasColumnType("numeric(19, 6)").HasColumnName("totiva");
			entity.Property((Bo2 e) => e.Ttecoval).HasColumnType("numeric(18, 5)").HasColumnName("ttecoval");
			entity.Property((Bo2 e) => e.Ttecoval2).HasColumnType("numeric(18, 5)").HasColumnName("ttecoval2");
			entity.Property((Bo2 e) => e.Ttieca).HasColumnType("numeric(18, 5)").HasColumnName("ttieca");
			entity.Property((Bo2 e) => e.UAgentec).HasColumnType("text").HasColumnName("u_agentec")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UAnexo).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("u_anexo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UAvaliado).HasColumnName("u_avaliado");
			entity.Property((Bo2 e) => e.UBillof4).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("u_billof4")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UBl).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_bl")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UCab).HasColumnName("u_cab");
			entity.Property((Bo2 e) => e.UCarregad).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_carregad")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UCartacon).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_cartacon")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UChegou).HasColumnName("u_chegou");
			entity.Property((Bo2 e) => e.UColaborr).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_colaborr")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UComboio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_comboio")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UConta).HasColumnType("text").HasColumnName("u_conta")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UCpoc1).HasColumnType("numeric(6, 0)").HasColumnName("u_cpoc1");
			entity.Property((Bo2 e) => e.UDesc).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_desc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDestino).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_destino")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDestino1).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_destino1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDestino2).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_destino2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDestino3).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_destino3")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDisponib).HasColumnType("text").HasColumnName("u_disponib")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDobra).HasColumnType("datetime").HasColumnName("u_dobra")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo2 e) => e.UDproject).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_dproject")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UDtopera).HasColumnType("datetime").HasColumnName("u_dtopera")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo2 e) => e.UDvalidad).HasColumnType("datetime").HasColumnName("u_dvalidad")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo2 e) => e.UEir).HasColumnName("u_eir");
			entity.Property((Bo2 e) => e.UFluxo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_fluxo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UFtacess).HasColumnName("u_ftacess");
			entity.Property((Bo2 e) => e.UFtconten).HasColumnName("u_ftconten");
			entity.Property((Bo2 e) => e.UJapedido).HasColumnName("u_japedido");
			entity.Property((Bo2 e) => e.UJustif).HasColumnType("text").HasColumnName("u_justif")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.ULtrigger).HasColumnName("U_LTRIGGER");
			entity.Property((Bo2 e) => e.UMatricul).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("u_matricul")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UMotivo).HasColumnType("text").HasColumnName("u_motivo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UMtstamp).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_mtstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UNdos).HasColumnType("numeric(3, 0)").HasColumnName("u_ndos");
			entity.Property((Bo2 e) => e.UNmpe).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("u_nmpe")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UNocolabo).HasColumnType("numeric(10, 0)").HasColumnName("u_nocolabo");
			entity.Property((Bo2 e) => e.UNope).HasColumnType("numeric(16, 0)").HasColumnName("u_nope");
			entity.Property((Bo2 e) => e.UNprst).HasColumnType("numeric(6, 0)").HasColumnName("u_nprst");
			entity.Property((Bo2 e) => e.UOrigem).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_origem")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UOrigem1).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_origem1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UOrigem2).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_origem2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UOrigem3).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_origem3")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UPartil1).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_partil1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UPartil2).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_partil2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UPartil3).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_partil3")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UPorto).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_porto")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UPorverf).HasColumnName("u_porverf");
			entity.Property((Bo2 e) => e.URefcarta).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("u_refcarta")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.USaldoa).HasColumnType("text").HasColumnName("u_saldoa")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.USaldoexi).HasColumnType("text").HasColumnName("u_saldoexi")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.USaldofin).HasColumnType("text").HasColumnName("u_saldofin")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UTad).HasColumnName("u_tad");
			entity.Property((Bo2 e) => e.UTarifa).HasColumnType("numeric(12, 0)").HasColumnName("u_tarifa");
			entity.Property((Bo2 e) => e.UTipcod).HasColumnType("numeric(10, 0)").HasColumnName("u_tipcod");
			entity.Property((Bo2 e) => e.UTipocheg).HasMaxLength(1).IsUnicode(unicode: false)
				.HasColumnName("u_tipocheg")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UTipoop).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("u_tipoop")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UTipres).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("u_tipres")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UTransp).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_transp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UTtqtdvag).HasColumnType("numeric(12, 3)").HasColumnName("u_ttqtdvag");
			entity.Property((Bo2 e) => e.UTtvag).HasColumnType("numeric(12, 0)").HasColumnName("u_ttvag");
			entity.Property((Bo2 e) => e.UValorreq).HasColumnType("text").HasColumnName("u_valorreq")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UViagem).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_viagem")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.UViagemc).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_viagemc")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Usaintra).HasColumnName("usaintra");
			entity.Property((Bo2 e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo2 e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Vencimento).HasColumnType("numeric(5, 0)").HasColumnName("vencimento");
			entity.Property((Bo2 e) => e.Versaochave).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("versaochave")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Versaocrono).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("versaocrono")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Versaorcamento).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("versaorcamento")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Xpddata).HasColumnType("datetime").HasColumnName("xpddata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo2 e) => e.Xpdhora).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("xpdhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Xpdviatura).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("xpdviatura")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Zncm).HasColumnType("numeric(2, 0)").HasColumnName("zncm");
			entity.Property((Bo2 e) => e.Znregiao).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("znregiao")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Zona1).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("zona1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Zona2).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("zona2")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bo3> entity)
		{
			entity.HasKey((Bo3 e) => e.Bo3stamp).HasName("pk_bo3").IsClustered(clustered: false);
			entity.ToTable("bo3");
			entity.Property((Bo3 e) => e.Bo3stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bo3stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo3 e) => e.Anuldata).HasColumnType("datetime").HasColumnName("anuldata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo3 e) => e.Anulhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("anulhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Anulinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("anulinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Atcud).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("atcud")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Barcode).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("barcode")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Cobradovmbway).HasColumnName("cobradovmbway");
			entity.Property((Bo3 e) => e.Cobradovpaypal).HasColumnName("cobradovpaypal");
			entity.Property((Bo3 e) => e.Cobradovunicre).HasColumnName("cobradovunicre");
			entity.Property((Bo3 e) => e.Codendereco).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("codendereco")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Codeuserpay).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("codeuserpay")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Codmotiseimp).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("codmotiseimp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Codpais).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("codpais")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Codpromo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codpromo")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Contingencia).HasColumnType("numeric(1, 0)").HasColumnName("contingencia");
			entity.Property((Bo3 e) => e.Descpais).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("descpais")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Distrito).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("distrito")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Documentnumberori).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("documentnumberori")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Easypayasyncid).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("easypayasyncid")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Entidademb).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("entidademb")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Etotalmb).HasColumnType("numeric(19, 6)").HasColumnName("etotalmb");
			entity.Property((Bo3 e) => e.Latitude).HasColumnType("numeric(10, 6)").HasColumnName("latitude");
			entity.Property((Bo3 e) => e.Latitudecarga).HasColumnType("numeric(10, 6)").HasColumnName("latitudecarga");
			entity.Property((Bo3 e) => e.Longitude).HasColumnType("numeric(10, 6)").HasColumnName("longitude");
			entity.Property((Bo3 e) => e.Longitudecarga).HasColumnType("numeric(10, 6)").HasColumnName("longitudecarga");
			entity.Property((Bo3 e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Bo3 e) => e.Meiotranscv).HasColumnType("numeric(1, 0)").HasColumnName("meiotranscv");
			entity.Property((Bo3 e) => e.Motanul).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("motanul")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Motiseimp).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("motiseimp")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Motorista).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("motorista")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Nomeat).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("nomeat")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Onlinepay).HasColumnName("onlinepay");
			entity.Property((Bo3 e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo3 e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Pagomb).HasColumnName("pagomb");
			entity.Property((Bo3 e) => e.Pncont).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("pncont")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Refmb1).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("refmb1")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Refmb2).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("refmb2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Refmb3).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("refmb3")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Taxpointdt).HasColumnType("datetime").HasColumnName("taxpointdt")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo3 e) => e.Telefone).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("telefone")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Totalmb).HasColumnType("numeric(18, 5)").HasColumnName("totalmb");
			entity.Property((Bo3 e) => e.UData).HasColumnType("datetime").HasColumnName("u_data")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo3 e) => e.UDtfproc).HasColumnType("datetime").HasColumnName("u_dtfproc")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo3 e) => e.UDtiproc).HasColumnType("datetime").HasColumnName("u_dtiproc")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo3 e) => e.UDtiprov).HasColumnType("datetime").HasColumnName("u_dtiprov")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bo3 e) => e.UGsync).HasColumnName("u_gsync");
			entity.Property((Bo3 e) => e.UNdos).HasColumnType("numeric(3, 0)").HasColumnName("u_ndos");
			entity.Property((Bo3 e) => e.UNotific).HasColumnName("u_notific");
			entity.Property((Bo3 e) => e.UObs2).HasColumnType("text").HasColumnName("u_obs2")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UProvider).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_provider")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.USaldobo).HasColumnType("numeric(16, 4)").HasColumnName("u_saldobo");
			entity.Property((Bo3 e) => e.UStatus).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_status")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.USync).HasColumnName("u_sync");
			entity.Property((Bo3 e) => e.UTarifa).HasColumnType("numeric(12, 0)").HasColumnName("u_tarifa");
			entity.Property((Bo3 e) => e.UTotalrd).HasColumnType("numeric(16, 4)").HasColumnName("u_totalrd");
			entity.Property((Bo3 e) => e.UTtbiva).HasColumnType("numeric(16, 5)").HasColumnName("u_ttbiva");
			entity.Property((Bo3 e) => e.UTtdesc).HasColumnType("numeric(16, 5)").HasColumnName("u_ttdesc");
			entity.Property((Bo3 e) => e.UTtdescf).HasColumnType("numeric(16, 5)").HasColumnName("u_ttdescf");
			entity.Property((Bo3 e) => e.UTtiliq).HasColumnType("numeric(16, 5)").HasColumnName("u_ttiliq");
			entity.Property((Bo3 e) => e.UTtiva).HasColumnType("numeric(16, 5)").HasColumnName("u_ttiva");
			entity.Property((Bo3 e) => e.UTtnotiva).HasColumnType("numeric(16, 5)").HasColumnName("u_ttnotiva");
			entity.Property((Bo3 e) => e.UTtsubtot).HasColumnType("numeric(16, 5)").HasColumnName("u_ttsubtot");
			entity.Property((Bo3 e) => e.UTttotal).HasColumnType("numeric(16, 5)").HasColumnName("u_tttotal");
			entity.Property((Bo3 e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo3 e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bi> entity)
		{
			entity.HasKey((Bi e) => e.Bistamp).HasName("pk_bi").IsClustered(clustered: false);
			entity.ToTable("bi");
			entity.HasIndex((Bi e) => new { e.Bostamp, e.Lordem }, "in_bi_bostamp").HasFillFactor(80);
			entity.HasIndex((Bi e) => e.Forref, "in_bi_forref").HasFillFactor(80);
			entity.HasIndex((Bi e) => e.Lrecno, "in_bi_lrecno").HasFillFactor(80);
			entity.HasIndex((Bi e) => e.No, "in_bi_no").HasFillFactor(80);
			entity.HasIndex((Bi e) => e.Obistamp, "in_bi_obistamp").HasFillFactor(80);
			entity.HasIndex((Bi e) => e.Oobistamp, "in_bi_oobistamp").HasFillFactor(80);
			entity.HasIndex((Bi e) => e.Ref, "in_bi_ref").HasFillFactor(80);
			entity.Property((Bi e) => e.Bistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bistamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Adjudicada).HasColumnName("adjudicada");
			entity.Property((Bi e) => e.Adoc).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("adoc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Altura).HasColumnType("numeric(13, 3)").HasColumnName("altura");
			entity.Property((Bi e) => e.Ar2mazem).HasColumnType("numeric(5, 0)").HasColumnName("ar2mazem");
			entity.Property((Bi e) => e.Armazem).HasColumnType("numeric(5, 0)").HasColumnName("armazem");
			entity.Property((Bi e) => e.Atedata).HasColumnType("datetime").HasColumnName("atedata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Bifref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("bifref")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Binum1).HasColumnType("numeric(10, 0)").HasColumnName("binum1");
			entity.Property((Bi e) => e.Binum2).HasColumnType("numeric(10, 0)").HasColumnName("binum2");
			entity.Property((Bi e) => e.Biserie).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("biserie")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Boclose).HasColumnName("boclose");
			entity.Property((Bi e) => e.Bofref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("bofref")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Cativo).HasColumnName("cativo");
			entity.Property((Bi e) => e.Ccusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ccusto")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Classif).HasColumnType("numeric(1, 0)").HasColumnName("classif");
			entity.Property((Bi e) => e.Classifc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("classifc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Codfiscal).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("codfiscal")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Codigo).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Codpost).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("codpost")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Composto).HasColumnName("composto");
			entity.Property((Bi e) => e.Compostoori).HasColumnName("compostoori");
			entity.Property((Bi e) => e.Cor).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("cor")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Cpoc).HasColumnType("numeric(6, 0)").HasColumnName("cpoc");
			entity.Property((Bi e) => e.Custoind).HasColumnType("numeric(18, 5)").HasColumnName("custoind");
			entity.Property((Bi e) => e.Datafecho).HasColumnType("datetime").HasColumnName("datafecho")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Datafinal).HasColumnType("datetime").HasColumnName("datafinal")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Dataobra).HasColumnType("datetime").HasColumnName("dataobra")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Dataopen).HasColumnType("datetime").HasColumnName("dataopen")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Debito).HasColumnType("numeric(18, 5)").HasColumnName("debito");
			entity.Property((Bi e) => e.Debitoori).HasColumnType("numeric(18, 5)").HasColumnName("debitoori");
			entity.Property((Bi e) => e.Dedata).HasColumnType("datetime").HasColumnName("dedata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Desc2).HasColumnType("numeric(5, 2)").HasColumnName("desc2");
			entity.Property((Bi e) => e.Desc3).HasColumnType("numeric(5, 2)").HasColumnName("desc3");
			entity.Property((Bi e) => e.Desc4).HasColumnType("numeric(5, 2)").HasColumnName("desc4");
			entity.Property((Bi e) => e.Desc5).HasColumnType("numeric(5, 2)").HasColumnName("desc5");
			entity.Property((Bi e) => e.Desc6).HasColumnType("numeric(5, 2)").HasColumnName("desc6");
			entity.Property((Bi e) => e.Descli).HasColumnName("descli");
			entity.Property((Bi e) => e.Desconto).HasColumnType("numeric(6, 2)").HasColumnName("desconto");
			entity.Property((Bi e) => e.Design).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("design")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Dgeral).HasColumnType("text").HasColumnName("dgeral")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Dtclose).HasColumnType("datetime").HasColumnName("dtclose")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Econotcalc).HasColumnName("econotcalc");
			entity.Property((Bi e) => e.Ecoval).HasColumnType("numeric(18, 5)").HasColumnName("ecoval");
			entity.Property((Bi e) => e.Ecoval2).HasColumnType("numeric(18, 5)").HasColumnName("ecoval2");
			entity.Property((Bi e) => e.Ecustoind).HasColumnType("numeric(19, 6)").HasColumnName("ecustoind");
			entity.Property((Bi e) => e.Edebito).HasColumnType("numeric(19, 6)").HasColumnName("edebito");
			entity.Property((Bi e) => e.Edebitoori).HasColumnType("numeric(19, 6)").HasColumnName("edebitoori");
			entity.Property((Bi e) => e.Eecoval).HasColumnType("numeric(19, 6)").HasColumnName("eecoval");
			entity.Property((Bi e) => e.Eecoval2).HasColumnType("numeric(19, 6)").HasColumnName("eecoval2");
			entity.Property((Bi e) => e.Eencargo).HasColumnType("numeric(19, 6)").HasColumnName("eencargo");
			entity.Property((Bi e) => e.Efornec).HasColumnType("numeric(10, 0)").HasColumnName("efornec");
			entity.Property((Bi e) => e.Efornecedor).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("efornecedor")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Efornestab).HasColumnType("numeric(3, 0)").HasColumnName("efornestab");
			entity.Property((Bi e) => e.Emconf).HasColumnName("emconf");
			entity.Property((Bi e) => e.Encargo).HasColumnType("numeric(18, 5)").HasColumnName("encargo");
			entity.Property((Bi e) => e.Encvest).HasColumnType("numeric(19, 6)").HasColumnName("encvest");
			entity.Property((Bi e) => e.Epcusto).HasColumnType("numeric(19, 6)").HasColumnName("epcusto");
			entity.Property((Bi e) => e.Epromo).HasColumnName("epromo");
			entity.Property((Bi e) => e.Eprorc).HasColumnType("numeric(19, 6)").HasColumnName("eprorc");
			entity.Property((Bi e) => e.Epu).HasColumnType("numeric(19, 6)").HasColumnName("epu");
			entity.Property((Bi e) => e.Esltt).HasColumnType("numeric(19, 6)").HasColumnName("esltt");
			entity.Property((Bi e) => e.Eslvu).HasColumnType("numeric(19, 6)").HasColumnName("eslvu");
			entity.Property((Bi e) => e.Espessura).HasColumnType("numeric(13, 3)").HasColumnName("espessura");
			entity.Property((Bi e) => e.Estab).HasColumnType("numeric(3, 0)").HasColumnName("estab");
			entity.Property((Bi e) => e.Etecoval).HasColumnType("numeric(19, 6)").HasColumnName("etecoval");
			entity.Property((Bi e) => e.Etecoval2).HasColumnType("numeric(19, 6)").HasColumnName("etecoval2");
			entity.Property((Bi e) => e.Etieca).HasColumnType("numeric(19, 6)").HasColumnName("etieca");
			entity.Property((Bi e) => e.Ettdeb).HasColumnType("numeric(19, 6)").HasColumnName("ettdeb");
			entity.Property((Bi e) => e.Evaldesc).HasColumnType("numeric(19, 6)").HasColumnName("evaldesc");
			entity.Property((Bi e) => e.Fami).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("fami")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Familia).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("familia")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Fdata).HasColumnType("datetime").HasColumnName("fdata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Fechabo).HasColumnName("fechabo");
			entity.Property((Bi e) => e.Fechada).HasColumnName("fechada");
			entity.Property((Bi e) => e.Fmarcada).HasColumnName("fmarcada");
			entity.Property((Bi e) => e.Fno).HasColumnType("numeric(10, 0)").HasColumnName("fno");
			entity.Property((Bi e) => e.Forref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("forref")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Grau).HasColumnType("numeric(9, 3)").HasColumnName("grau");
			entity.Property((Bi e) => e.Iecacodisen).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("iecacodisen")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Iecagrad).HasColumnType("numeric(7, 3)").HasColumnName("iecagrad");
			entity.Property((Bi e) => e.Iecasug).HasColumnName("iecasug");
			entity.Property((Bi e) => e.Infref).HasColumnName("infref");
			entity.Property((Bi e) => e.Iprint).HasColumnName("iprint");
			entity.Property((Bi e) => e.Iva).HasColumnType("numeric(5, 2)").HasColumnName("iva");
			entity.Property((Bi e) => e.Ivaincl).HasColumnName("ivaincl");
			entity.Property((Bi e) => e.Largura).HasColumnType("numeric(13, 3)").HasColumnName("largura");
			entity.Property((Bi e) => e.Ldossier).HasColumnType("numeric(2, 0)").HasColumnName("ldossier");
			entity.Property((Bi e) => e.Lifref).HasColumnName("lifref");
			entity.Property((Bi e) => e.Litem).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("litem")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Litem2).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("litem2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Lobs).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("lobs")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Lobs2).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("lobs2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Lobs3).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("lobs3")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Local).HasMaxLength(43).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Lordem).HasColumnType("numeric(10, 0)").HasColumnName("lordem");
			entity.Property((Bi e) => e.Lote).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("lote")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Lrecno).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("lrecno")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ltab1).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ltab1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ltab2).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ltab2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ltab3).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ltab3")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ltab4).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ltab4")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ltab5).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ltab5")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Maquina).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("maquina")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Marca).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("marca")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Bi e) => e.Mntencargos).HasColumnName("mntencargos");
			entity.Property((Bi e) => e.Morada).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("morada")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Mtieca).HasColumnType("numeric(13, 3)").HasColumnName("mtieca");
			entity.Property((Bi e) => e.Nccod).HasMaxLength(13).IsUnicode(unicode: false)
				.HasColumnName("nccod")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ncinteg).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ncinteg")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ncmassa).HasColumnType("numeric(16, 3)").HasColumnName("ncmassa");
			entity.Property((Bi e) => e.Ncunsup).HasColumnType("numeric(14, 4)").HasColumnName("ncunsup");
			entity.Property((Bi e) => e.Ncusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ncusto")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ncvest).HasColumnType("numeric(18, 5)").HasColumnName("ncvest");
			entity.Property((Bi e) => e.Ndoc).HasColumnType("numeric(3, 0)").HasColumnName("ndoc");
			entity.Property((Bi e) => e.Ndos).HasColumnType("numeric(3, 0)").HasColumnName("ndos");
			entity.Property((Bi e) => e.Nmdoc).HasMaxLength(24).IsUnicode(unicode: false)
				.HasColumnName("nmdoc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Nmdos).HasMaxLength(24).IsUnicode(unicode: false)
				.HasColumnName("nmdos")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.No).HasColumnType("numeric(10, 0)").HasColumnName("no");
			entity.Property((Bi e) => e.Nome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Nomquina).HasColumnType("numeric(7, 0)").HasColumnName("nomquina");
			entity.Property((Bi e) => e.Nopat).HasColumnType("numeric(10, 0)").HasColumnName("nopat");
			entity.Property((Bi e) => e.Noserie).HasColumnName("noserie");
			entity.Property((Bi e) => e.Num1).HasColumnType("numeric(19, 5)").HasColumnName("num1");
			entity.Property((Bi e) => e.Obistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("obistamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Obrano).HasColumnType("numeric(10, 0)").HasColumnName("obrano");
			entity.Property((Bi e) => e.Obranome).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("obranome")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ofostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ofostamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Oftstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oftstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Oobistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oobistamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Oobostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oobostamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Optstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("optstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Oristamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oristamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Partes).HasColumnType("numeric(13, 3)").HasColumnName("partes");
			entity.Property((Bi e) => e.Partes2).HasColumnType("numeric(13, 3)").HasColumnName("partes2");
			entity.Property((Bi e) => e.Pbruto).HasColumnType("numeric(14, 3)").HasColumnName("pbruto");
			entity.Property((Bi e) => e.Pctfami).HasColumnType("numeric(6, 2)").HasColumnName("pctfami");
			entity.Property((Bi e) => e.Pcusto).HasColumnType("numeric(18, 5)").HasColumnName("pcusto");
			entity.Property((Bi e) => e.Peso).HasColumnType("numeric(14, 3)").HasColumnName("peso");
			entity.Property((Bi e) => e.Posic).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("posic")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Producao).HasColumnName("producao");
			entity.Property((Bi e) => e.Promo).HasColumnName("promo");
			entity.Property((Bi e) => e.Prorc).HasColumnType("numeric(14, 3)").HasColumnName("prorc");
			entity.Property((Bi e) => e.Pu).HasColumnType("numeric(18, 5)").HasColumnName("pu");
			entity.Property((Bi e) => e.Pvok).HasColumnName("pvok");
			entity.Property((Bi e) => e.Qtt).HasColumnType("numeric(14, 4)").HasColumnName("qtt");
			entity.Property((Bi e) => e.Qtt2).HasColumnType("numeric(14, 4)").HasColumnName("qtt2");
			entity.Property((Bi e) => e.Quarto).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("quarto")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Rdata).HasColumnType("datetime").HasColumnName("rdata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi e) => e.Ref).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Reff).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("reff")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Rescli).HasColumnName("rescli");
			entity.Property((Bi e) => e.Resfor).HasColumnName("resfor");
			entity.Property((Bi e) => e.Resrec).HasColumnName("resrec");
			entity.Property((Bi e) => e.Resusr).HasColumnName("resusr");
			entity.Property((Bi e) => e.Sattotal).HasColumnName("sattotal");
			entity.Property((Bi e) => e.Segmento).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("segmento")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Serie).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("serie")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Series).HasColumnType("text").HasColumnName("series")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Series2).HasColumnType("text").HasColumnName("series2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Sltt).HasColumnType("numeric(18, 5)").HasColumnName("sltt");
			entity.Property((Bi e) => e.Slttmoeda).HasColumnType("numeric(15, 2)").HasColumnName("slttmoeda");
			entity.Property((Bi e) => e.Slvu).HasColumnType("numeric(18, 5)").HasColumnName("slvu");
			entity.Property((Bi e) => e.Slvumoeda).HasColumnType("numeric(15, 2)").HasColumnName("slvumoeda");
			entity.Property((Bi e) => e.Stipo).HasColumnType("numeric(1, 0)").HasColumnName("stipo");
			entity.Property((Bi e) => e.Stns).HasColumnName("stns");
			entity.Property((Bi e) => e.Tabela1).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tabela1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Tabfor).HasColumnName("tabfor");
			entity.Property((Bi e) => e.Tabiva).HasColumnType("numeric(1, 0)").HasColumnName("tabiva");
			entity.Property((Bi e) => e.Tam).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tam")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Tecnico).HasColumnType("numeric(4, 0)").HasColumnName("tecnico");
			entity.Property((Bi e) => e.Tecoval).HasColumnType("numeric(18, 5)").HasColumnName("tecoval");
			entity.Property((Bi e) => e.Tecoval2).HasColumnType("numeric(18, 5)").HasColumnName("tecoval2");
			entity.Property((Bi e) => e.Temeco).HasColumnName("temeco");
			entity.Property((Bi e) => e.Temoci).HasColumnName("temoci");
			entity.Property((Bi e) => e.Temomi).HasColumnName("temomi");
			entity.Property((Bi e) => e.Temsubemp).HasColumnName("temsubemp");
			entity.Property((Bi e) => e.Texteis).HasColumnName("texteis");
			entity.Property((Bi e) => e.Tieca).HasColumnType("numeric(18, 5)").HasColumnName("tieca");
			entity.Property((Bi e) => e.Tiposemp).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tiposemp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Tpromo).HasColumnType("numeric(1, 0)").HasColumnName("tpromo");
			entity.Property((Bi e) => e.Trocaequi).HasColumnName("trocaequi");
			entity.Property((Bi e) => e.Ttdeb).HasColumnType("numeric(18, 5)").HasColumnName("ttdeb");
			entity.Property((Bi e) => e.Ttmoeda).HasColumnType("numeric(12, 2)").HasColumnName("ttmoeda");
			entity.Property((Bi e) => e.Txiva).HasColumnType("numeric(2, 0)").HasColumnName("txiva");
			entity.Property((Bi e) => e.UBi).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("u_bi")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UBicam).HasColumnType("numeric(16, 2)").HasColumnName("u_bicam");
			entity.Property((Bi e) => e.UBitot).HasColumnType("numeric(16, 2)").HasColumnName("u_bitot");
			entity.Property((Bi e) => e.UCambfixo).HasColumnName("u_cambfixo");
			entity.Property((Bi e) => e.UCif).HasColumnType("numeric(16, 2)").HasColumnName("u_cif");
			entity.Property((Bi e) => e.UConta).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_conta")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UDescobri).HasColumnName("u_descobri");
			entity.Property((Bi e) => e.UEquip).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("u_equip")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UGastoite).HasColumnType("numeric(16, 2)").HasColumnName("u_gastoite");
			entity.Property((Bi e) => e.UGastos).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_gastos")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UIdpacien).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("u_idpacien")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UItobrig).HasColumnName("u_itobrig");
			entity.Property((Bi e) => e.UKm).HasColumnType("numeric(16, 0)").HasColumnName("u_km");
			entity.Property((Bi e) => e.ULogi1).HasColumnName("u_logi1");
			entity.Property((Bi e) => e.ULtrigger).HasColumnName("U_LTRIGGER");
			entity.Property((Bi e) => e.UMancod).HasColumnType("numeric(16, 0)").HasColumnName("u_mancod");
			entity.Property((Bi e) => e.UMcam).HasColumnType("numeric(16, 2)").HasColumnName("u_mcam");
			entity.Property((Bi e) => e.UMcstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_mcstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UMlogi1).HasColumnType("text").HasColumnName("u_mlogi1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UMoeda).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_moeda")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UNmpacien).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("u_nmpacien")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UOlcodigo).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("u_olcodigo")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UOperacao).HasColumnType("numeric(5, 0)").HasColumnName("u_operacao");
			entity.Property((Bi e) => e.UPapagar).HasColumnName("u_papagar");
			entity.Property((Bi e) => e.UPct).HasColumnType("numeric(16, 2)").HasColumnName("u_pct");
			entity.Property((Bi e) => e.UPctg).HasColumnType("numeric(16, 2)").HasColumnName("u_pctg");
			entity.Property((Bi e) => e.UPeso).HasColumnType("numeric(12, 2)").HasColumnName("u_peso");
			entity.Property((Bi e) => e.UPrecoadm).HasColumnType("numeric(16, 5)").HasColumnName("u_precoadm");
			entity.Property((Bi e) => e.URefcarta).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_refcarta")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.URefctt).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_refctt")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UReferen).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_referen")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.USaldo).HasColumnType("numeric(16, 2)").HasColumnName("u_saldo");
			entity.Property((Bi e) => e.USaldoite).HasColumnType("numeric(16, 2)").HasColumnName("u_saldoite");
			entity.Property((Bi e) => e.UStampart).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_stampart")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UTcarga).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_tcarga")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UTipest).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_tipest")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UTxcambio).HasColumnType("numeric(16, 2)").HasColumnName("u_txcambio");
			entity.Property((Bi e) => e.UValoradm).HasColumnType("numeric(16, 5)").HasColumnName("u_valoradm");
			entity.Property((Bi e) => e.UValortd).HasColumnType("numeric(16, 2)").HasColumnName("u_valortd");
			entity.Property((Bi e) => e.UViatura).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_viatura")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.UVmoeda).HasColumnType("numeric(16, 2)").HasColumnName("u_vmoeda");
			entity.Property((Bi e) => e.UVolume).HasColumnType("numeric(12, 2)").HasColumnName("u_volume");
			entity.Property((Bi e) => e.Uni2qtt).HasColumnType("numeric(14, 4)").HasColumnName("uni2qtt");
			entity.Property((Bi e) => e.Unidad2).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("unidad2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Unidade).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("unidade")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usalote).HasColumnName("usalote");
			entity.Property((Bi e) => e.Usr1).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("usr1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usr2).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("usr2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usr3).HasMaxLength(35).IsUnicode(unicode: false)
				.HasColumnName("usr3")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usr4).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("usr4")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usr5).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("usr5")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usr6).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usr6")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Valdesc).HasColumnType("numeric(18, 5)").HasColumnName("valdesc");
			entity.Property((Bi e) => e.Vendedor).HasColumnType("numeric(4, 0)").HasColumnName("vendedor");
			entity.Property((Bi e) => e.Vendnm).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("vendnm")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Volume).HasColumnType("numeric(11, 3)").HasColumnName("volume");
			entity.Property((Bi e) => e.Vumoeda).HasColumnType("numeric(19, 6)").HasColumnName("vumoeda");
			entity.Property((Bi e) => e.Zona).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("zona")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bi2> entity)
		{
			entity.HasKey((Bi2 e) => e.Bi2stamp).HasName("pk_bi2").IsClustered(clustered: false);
			entity.ToTable("bi2");
			entity.HasIndex((Bi2 e) => e.Bostamp, "in_bi2_bostamp").HasFillFactor(80);
			entity.Property((Bi2 e) => e.Bi2stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bi2stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Alvstamp1).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("alvstamp1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Alvstamp2).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("alvstamp2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Atal).HasColumnType("numeric(5, 0)").HasColumnName("atal");
			entity.Property((Bi2 e) => e.Ataldesc).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("ataldesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Aviado).HasColumnName("aviado");
			entity.Property((Bi2 e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Cladrsdesc).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("cladrsdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Cladrsstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("cladrsstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Cladrszona).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("cladrszona")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Codpost).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("codpost")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Contacto).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("contacto")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Custoinddif).HasColumnType("numeric(18, 5)").HasColumnName("custoinddif");
			entity.Property((Bi2 e) => e.Custoinddifperc).HasColumnType("numeric(15, 2)").HasColumnName("custoinddifperc");
			entity.Property((Bi2 e) => e.Custoindorcamento).HasColumnType("numeric(18, 5)").HasColumnName("custoindorcamento");
			entity.Property((Bi2 e) => e.Custototaldif).HasColumnType("numeric(18, 5)").HasColumnName("custototaldif");
			entity.Property((Bi2 e) => e.Custototaldifperc).HasColumnType("numeric(15, 2)").HasColumnName("custototaldifperc");
			entity.Property((Bi2 e) => e.Custototalorcamento).HasColumnType("numeric(18, 5)").HasColumnName("custototalorcamento");
			entity.Property((Bi2 e) => e.Descfx).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("descfx")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Desemb).HasColumnName("desemb");
			entity.Property((Bi2 e) => e.Ecustoinddif).HasColumnType("numeric(19, 6)").HasColumnName("ecustoinddif");
			entity.Property((Bi2 e) => e.Ecustoindorcamento).HasColumnType("numeric(19, 6)").HasColumnName("ecustoindorcamento");
			entity.Property((Bi2 e) => e.Ecustototaldif).HasColumnType("numeric(19, 6)").HasColumnName("ecustototaldif");
			entity.Property((Bi2 e) => e.Ecustototalorcamento).HasColumnType("numeric(19, 6)").HasColumnName("ecustototalorcamento");
			entity.Property((Bi2 e) => e.EftaxamtA).HasColumnType("numeric(19, 6)").HasColumnName("eftaxamt_a");
			entity.Property((Bi2 e) => e.EftaxamtB).HasColumnType("numeric(19, 6)").HasColumnName("eftaxamt_b");
			entity.Property((Bi2 e) => e.Email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("email")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Emargemdif).HasColumnType("numeric(19, 6)").HasColumnName("emargemdif");
			entity.Property((Bi2 e) => e.Emargemorcamento).HasColumnType("numeric(19, 6)").HasColumnName("emargemorcamento");
			entity.Property((Bi2 e) => e.Eqttaprval).HasColumnType("numeric(19, 6)").HasColumnName("eqttaprval");
			entity.Property((Bi2 e) => e.Eqttfaltaval).HasColumnType("numeric(19, 6)").HasColumnName("eqttfaltaval");
			entity.Property((Bi2 e) => e.Eqttmedidaval).HasColumnType("numeric(19, 6)").HasColumnName("eqttmedidaval");
			entity.Property((Bi2 e) => e.Eqttnoaprval).HasColumnType("numeric(19, 6)").HasColumnName("eqttnoaprval");
			entity.Property((Bi2 e) => e.Etbrmaisval).HasColumnType("numeric(19, 6)").HasColumnName("etbrmaisval");
			entity.Property((Bi2 e) => e.Etbrmenosval).HasColumnType("numeric(19, 6)").HasColumnName("etbrmenosval");
			entity.Property((Bi2 e) => e.Evalnew).HasColumnType("numeric(19, 6)").HasColumnName("evalnew");
			entity.Property((Bi2 e) => e.Fechabosatisteito).HasColumnName("fechabosatisteito");
			entity.Property((Bi2 e) => e.Fistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("fistamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Fnstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("fnstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Foadoc).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("foadoc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Fodocnome).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("fodocnome")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.FtaxamtA).HasColumnType("numeric(18, 5)").HasColumnName("ftaxamt_a");
			entity.Property((Bi2 e) => e.FtaxamtB).HasColumnType("numeric(18, 5)").HasColumnName("ftaxamt_b");
			entity.Property((Bi2 e) => e.Identificacao1).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("identificacao1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Identificacao2).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("identificacao2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Lobsauto).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("lobsauto")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Local).HasMaxLength(43).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Bi2 e) => e.Marcadacustoloja).HasColumnName("marcadacustoloja");
			entity.Property((Bi2 e) => e.Margemdif).HasColumnType("numeric(18, 5)").HasColumnName("margemdif");
			entity.Property((Bi2 e) => e.Margemorcamento).HasColumnType("numeric(18, 5)").HasColumnName("margemorcamento");
			entity.Property((Bi2 e) => e.Mcomercial).HasColumnType("numeric(6, 2)").HasColumnName("mcomercial");
			entity.Property((Bi2 e) => e.Morada).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("morada")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Noaprov).HasColumnType("numeric(14, 4)").HasColumnName("noaprov");
			entity.Property((Bi2 e) => e.Noaprov2).HasColumnType("numeric(14, 4)").HasColumnName("noaprov2");
			entity.Property((Bi2 e) => e.Ofcstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ofcstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Ooboano).HasColumnType("numeric(4, 0)").HasColumnName("ooboano");
			entity.Property((Bi2 e) => e.Oonmdos).HasMaxLength(24).IsUnicode(unicode: false)
				.HasColumnName("oonmdos")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Ooobrano).HasColumnType("numeric(10, 0)").HasColumnName("ooobrano");
			entity.Property((Bi2 e) => e.Ooobranome).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ooobranome")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Origbistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("origbistamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi2 e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Percnew).HasColumnType("numeric(15, 2)").HasColumnName("percnew");
			entity.Property((Bi2 e) => e.Precatstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("precatstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Promoordem).HasColumnType("numeric(10, 0)").HasColumnName("promoordem");
			entity.Property((Bi2 e) => e.Pscmori).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("pscmori")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Pscmoridesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("pscmoridesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Qttadj).HasColumnType("numeric(14, 4)").HasColumnName("qttadj");
			entity.Property((Bi2 e) => e.Qttapr).HasColumnType("numeric(14, 4)").HasColumnName("qttapr");
			entity.Property((Bi2 e) => e.Qttaprval).HasColumnType("numeric(18, 5)").HasColumnName("qttaprval");
			entity.Property((Bi2 e) => e.Qttatrib).HasColumnType("numeric(14, 4)").HasColumnName("qttatrib");
			entity.Property((Bi2 e) => e.Qttcompra).HasColumnType("numeric(14, 4)").HasColumnName("qttcompra");
			entity.Property((Bi2 e) => e.Qttdif).HasColumnType("numeric(14, 4)").HasColumnName("qttdif");
			entity.Property((Bi2 e) => e.Qttdifperc).HasColumnType("numeric(15, 2)").HasColumnName("qttdifperc");
			entity.Property((Bi2 e) => e.Qttenc).HasColumnType("numeric(14, 4)").HasColumnName("qttenc");
			entity.Property((Bi2 e) => e.Qttfalta).HasColumnType("numeric(14, 4)").HasColumnName("qttfalta");
			entity.Property((Bi2 e) => e.Qttfaltaval).HasColumnType("numeric(18, 5)").HasColumnName("qttfaltaval");
			entity.Property((Bi2 e) => e.Qttmedida).HasColumnType("numeric(14, 4)").HasColumnName("qttmedida");
			entity.Property((Bi2 e) => e.Qttmedidaperc).HasColumnType("numeric(15, 2)").HasColumnName("qttmedidaperc");
			entity.Property((Bi2 e) => e.Qttmedidaval).HasColumnType("numeric(18, 5)").HasColumnName("qttmedidaval");
			entity.Property((Bi2 e) => e.Qttnew).HasColumnType("numeric(14, 4)").HasColumnName("qttnew");
			entity.Property((Bi2 e) => e.Qttnew2).HasColumnType("numeric(14, 4)").HasColumnName("qttnew2");
			entity.Property((Bi2 e) => e.Qttnoapr).HasColumnType("numeric(14, 4)").HasColumnName("qttnoapr");
			entity.Property((Bi2 e) => e.Qttnoaprval).HasColumnType("numeric(18, 5)").HasColumnName("qttnoaprval");
			entity.Property((Bi2 e) => e.Qttorcamento).HasColumnType("numeric(14, 4)").HasColumnName("qttorcamento");
			entity.Property((Bi2 e) => e.Qtttbrmais).HasColumnType("numeric(14, 4)").HasColumnName("qtttbrmais");
			entity.Property((Bi2 e) => e.Qtttbrmenos).HasColumnType("numeric(14, 4)").HasColumnName("qtttbrmenos");
			entity.Property((Bi2 e) => e.Semserprv).HasColumnName("semserprv");
			entity.Property((Bi2 e) => e.Szzstamp1).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("szzstamp1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Szzstamp2).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("szzstamp2")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Tbrmais).HasColumnType("numeric(14, 4)").HasColumnName("tbrmais");
			entity.Property((Bi2 e) => e.Tbrmaisval).HasColumnType("numeric(18, 5)").HasColumnName("tbrmaisval");
			entity.Property((Bi2 e) => e.Tbrmenos).HasColumnType("numeric(14, 4)").HasColumnName("tbrmenos");
			entity.Property((Bi2 e) => e.Tbrmenosval).HasColumnType("numeric(18, 5)").HasColumnName("tbrmenosval");
			entity.Property((Bi2 e) => e.Telefone).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("telefone")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Temfx).HasColumnName("temfx");
			entity.Property((Bi2 e) => e.Tkhcodcmb).HasColumnType("numeric(2, 0)").HasColumnName("tkhcodcmb");
			entity.Property((Bi2 e) => e.Tkhpmp).HasColumnType("numeric(10, 0)").HasColumnName("tkhpmp");
			entity.Property((Bi2 e) => e.Tkhposlstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tkhposlstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UAgente).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_agente")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UBaldeado).HasColumnName("u_baldeado");
			entity.Property((Bi2 e) => e.UBl).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_bl")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UBooking).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("u_booking")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UCancdata).HasColumnType("datetime").HasColumnName("u_cancdata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi2 e) => e.UCancelar).HasColumnName("u_cancelar");
			entity.Property((Bi2 e) => e.UCancjus).HasMaxLength(90).IsUnicode(unicode: false)
				.HasColumnName("u_cancjus")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UCancnome).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("u_cancnome")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UCarga).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_carga")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UCctbenef).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_cctbenef")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UChegou).HasColumnName("u_chegou");
			entity.Property((Bi2 e) => e.UClno).HasColumnType("numeric(6, 0)").HasColumnName("u_clno");
			entity.Property((Bi2 e) => e.UCod).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("u_cod")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UComboio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_comboio")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UConf).HasColumnName("u_conf");
			entity.Property((Bi2 e) => e.UConsignt).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_consignt")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UContram).HasMaxLength(7).IsUnicode(unicode: false)
				.HasColumnName("u_contram")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDeslocal).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_deslocal")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDfactura).HasColumnType("datetime").HasColumnName("u_dfactura")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi2 e) => e.UDtdesp).HasColumnType("datetime").HasColumnName("u_dtdesp")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi2 e) => e.UDtopera).HasColumnType("datetime").HasColumnName("u_dtopera")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Bi2 e) => e.UEmbarca).HasColumnName("u_embarca");
			entity.Property((Bi2 e) => e.UExpedido).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_expedido")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UFcllcl).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("u_fcllcl")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UFmstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fmstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UFrigo).HasColumnName("u_frigo");
			entity.Property((Bi2 e) => e.UFullempy).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("u_fullempy")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UHfim).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_hfim")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UHini).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_hini")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UIsocode).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("u_isocode")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.ULinha).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_linha")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UMaxx).HasColumnType("numeric(10, 0)").HasColumnName("u_maxx");
			entity.Property((Bi2 e) => e.UMinn).HasColumnType("numeric(10, 0)").HasColumnName("u_minn");
			entity.Property((Bi2 e) => e.UNcontent).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_ncontent")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UNdos).HasColumnType("numeric(3, 0)").HasColumnName("u_ndos");
			entity.Property((Bi2 e) => e.UNfactura).HasColumnType("numeric(12, 0)").HasColumnName("u_nfactura");
			entity.Property((Bi2 e) => e.UNoguia).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("u_noguia")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UNomecl).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_nomecl")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UNomnavio).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_nomnavio")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UNselo).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_nselo")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UOperacao).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("u_operacao")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UOpporto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_opporto")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPeso).HasColumnType("numeric(8, 2)").HasColumnName("u_peso");
			entity.Property((Bi2 e) => e.UPod).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_pod")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPol).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_pol")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPor001st).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_por001st")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPor013st).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_por013st")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPosnavio).HasColumnType("numeric(10, 0)").HasColumnName("u_posnavio");
			entity.Property((Bi2 e) => e.URefnavio).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_refnavio")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.USett).HasColumnType("numeric(10, 0)").HasColumnName("u_sett");
			entity.Property((Bi2 e) => e.USituacao).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_situacao")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UTara).HasColumnType("numeric(10, 0)").HasColumnName("u_tara");
			entity.Property((Bi2 e) => e.UTempunit).HasColumnType("numeric(3, 0)").HasColumnName("u_tempunit");
			entity.Property((Bi2 e) => e.UTpcarga).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_tpcarga")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UTpconten).HasColumnType("numeric(3, 0)").HasColumnName("u_tpconten");
			entity.Property((Bi2 e) => e.UTrafego).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("u_trafego")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UUnidade).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("u_unidade")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UVagao).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_vagao")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UViagem).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_viagem")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Usamcomercial).HasColumnName("usamcomercial");
			entity.Property((Bi2 e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi2 e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Valnew).HasColumnType("numeric(18, 5)").HasColumnName("valnew");
			entity.Property((Bi2 e) => e.Zona1).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("zona1")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Zona2).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("zona2")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UBovg> entity)
		{
			entity.HasKey((UBovg e) => e.UBovgstamp).HasName("pk_u_bovg").IsClustered(clustered: false);
			entity.ToTable("u_bovg");
			entity.Property((UBovg e) => e.UBovgstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_bovgstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UBovg e) => e.Admin).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UBovg e) => e.Carga).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Contentor1).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("contentor1")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Contentor2).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("contentor2")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Encerno).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("encerno")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UBovg e) => e.Marcas).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("marcas")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Ndos).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("ndos")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Novg).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("novg")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UBovg e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Qtd).HasColumnType("numeric(10, 3)").HasColumnName("qtd");
			entity.Property((UBovg e) => e.Qtd2).HasColumnType("numeric(10, 0)").HasColumnName("qtd2");
			entity.Property((UBovg e) => e.Stampevg).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampevg")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Tipo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UBovg e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UBovg e) => e.Vgno).HasColumnType("numeric(10, 0)").HasColumnName("vgno");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Cl> entity)
		{
			entity.HasKey((Cl e) => new { e.No, e.Estab }).HasName("pk_cl").IsClustered(clustered: false);
			entity.ToTable("cl");
			entity.HasIndex((Cl e) => new { e.Nome, e.Nome2, e.No, e.Estab, e.Clstamp }, "in_cl_cllist").HasFillFactor(80);
			entity.HasIndex((Cl e) => e.Ncont, "in_cl_ncont").HasFillFactor(80);
			entity.HasIndex((Cl e) => e.No, "in_cl_no").HasFillFactor(80);
			entity.HasIndex((Cl e) => e.Nome, "in_cl_nome").HasFillFactor(80);
			entity.HasIndex((Cl e) => e.Clstamp, "in_cl_stamp").IsUnique().HasFillFactor(80);
			entity.HasIndex((Cl e) => e.Vendedor, "in_cl_vendedor").HasFillFactor(80);
			entity.Property((Cl e) => e.No).HasColumnType("numeric(10, 0)").HasColumnName("no");
			entity.Property((Cl e) => e.Estab).HasColumnType("numeric(3, 0)").HasColumnName("estab");
			entity.Property((Cl e) => e.Acc).HasColumnType("numeric(1, 0)").HasColumnName("acc");
			entity.Property((Cl e) => e.Acmfact).HasColumnType("numeric(18, 5)").HasColumnName("acmfact");
			entity.Property((Cl e) => e.Addd).HasColumnName("addd");
			entity.Property((Cl e) => e.Agno).HasColumnType("numeric(10, 0)").HasColumnName("agno");
			entity.Property((Cl e) => e.Alimite).HasColumnType("numeric(5, 0)").HasColumnName("alimite");
			entity.Property((Cl e) => e.Area).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("area")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Cl e) => e.Autofact).HasColumnName("autofact");
			entity.Property((Cl e) => e.Autorizacaoactiva).HasColumnName("autorizacaoactiva");
			entity.Property((Cl e) => e.Bic).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("bic")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Bidata).HasColumnType("datetime").HasColumnName("bidata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Cl e) => e.Bilocal).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bilocal")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Bino).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("bino")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Bizzaddress).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("bizzaddress")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Bizzproto).HasColumnType("numeric(1, 0)").HasColumnName("bizzproto");
			entity.Property((Cl e) => e.Blck).HasColumnName("blck");
			entity.Property((Cl e) => e.C1email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("c1email")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C1fax).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("c1fax")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C1func).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("c1func")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C1tele).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("c1tele")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C2email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("c2email")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C2fax).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("c2fax")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C2func).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("c2func")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C2tacto).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("c2tacto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C2tele).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("c2tele")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C3email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("c3email")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C3fax).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("c3fax")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C3func).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("c3func")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C3tacto).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("c3tacto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.C3tele).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("c3tele")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cancpos).HasColumnName("cancpos");
			entity.Property((Cl e) => e.Carr).HasColumnName("carr");
			entity.Property((Cl e) => e.Cass).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("cass")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ccadmin).HasColumnName("ccadmin");
			entity.Property((Cl e) => e.Ccusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ccusto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Classe).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("classe")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Clifactor).HasColumnName("clifactor");
			entity.Property((Cl e) => e.Clinica).HasColumnName("clinica");
			entity.Property((Cl e) => e.Clivd).HasColumnName("clivd");
			entity.Property((Cl e) => e.Clstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("clstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Cl e) => e.Cobemail).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("cobemail")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cobfax).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("cobfax")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cobfunc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("cobfunc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cobnao).HasColumnName("cobnao");
			entity.Property((Cl e) => e.Cobrador).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("cobrador")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cobranca).HasMaxLength(22).IsUnicode(unicode: false)
				.HasColumnName("cobranca")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cobtacto).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("cobtacto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cobtele).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("cobtele")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Codfornecedor).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codfornecedor")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Codmotiseimp).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("codmotiseimp")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Codpost).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("codpost")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Codprovincia).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("codprovincia")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Consfinal).HasColumnName("consfinal");
			entity.Property((Cl e) => e.Conta).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("conta")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contaacer).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contaacer")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contaainc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contaainc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contacto).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("contacto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contadivinc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contadivinc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contado).HasColumnType("numeric(4, 0)").HasColumnName("contado");
			entity.Property((Cl e) => e.Contafac).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contafac")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contalet).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contalet")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contaletdes).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contaletdes")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contaletsac).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contaletsac")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contamovinc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contamovinc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Contatit).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contatit")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Cw).HasColumnName("cw");
			entity.Property((Cl e) => e.Datasdd).HasColumnType("datetime").HasColumnName("datasdd")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Cl e) => e.Descarga).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("descarga")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Desccmb).HasColumnType("numeric(10, 3)").HasColumnName("desccmb");
			entity.Property((Cl e) => e.Descloj).HasColumnType("numeric(6, 2)").HasColumnName("descloj");
			entity.Property((Cl e) => e.Desconto).HasColumnType("numeric(5, 2)").HasColumnName("desconto");
			entity.Property((Cl e) => e.Descpp).HasColumnType("numeric(5, 2)").HasColumnName("descpp");
			entity.Property((Cl e) => e.Descregiva).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("descregiva")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Dformacao).HasColumnName("dformacao");
			entity.Property((Cl e) => e.Dfront).HasColumnName("dfront");
			entity.Property((Cl e) => e.Diaspag).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("diaspag")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Did).HasColumnName("did");
			entity.Property((Cl e) => e.Distrito).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("distrito")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Dqtt).HasColumnName("dqtt");
			entity.Property((Cl e) => e.Dqttval).HasColumnType("numeric(1, 0)").HasColumnName("dqttval");
			entity.Property((Cl e) => e.Dsuporte).HasColumnName("dsuporte");
			entity.Property((Cl e) => e.Dteam).HasColumnName("dteam");
			entity.Property((Cl e) => e.Eacmfact).HasColumnType("numeric(19, 6)").HasColumnName("eacmfact");
			entity.Property((Cl e) => e.Eag).HasColumnName("eag");
			entity.Property((Cl e) => e.Eancl).HasMaxLength(35).IsUnicode(unicode: false)
				.HasColumnName("eancl")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ecoisento).HasColumnName("ecoisento");
			entity.Property((Cl e) => e.Ediexp).HasColumnName("ediexp");
			entity.Property((Cl e) => e.Eem).HasColumnName("eem");
			entity.Property((Cl e) => e.Efl).HasColumnName("efl");
			entity.Property((Cl e) => e.Eid).HasColumnName("eid");
			entity.Property((Cl e) => e.Email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("email")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Emno).HasColumnType("numeric(10, 0)").HasColumnName("emno");
			entity.Property((Cl e) => e.Encm).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("encm")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Encmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("encmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Encrpin).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("encrpin")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Eplafond).HasColumnType("numeric(19, 6)").HasColumnName("eplafond");
			entity.Property((Cl e) => e.Erentval).HasColumnType("numeric(19, 6)").HasColumnName("erentval");
			entity.Property((Cl e) => e.Esaldlet).HasColumnType("numeric(19, 6)").HasColumnName("esaldlet");
			entity.Property((Cl e) => e.Esaldo).HasColumnType("numeric(19, 6)").HasColumnName("esaldo");
			entity.Property((Cl e) => e.Excm).HasColumnType("numeric(2, 0)").HasColumnName("excm");
			entity.Property((Cl e) => e.Excmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("excmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Exporpos).HasColumnName("exporpos");
			entity.Property((Cl e) => e.Fax).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("fax")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Filtrast).HasColumnName("filtrast");
			entity.Property((Cl e) => e.Flestab).HasColumnType("numeric(3, 0)").HasColumnName("flestab");
			entity.Property((Cl e) => e.Flno).HasColumnType("numeric(10, 0)").HasColumnName("flno");
			entity.Property((Cl e) => e.Fref).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("fref")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ftdatasmr).HasColumnName("ftdatasmr");
			entity.Property((Cl e) => e.Ftdiasmr).HasColumnName("ftdiasmr");
			entity.Property((Cl e) => e.Ftidbi).HasColumnName("ftidbi");
			entity.Property((Cl e) => e.Ftidcob).HasColumnName("ftidcob");
			entity.Property((Cl e) => e.Ftidcont).HasColumnName("ftidcont");
			entity.Property((Cl e) => e.Ftidcontacto).HasColumnName("ftidcontacto");
			entity.Property((Cl e) => e.Ftidnac).HasColumnName("ftidnac");
			entity.Property((Cl e) => e.Ftidnome).HasColumnName("ftidnome");
			entity.Property((Cl e) => e.Ftidutente).HasColumnName("ftidutente");
			entity.Property((Cl e) => e.Ftmrtot).HasColumnName("ftmrtot");
			entity.Property((Cl e) => e.Ftndias).HasColumnName("ftndias");
			entity.Property((Cl e) => e.Ftnid).HasColumnName("ftnid");
			entity.Property((Cl e) => e.Ftumamr).HasColumnName("ftumamr");
			entity.Property((Cl e) => e.Fuels).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("fuels")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Gaecstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("gaecstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Cl e) => e.Gaenome).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("gaenome")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Cl e) => e.Geramb).HasColumnName("geramb");
			entity.Property((Cl e) => e.Glncl).HasMaxLength(35).IsUnicode(unicode: false)
				.HasColumnName("glncl")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Iban).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("iban")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Id).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("id")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Idno).HasColumnType("numeric(10, 0)").HasColumnName("idno");
			entity.Property((Cl e) => e.Iectisento).HasColumnName("iectisento");
			entity.Property((Cl e) => e.Imagem).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("imagem")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((Cl e) => e.Isperson).HasColumnName("isperson");
			entity.Property((Cl e) => e.Lang).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("lang")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Cl e) => e.Lmlt).HasColumnType("numeric(5, 2)").HasColumnName("lmlt");
			entity.Property((Cl e) => e.Local).HasMaxLength(43).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Localentrega).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("localentrega")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ltyp).HasColumnType("numeric(1, 0)").HasColumnName("ltyp");
			entity.Property((Cl e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Cl e) => e.Matric).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("matric")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Mesesnaopag).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("mesesnaopag")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Moeda).HasMaxLength(11).IsUnicode(unicode: false)
				.HasColumnName("moeda")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Morada).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("morada")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Motiseimp).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("motiseimp")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Naoencomenda).HasColumnName("naoencomenda");
			entity.Property((Cl e) => e.Naomail).HasColumnName("naomail");
			entity.Property((Cl e) => e.Naood).HasColumnName("naood");
			entity.Property((Cl e) => e.Nascimento).HasColumnType("datetime").HasColumnName("nascimento")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Cl e) => e.Naturalid).HasMaxLength(17).IsUnicode(unicode: false)
				.HasColumnName("naturalid")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ncont).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ncont")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ncusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ncusto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Nib).HasMaxLength(28).IsUnicode(unicode: false)
				.HasColumnName("nib")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Niec).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("niec")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Nocredit).HasColumnName("nocredit");
			entity.Property((Cl e) => e.Nome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Nome2).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome2")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ntcm).HasColumnType("numeric(2, 0)").HasColumnName("ntcm");
			entity.Property((Cl e) => e.Numautorizacaosdd).HasMaxLength(35).IsUnicode(unicode: false)
				.HasColumnName("numautorizacaosdd")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Numcontrepres).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("numcontrepres")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Numseqaut).HasColumnType("numeric(3, 0)").HasColumnName("numseqaut");
			entity.Property((Cl e) => e.Obs).HasMaxLength(240).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Obsdoc).HasColumnType("text").HasColumnName("obsdoc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Odatraso).HasColumnType("numeric(3, 0)").HasColumnName("odatraso");
			entity.Property((Cl e) => e.Odo).HasColumnName("odo");
			entity.Property((Cl e) => e.Ollocal).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("ollocal")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Operext).HasColumnName("operext");
			entity.Property((Cl e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Cl e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Pagamento).HasMaxLength(28).IsUnicode(unicode: false)
				.HasColumnName("pagamento")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Pais).HasColumnType("numeric(1, 0)").HasColumnName("pais");
			entity.Property((Cl e) => e.Paramr).HasColumnName("paramr");
			entity.Property((Cl e) => e.Particular).HasColumnName("particular");
			entity.Property((Cl e) => e.Passaporte).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("passaporte")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Pcktsyncdate).HasColumnType("datetime").HasColumnName("pcktsyncdate")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Cl e) => e.Pcktsynctime).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("pcktsynctime")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Pin).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("pin")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Plafond).HasColumnType("numeric(18, 5)").HasColumnName("plafond");
			entity.Property((Cl e) => e.Pncont).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("pncont")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Preco).HasColumnType("numeric(1, 0)").HasColumnName("preco");
			entity.Property((Cl e) => e.Pscm).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("pscm")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Pscmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("pscmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ptcm).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("ptcm")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ptcmdesc).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("ptcmdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Radicaltipoemp).HasColumnType("numeric(1, 0)").HasColumnName("radicaltipoemp");
			entity.Property((Cl e) => e.Rbal).HasColumnType("numeric(1, 0)").HasColumnName("rbal");
			entity.Property((Cl e) => e.Recdocdig).HasColumnName("recdocdig");
			entity.Property((Cl e) => e.Refcli).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("refcli")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Rentval).HasColumnType("numeric(18, 5)").HasColumnName("rentval");
			entity.Property((Cl e) => e.Repl).HasColumnName("repl");
			entity.Property((Cl e) => e.Rota).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("rota")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Saldlet).HasColumnType("numeric(18, 5)").HasColumnName("saldlet");
			entity.Property((Cl e) => e.Saldo).HasColumnType("numeric(18, 5)").HasColumnName("saldo");
			entity.Property((Cl e) => e.Saldoini).HasColumnType("numeric(16, 2)").HasColumnName("saldoini");
			entity.Property((Cl e) => e.Saldopa).HasColumnType("numeric(16, 2)").HasColumnName("saldopa");
			entity.Property((Cl e) => e.Segmento).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("segmento")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Sepacode).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("sepacode")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Shop).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("shop")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Site).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("site")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Statuspda).HasMaxLength(1).IsUnicode(unicode: false)
				.HasColumnName("statuspda")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Tabiva).HasColumnType("numeric(1, 0)").HasColumnName("tabiva");
			entity.Property((Cl e) => e.Taxairs).HasColumnType("numeric(5, 2)").HasColumnName("taxairs");
			entity.Property((Cl e) => e.Tbprcod).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tbprcod")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Telefone).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("telefone")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Temcred).HasColumnName("temcred");
			entity.Property((Cl e) => e.Temftglob).HasColumnName("temftglob");
			entity.Property((Cl e) => e.Tipo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Tipodesc).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("tipodesc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Tlmvl).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("tlmvl")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Tpdesc).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("tpdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Tpstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tpstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Cl e) => e.Track).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("track")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Tracknr).HasColumnType("numeric(1, 0)").HasColumnName("tracknr");
			entity.Property((Cl e) => e.Txftdata).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftdata")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftdias).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftdias")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidbi).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidbi")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidcob).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidcob")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidcont).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidcont")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidcontacto).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidcontacto")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidnac).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidnac")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidnome).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidnome")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftidutente).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftidutente")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftmrtot).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftmrtot")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftndias).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftndias")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txftnid).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("txftnid")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Txirspersonalizada).HasColumnName("txirspersonalizada");
			entity.Property((Cl e) => e.UAdmviz).HasColumnName("u_admviz");
			entity.Property((Cl e) => e.UAgua).HasColumnName("u_agua");
			entity.Property((Cl e) => e.UBanco).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_banco")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.UDtegar).HasColumnType("datetime").HasColumnName("u_dtegar")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Cl e) => e.UDtval).HasColumnType("datetime").HasColumnName("u_dtval")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Cl e) => e.UEntid).HasMaxLength(28).IsUnicode(unicode: false)
				.HasColumnName("u_entid")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.ULtrigger).HasColumnName("U_LTRIGGER");
			entity.Property((Cl e) => e.ULuz).HasColumnName("u_luz");
			entity.Property((Cl e) => e.UMoeda).HasMaxLength(11).IsUnicode(unicode: false)
				.HasColumnName("u_moeda")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.UNestac).HasColumnType("numeric(5, 0)").HasColumnName("u_nestac");
			entity.Property((Cl e) => e.UNomoeda).HasColumnType("numeric(1, 0)").HasColumnName("u_nomoeda");
			entity.Property((Cl e) => e.UNope).HasColumnType("numeric(8, 0)").HasColumnName("u_nope");
			entity.Property((Cl e) => e.UNrgarant).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_nrgarant")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.URef).HasMaxLength(28).IsUnicode(unicode: false)
				.HasColumnName("u_ref")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.UTerminal).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_terminal")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.UValorgar).HasColumnType("numeric(16, 5)").HasColumnName("u_valorgar");
			entity.Property((Cl e) => e.Url).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("url")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Usaintra).HasColumnName("usaintra");
			entity.Property((Cl e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Cl e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Vencimento).HasColumnType("numeric(3, 0)").HasColumnName("vencimento");
			entity.Property((Cl e) => e.Vendedor).HasColumnType("numeric(4, 0)").HasColumnName("vendedor");
			entity.Property((Cl e) => e.Vendnm).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("vendnm")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Zncm).HasColumnType("numeric(2, 0)").HasColumnName("zncm");
			entity.Property((Cl e) => e.Znregiao).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("znregiao")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Zona).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("zona")
				.HasDefaultValueSql("('')");
		});
	}
}
