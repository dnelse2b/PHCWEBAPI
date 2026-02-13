using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using SGOFWS.Domains.Models;
using SGOFWS.Extensions;

namespace SGOFWS.Persistence.Contexts;

public class SGOFCTX : DbContext
{
	public virtual DbSet<Bi> Bi { get; set; }

	public virtual DbSet<USync> USync { get; set; }

	public virtual DbSet<UTopup> UTopup { get; set; }

	public virtual DbSet<URota> URota { get; set; }

	public virtual DbSet<UEntrot> UEntrot { get; set; }

	public virtual DbSet<Bi2> Bi2 { get; set; }

	public virtual DbSet<Bo> Bo { get; set; }

	public virtual DbSet<Telemetro> Telemetro { get; set; }

	public virtual DbSet<UStqueue> UStqueue { get; set; }

	public virtual DbSet<Us> Us { get; set; }

	public virtual DbSet<Bo2> Bo2 { get; set; }

	public virtual DbSet<Cl> Cl { get; set; }

	public virtual DbSet<Bo3> Bo3 { get; set; }

	public virtual DbSet<UTrajrot> UTrajrot { get; set; }

	public virtual DbSet<UTrajcons> UTrajcons { get; set; }

	public virtual DbSet<UBocont> UBocont { get; set; }

	public virtual DbSet<Contentor> Contentor { get; set; }

	public virtual DbSet<UTopuplin> UTopuplin { get; set; }

	public virtual DbSet<LocaisFerroviarios> LocaisFerroviarios { get; set; }

	public virtual DbSet<CabecalhoHistoricoDescarregamentoVagao> CabecalhoHistoricoDescarregamentoVagao { get; set; }

	public virtual DbSet<DesengateComboio> DesengateComboio { get; set; }

	public virtual DbSet<VeiculoFerroviario> VeiculoFerroviario { get; set; }

	public virtual DbSet<CabecalhoPlanoManobra> CabecalhoPlanoManobra { get; set; }

	public virtual DbSet<LinhasPlanoManobra> LinhasPlanoManobra { get; set; }

	public virtual DbSet<UDestnot> UDestnot { get; set; }

	public virtual DbSet<EntidadeVagao> EntidadeVagao { get; set; }

	public virtual DbSet<Fluxo> Fluxo { get; set; }

	public virtual DbSet<VeiculosPorRegularizar> VeiculosPorRegularizar { get; set; }

	public virtual DbSet<Manobras> Manobras { get; set; }

	public virtual DbSet<UEstacnt> UEstacnt { get; set; }

	public virtual DbSet<Tempos> Tempos { get; set; }

	public virtual DbSet<LinhasManobra> LinhasManobra { get; set; }

	public virtual DbSet<AdmnistracaoVizinha> AdmnistracaoVizinha { get; set; }

	public virtual DbSet<AgenteTransitario> AgenteTransitario { get; set; }

	public virtual DbSet<LinhaVeiculoFerroviario> LinhaVeiculoFerroviario { get; set; }

	public virtual DbSet<HistoricoDescarregamentoVagao> HistoricoDescarregamentoVagao { get; set; }

	public virtual DbSet<Recebimento> Recebimento { get; set; }

	public virtual DbSet<CabecalhoPedidoRetirada> CabecalhoPedidoRetirada { get; set; }

	public virtual DbSet<ComboioRegisto> ComboioRegisto { get; set; }

	public virtual DbSet<Composicao> Composicao { get; set; }

	public virtual DbSet<LinhaComposicao> LinhaComposicao { get; set; }

	public virtual DbSet<PrevisaoMensalCarga> PrevisaoMensalCarga { get; set; }

	public virtual DbSet<UNotascomp> UNotascomp { get; set; }

	public virtual DbSet<ULogs> ULogs { get; set; }

	public virtual DbSet<UNotific> UNotific { get; set; }

	public virtual DbSet<UOridest> UOridest { get; set; }

	public virtual DbSet<UHistcons> UHistcons { get; set; }

	public virtual DbSet<PatiosManobra> PatiosManobra { get; set; }

	public virtual DbSet<RevisaoMaterial> RevisaoMaterial { get; set; }

	public virtual DbSet<LinhaRevisao> LinhaRevisao { get; set; }

	public virtual DbSet<UNotasrev> UNotasrev { get; set; }

	public SGOFCTX()
	{
	}

	public SGOFCTX(DbContextOptions<SGOFCTX> options)
		: base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBconnect"));
	}

	public string GetModelNameForTable(string tableName)
	{
		return Model.GetEntityTypes().FirstOrDefault((IEntityType et) => string.Equals(et.GetTableName(), tableName, StringComparison.OrdinalIgnoreCase))?.ClrType?.Name ?? "Modelo não encontrado";
	}

	public string GetPropertyNameForColumn(string tableName, string columnName)
	{
		IEntityType entityType = Model.GetEntityTypes().FirstOrDefault((IEntityType et) => string.Equals(et.GetTableName(), tableName, StringComparison.OrdinalIgnoreCase));
		if (entityType == null)
		{
			return "Tabela não encontrada";
		}
		foreach (IProperty property in entityType.GetProperties())
		{
			if (string.Equals(property.GetColumnName(), columnName, StringComparison.OrdinalIgnoreCase))
			{
				return property.Name;
			}
		}
		return "Coluna não encontrada";
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		if (Database.IsSqlServer())
		{
			modelBuilder.AddSqlConvertFunction();
		}
		modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
		modelBuilder.Entity(delegate(EntityTypeBuilder<Us> entity)
		{
			entity.HasKey((Us e) => e.Userno).HasName("pk_us").IsClustered(clustered: false);
			entity.ToTable("us");
			entity.HasIndex((Us e) => e.Usstamp, "in_us_stamp").IsUnique().HasFillFactor(80);
			entity.HasIndex((Us e) => e.Usercode, "in_us_usercode").IsUnique().HasFillFactor(80);
			entity.HasIndex((Us e) => e.Username, "in_us_username").HasFillFactor(80);
			entity.HasIndex((Us e) => e.Userno, "in_us_userno").HasFillFactor(80);
			entity.Property((Us e) => e.Userno).HasColumnType("numeric(6, 0)").HasColumnName("userno");
			entity.Property((Us e) => e.Abrecalfis).HasColumnName("abrecalfis");
			entity.Property((Us e) => e.Abremonrelcred).HasColumnName("abremonrelcred");
			entity.Property((Us e) => e.Actk2fa).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("actk2fa")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Admdcont).HasColumnName("admdcont");
			entity.Property((Us e) => e.Admdpess).HasColumnName("admdpess");
			entity.Property((Us e) => e.Aextpw).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("aextpw")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Aextra).HasColumnName("aextra");
			entity.Property((Us e) => e.Agencalfis).HasColumnName("agencalfis");
			entity.Property((Us e) => e.Alertsweb).HasColumnName("alertsweb");
			entity.Property((Us e) => e.Antf).HasColumnName("antf");
			entity.Property((Us e) => e.Area).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("area")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Us e) => e.Asusername).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("asusername")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Asuserno).HasColumnType("numeric(6, 0)").HasColumnName("asuserno");
			entity.Property((Us e) => e.Autposmv).HasColumnName("autposmv");
			entity.Property((Us e) => e.Avstamp).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("avstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Avstimer).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("avstimer")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Us e) => e.Bwscodigo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("bwscodigo")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Bwspw).HasMaxLength(32).IsUnicode(unicode: false)
				.HasColumnName("bwspw")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Centrolog).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("centrolog")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Checkhelpt).HasColumnName("checkhelpt");
			entity.Property((Us e) => e.Clbadm).HasColumnName("clbadm");
			entity.Property((Us e) => e.Clbno).HasColumnType("numeric(10, 0)").HasColumnName("clbno");
			entity.Property((Us e) => e.Clbnome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("clbnome")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Us e) => e.Clbstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("clbstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Cliadm).HasColumnName("cliadm");
			entity.Property((Us e) => e.Cvenome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("cvenome")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Us e) => e.Cvestamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("cvestamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Dataultpass).HasColumnType("datetime").HasColumnName("dataultpass")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Dexp2fa).HasColumnType("datetime").HasColumnName("dexp2fa")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Dexptk2fa).HasColumnType("datetime").HasColumnName("dexptk2fa")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Diasani).HasColumnName("diasani");
			entity.Property((Us e) => e.Diascalfis).HasColumnType("numeric(3, 0)").HasColumnName("diascalfis");
			entity.Property((Us e) => e.Diascon).HasColumnName("diascon");
			entity.Property((Us e) => e.Dntf).HasColumnType("datetime").HasColumnName("dntf")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Dpt).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("dpt")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Drno).HasColumnType("numeric(10, 0)").HasColumnName("drno");
			entity.Property((Us e) => e.Drnome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("drnome")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Drstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("drstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Dultrs).HasColumnType("datetime").HasColumnName("dultrs")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Email).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("email")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Emaxposmv).HasColumnType("numeric(19, 6)").HasColumnName("emaxposmv");
			entity.Property((Us e) => e.Empregado).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("empregado")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Esa).HasColumnName("esa");
			entity.Property((Us e) => e.Estatuto).HasColumnType("text").HasColumnName("estatuto")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Etiquetascodigo).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("etiquetascodigo")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Etiquetasdescricao).HasColumnType("text").HasColumnName("etiquetasdescricao")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Exchangepass).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("exchangepass")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Fazfxmanutencao).HasColumnName("fazfxmanutencao");
			entity.Property((Us e) => e.Filtrocts).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("filtrocts")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtroctsstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("filtroctsstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtroem).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("filtroem")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtroemstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("filtroemstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtromx).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("filtromx")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtromxstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("filtromxstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtrotta).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("filtrotta")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtrottastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("filtrottastamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtrovi).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("filtrovi")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Filtrovistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("filtrovistamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Fntf).HasColumnType("numeric(2, 0)").HasColumnName("fntf");
			entity.Property((Us e) => e.Forgotdate).HasColumnType("datetime").HasColumnName("forgotdate")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Forgotid).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("forgotid")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Gestordenuncias).HasColumnName("gestordenuncias");
			entity.Property((Us e) => e.Grupo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("grupo")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Hntf).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("hntf")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Homeus).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("homeus")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Hultrs).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("hultrs")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Idioma).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("idioma")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Idiomakey).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("idiomakey")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Imagem).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("imagem")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((Us e) => e.Iniciais).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("iniciais")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Inirs).HasColumnName("inirs");
			entity.Property((Us e) => e.Jaidirecto).HasColumnName("jaidirecto");
			entity.Property((Us e) => e.Jaini).HasColumnName("jaini");
			entity.Property((Us e) => e.Loginerrado).HasColumnType("numeric(6, 0)").HasColumnName("loginerrado");
			entity.Property((Us e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Us e) => e.Maxposmv).HasColumnType("numeric(18, 5)").HasColumnName("maxposmv");
			entity.Property((Us e) => e.Mcdata).HasColumnType("datetime").HasColumnName("mcdata")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.Mcmes).HasColumnType("numeric(2, 0)").HasColumnName("mcmes");
			entity.Property((Us e) => e.Menuesquerda).HasColumnName("menuesquerda");
			entity.Property((Us e) => e.Nivelaprovacao).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("nivelaprovacao")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Notifypw).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("notifypw")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Notifytk).HasMaxLength(254).IsUnicode(unicode: false)
				.HasColumnName("notifytk")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Notifyus).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("notifyus")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Ntfma).HasColumnName("ntfma");
			entity.Property((Us e) => e.Nusamntlb).HasColumnName("nusamntlb");
			entity.Property((Us e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Us e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Pederelcred).HasColumnName("pederelcred");
			entity.Property((Us e) => e.Peno).HasColumnType("numeric(6, 0)").HasColumnName("peno");
			entity.Property((Us e) => e.Pestamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("pestamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Us e) => e.Pntf).HasColumnName("pntf");
			entity.Property((Us e) => e.Profission).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("profission")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Pwautent).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("pwautent")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Pwpos).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("pwpos")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Rgpdadm).HasColumnName("rgpdadm");
			entity.Property((Us e) => e.Setpasswd).HasColumnName("setpasswd");
			entity.Property((Us e) => e.Setpasswdintra).HasColumnName("setpasswdintra");
			entity.Property((Us e) => e.Sgqadm).HasColumnName("sgqadm");
			entity.Property((Us e) => e.Skypeid).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("skypeid")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Smsemail).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("smsemail")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Susername).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("susername")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Suserno).HasColumnType("numeric(6, 0)").HasColumnName("suserno");
			entity.Property((Us e) => e.Synactcem).HasColumnName("synactcem");
			entity.Property((Us e) => e.Synactcvi).HasColumnName("synactcvi");
			entity.Property((Us e) => e.Syncactcts).HasColumnName("syncactcts");
			entity.Property((Us e) => e.Syncactmx).HasColumnName("syncactmx");
			entity.Property((Us e) => e.Syncacttda).HasColumnName("syncacttda");
			entity.Property((Us e) => e.Synccts).HasColumnName("synccts");
			entity.Property((Us e) => e.Syncem).HasColumnName("syncem");
			entity.Property((Us e) => e.Syncimpnovatda).HasColumnName("syncimpnovatda");
			entity.Property((Us e) => e.Syncimpnovatta).HasColumnName("syncimpnovatta");
			entity.Property((Us e) => e.Syncmx).HasColumnName("syncmx");
			entity.Property((Us e) => e.Synctda).HasColumnName("synctda");
			entity.Property((Us e) => e.Synctta).HasColumnName("synctta");
			entity.Property((Us e) => e.Syncvi).HasColumnName("syncvi");
			entity.Property((Us e) => e.Tecnico).HasColumnType("numeric(6, 0)").HasColumnName("tecnico");
			entity.Property((Us e) => e.Tecnnm).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tecnnm")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Tema).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tema")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Tembws).HasColumnName("tembws");
			entity.Property((Us e) => e.Temveriprstock).HasColumnName("temveriprstock");
			entity.Property((Us e) => e.Tipoacd).HasColumnType("numeric(1, 0)").HasColumnName("tipoacd");
			entity.Property((Us e) => e.Tipoacdvs).HasColumnType("numeric(1, 0)").HasColumnName("tipoacdvs");
			entity.Property((Us e) => e.Tlmvl).HasMaxLength(45).IsUnicode(unicode: false)
				.HasColumnName("tlmvl")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Tntf).HasColumnType("numeric(2, 0)").HasColumnName("tntf");
			entity.Property((Us e) => e.UBase).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_base")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UBasedesn).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_basedesn")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UBztpass).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("u_bztpass")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UCctst).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("u_cctst")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UCestacao).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("u_cestacao")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UCturnno).HasColumnType("numeric(16, 0)").HasColumnName("u_cturnno")
				.HasDefaultValueSql("((0))");
			entity.Property((Us e) => e.UDstacao).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("u_dstacao")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UDtfi).HasColumnType("datetime").HasColumnName("u_dtfi")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.UDtfic).HasColumnType("datetime").HasColumnName("u_dtfic")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.UDtin).HasColumnType("datetime").HasColumnName("u_dtin")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.UDtinc).HasColumnType("datetime").HasColumnName("u_dtinc")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Us e) => e.UEbr).HasColumnName("u_ebr");
			entity.Property((Us e) => e.UEscala).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("u_escala")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UEscalabr).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("u_escalabr")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UEsccatg).HasMaxLength(253).IsUnicode(unicode: false)
				.HasColumnName("u_esccatg")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UFunc).HasColumnName("u_func");
			entity.Property((Us e) => e.UMotivo).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_motivo")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UNaprov).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("u_naprov")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.UNestacao).HasColumnType("numeric(6, 0)").HasColumnName("u_nestacao");
			entity.Property((Us e) => e.UNo).HasColumnType("numeric(6, 0)").HasColumnName("u_no");
			entity.Property((Us e) => e.URenovar).HasColumnName("u_renovar");
			entity.Property((Us e) => e.USuperus).HasColumnName("u_superus");
			entity.Property((Us e) => e.UUtente).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_utente")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Ugstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ugstamp")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Usaarea).HasColumnName("usaarea");
			entity.Property((Us e) => e.Usatimezone).HasColumnName("usatimezone");
			entity.Property((Us e) => e.Usavanc).HasColumnName("usavanc");
			entity.Property((Us e) => e.Use2fa).HasColumnName("use2fa");
			entity.Property((Us e) => e.Usercode).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("usercode")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Username).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("username")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Us e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Usstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("usstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Us e) => e.Utcbrowser).HasColumnType("numeric(1, 0)").HasColumnName("utcbrowser");
			entity.Property((Us e) => e.Utcdisplayname).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("utcdisplayname")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Utcuserid).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("utcuserid")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Vendedor).HasColumnType("numeric(6, 0)").HasColumnName("vendedor");
			entity.Property((Us e) => e.Vendnm).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("vendnm")
				.HasDefaultValueSql("('')");
			entity.Property((Us e) => e.Verificachamadas).HasColumnName("verificachamadas");
			entity.Property((Us e) => e.Vsstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("vsstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
		});
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
			entity.Property((VeiculosPorRegularizar e) => e.Descerr).HasMaxLength(250).IsUnicode(unicode: false)
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
		modelBuilder.Entity(delegate(EntityTypeBuilder<Telemetro> entity)
		{
			entity.HasKey((Telemetro e) => e.Telemetrostamp).HasName("pk_u_fer10077").IsClustered(clustered: false);
			entity.ToTable("u_fer10077");
			entity.Property((Telemetro e) => e.Telemetrostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer10077stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Telemetro e) => e.Antest).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("antest")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Coantest).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("coantest")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Coultest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("coultest")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Telemetro e) => e.No).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Telemetro e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Ultest).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("ultest")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Telemetro e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Telemetro e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UTopuplin> entity)
		{
			entity.HasKey((UTopuplin e) => e.UTopuplinstamp).HasName("pk_u_topuplin").IsClustered(clustered: false);
			entity.ToTable("u_topuplin");
			entity.Property((UTopuplin e) => e.UTopuplinstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_topuplinstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UTopuplin e) => e.Carga).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Stampvag).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampvag")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Catcarga).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("catcarga")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Desiglocal).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("desiglocal")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Estado).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estado")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Local).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UTopuplin e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTopuplin e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Peso).HasColumnType("numeric(16, 2)").HasColumnName("peso");
			entity.Property((UTopuplin e) => e.UTopupstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_topupstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UTopuplin e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTopuplin e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UTopuplin e) => e.Vagno).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("vagno")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UNotascomp> entity)
		{
			entity.HasKey((UNotascomp e) => e.UNotascompstamp).HasName("pk_u_notascomp").IsClustered(clustered: false);
			entity.ToTable("u_notascomp");
			entity.Property((UNotascomp e) => e.UNotascompstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_notascompstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UNotascomp e) => e.Codigo).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.Descricao).HasColumnType("text").HasColumnName("descricao")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UNotascomp e) => e.Oristamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oristamp")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UNotascomp e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.Outro).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("outro")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.LinhaComposicaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer1006stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UNotascomp e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UNotascomp e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UNotascomp e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Composicao> entity)
		{
			entity.HasKey((Composicao e) => e.Composicaostamp).HasName("pk_u_fer10055").IsClustered(clustered: false);
			entity.ToTable("u_fer10055");
			entity.Property((Composicao e) => e.Composicaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer10055stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Composicao e) => e.Codest).HasMaxLength(12).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Sequencia).HasColumnName("sequencia");
			entity.Property((Composicao e) => e.Comboio).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("comboio")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Datafim).HasColumnType("datetime").HasColumnName("datafim")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Composicao e) => e.Dataini).HasColumnType("datetime").HasColumnName("dataini")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Composicao e) => e.Docori).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("docori")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.No).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Horaini).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("horaini")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Horafim).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("horafim")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Estacao).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Composicao e) => e.Sync).HasColumnName("sync");
			entity.Property((Composicao e) => e.Nocomp).HasColumnType("numeric(16, 0)").HasColumnName("nocomp");
			entity.Property((Composicao e) => e.Nomecomp).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("nomecomp")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Obs).HasColumnType("text").HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Composicao e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Ref).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Sentido).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("sentido")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Composicao e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Composicao e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<LinhaComposicao> entity)
		{
			entity.HasKey((LinhaComposicao e) => e.LinhaComposicaostamp).HasName("pk_u_fer1006").IsClustered(clustered: false);
			entity.ToTable("u_fer1006");
			entity.Property((LinhaComposicao e) => e.LinhaComposicaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer1006stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhaComposicao e) => e.Adicionado).HasColumnName("adicionado");
			entity.Property((LinhaComposicao e) => e.Admin).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Carga).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Docno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("docno")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Docori).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("docori")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Estadocomp).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("estadocomp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Fullempty).HasColumnName("fullempty");
			entity.Property((LinhaComposicao e) => e.Marcada).HasColumnName("marcada");
			entity.Property((LinhaComposicao e) => e.Novg).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("novg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Oristamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oristamp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhaComposicao e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Peso).HasColumnType("numeric(16, 3)").HasColumnName("peso");
			entity.Property((LinhaComposicao e) => e.Composicaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer10055stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhaComposicao e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhaComposicao e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Vagtip).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("vagtip")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaComposicao e) => e.Sequencia).HasColumnName("sequencia");
			entity.Property((LinhaComposicao e) => e.Volume).HasColumnType("numeric(16, 3)").HasColumnName("volume");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UTopup> entity)
		{
			entity.HasKey((UTopup e) => e.UTopupstamp).HasName("pk_u_topup").IsClustered(clustered: false);
			entity.ToTable("u_topup");
			entity.Property((UTopup e) => e.UTopupstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_topupstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UTopup e) => e.Data).HasColumnType("datetime").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UTopup e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UTopup e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTopup e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTopup e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UTopup e) => e.Pedidoid).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("pedidoid")
				.HasDefaultValueSql("('')");
			entity.Property((UTopup e) => e.Aprovado).HasColumnName("aprovado");
			entity.Property((UTopup e) => e.Tipo).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((UTopup e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTopup e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTopup e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UTrajrot> entity)
		{
			entity.HasKey((UTrajrot e) => e.UTrajrotstamp).HasName("pk_u_trajrot").IsClustered(clustered: false);
			entity.ToTable("u_trajrot");
			entity.Property((UTrajrot e) => e.UTrajrotstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_trajrotstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UTrajrot e) => e.Admin).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Bossa).HasColumnName("bossa");
			entity.Property((UTrajrot e) => e.Codigo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Codprxst).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codprxst")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Descesta).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("descesta")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Destaque).HasColumnName("destaque");
			entity.Property((UTrajrot e) => e.Destino).HasColumnName("destino");
			entity.Property((UTrajrot e) => e.Estacao).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Fronteira).HasColumnName("fronteira");
			entity.Property((UTrajrot e) => e.Hora).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("hora")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UTrajrot e) => e.Ordem).HasColumnType("numeric(16, 0)").HasColumnName("ordem");
			entity.Property((UTrajrot e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTrajrot e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Proximaestacao).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("proximaestacao")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.URotastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_rotastamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UTrajrot e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTrajrot e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajrot e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UEntrot> entity)
		{
			entity.HasKey((UEntrot e) => e.UEntrotstamp).HasName("pk_u_entrot").IsClustered(clustered: false);
			entity.ToTable("u_entrot");
			entity.Property((UEntrot e) => e.UEntrotstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_entrotstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UEntrot e) => e.Entidade).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("entidade")
				.HasDefaultValueSql("('')");
			entity.Property((UEntrot e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UEntrot e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UEntrot e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UEntrot e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UEntrot e) => e.URotastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_rotastamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UEntrot e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UEntrot e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UEntrot e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UDestnot> entity)
		{
			entity.HasKey((UDestnot e) => e.UDestnotstamp).HasName("pk_u_destnot").IsClustered(clustered: false);
			entity.ToTable("u_destnot");
			entity.Property((UDestnot e) => e.UDestnotstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_destnotstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UDestnot e) => e.Codigo).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((UDestnot e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UDestnot e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UDestnot e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UDestnot e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UDestnot e) => e.UEstacntstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_estacntstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UDestnot e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UDestnot e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UDestnot e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<LinhaRevisao> entity)
		{
			entity.HasKey((LinhaRevisao e) => e.LinhaRevisaostamp).HasName("pk_u_fer085").IsClustered(clustered: false);
			entity.ToTable("u_fer085");
			entity.Property((LinhaRevisao e) => e.LinhaRevisaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer085stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhaRevisao e) => e.Abaeng).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("abaeng")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Admin).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Almofapoio).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("almofapoio")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Bi2stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bi2stamp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Bocim).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("bocim")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Cabecote).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("cabecote")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Cabopuxa).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("cabopuxa")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Carga).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Cavdentro).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("cavdentro")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Cavpalmat).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("cavpalmat")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Corremao).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("corremao")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Cunhal).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("cunhal")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Elevmart).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("elevmart")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Estadorv).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estadorv")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Estribo).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("estribo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Faltaparaf).HasColumnName("faltaparaf");
			entity.Property((LinhaRevisao e) => e.Fe014stamp).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("fe014stamp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Fer014stamp).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("fer014stamp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Fullempty).HasColumnName("fullempty");
			entity.Property((LinhaRevisao e) => e.Fuso).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("fuso")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Hastecil).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("hastecil")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Mangueira).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("mangueira")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Marcada).HasColumnName("marcada");
			entity.Property((LinhaRevisao e) => e.Molabate).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("molabate")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Molasusp).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("molasusp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Munhcil).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("munhcil")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Novg).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("novg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Obs).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Oristamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oristamp")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhaRevisao e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Outr).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("outr")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Paraftracc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("paraftracc")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Peso).HasColumnType("numeric(16, 3)").HasColumnName("peso");
			entity.Property((LinhaRevisao e) => e.Prumo).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("prumo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Rebpivot).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("rebpivot")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Regulador).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("regulador")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Semdentes).HasColumnName("semdentes");
			entity.Property((LinhaRevisao e) => e.Semmanip).HasColumnName("semmanip");
			entity.Property((LinhaRevisao e) => e.Semtroco).HasColumnName("semtroco");
			entity.Property((LinhaRevisao e) => e.Testeira).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("testeira")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Tirelevm).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tirelevm")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Travdecant).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("travdecant")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.RevisaoMaterialstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer084stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhaRevisao e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhaRevisao e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Vagtip).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("vagtip")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Valvapert).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("valvapert")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Volman).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("volman")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaRevisao e) => e.Volume).HasColumnType("numeric(16, 3)").HasColumnName("volume");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<RevisaoMaterial> entity)
		{
			entity.HasKey((RevisaoMaterial e) => e.RevisaoMaterialstamp).HasName("pk_u_fer084").IsClustered(clustered: false);
			entity.ToTable("u_fer084");
			entity.Property((RevisaoMaterial e) => e.RevisaoMaterialstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer084stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((RevisaoMaterial e) => e.Bistamp).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("bistamp")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Codrev).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("codrev")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Comboio).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("comboio")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Dtcomb).HasColumnType("datetime").HasColumnName("dtcomb")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((RevisaoMaterial e) => e.Dtfrev).HasColumnType("datetime").HasColumnName("dtfrev")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((RevisaoMaterial e) => e.Dtirev).HasColumnType("datetime").HasColumnName("dtirev")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((RevisaoMaterial e) => e.Estacao).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Hrcomb).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("hrcomb")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Hrfrev).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("hrfrev")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Hrirev).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("hrirev")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Marcada).HasColumnName("marcada");
			entity.Property((RevisaoMaterial e) => e.Sync).HasColumnName("sync");
			entity.Property((RevisaoMaterial e) => e.No).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((RevisaoMaterial e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Ref).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Revcfm).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("revcfm")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Norev).HasColumnType("numeric(16, 0)").HasColumnName("norev");
			entity.Property((RevisaoMaterial e) => e.Revoutr).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("revoutr")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Statusrev).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("statusrev")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((RevisaoMaterial e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((RevisaoMaterial e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<USync> entity)
		{
			entity.HasKey((USync e) => e.USyncstamp).HasName("pk_u_sync").IsClustered(clustered: false);
			entity.ToTable("u_sync");
			entity.Property((USync e) => e.Data).HasColumnType("text").HasColumnName("data")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Date).HasColumnType("datetime").HasColumnName("date")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((USync e) => e.Marcada).HasColumnName("marcada");
			entity.Property((USync e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((USync e) => e.Dataproc).HasColumnType("datetime").HasColumnName("dataproc")
				.HasDefaultValueSql("(getdate())");
			entity.Property((USync e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Processcode).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("processcode")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Processid).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("processid")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Responseproc).HasColumnType("text").HasColumnName("responseproc")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Status).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("status")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.USyncstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_syncstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((USync e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((USync e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((USync e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UNotasrev> entity)
		{
			entity.HasKey((UNotasrev e) => e.UNotasrevstamp).HasName("pk_u_notasrev").IsClustered(clustered: false);
			entity.ToTable("u_notasrev");
			entity.Property((UNotasrev e) => e.UNotasrevstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_notasrevstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UNotasrev e) => e.Codigo).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.Descricao).HasColumnType("text").HasColumnName("descricao")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UNotasrev e) => e.Oristamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("oristamp")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UNotasrev e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.Outro).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("outro")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.LinhaRevisaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer085stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UNotasrev e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UNotasrev e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UNotasrev e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<CabecalhoHistoricoDescarregamentoVagao> entity)
		{
			entity.HasKey((CabecalhoHistoricoDescarregamentoVagao e) => e.CabecalhoHistoricoDescarregamentoVagaostamp).HasName("pk_u_fer01001").IsClustered(clustered: false);
			entity.ToTable("u_fer01001");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.CabecalhoHistoricoDescarregamentoVagaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer01001stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Manobrastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("manobrastamp")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Marcada).HasColumnName("marcada");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Meddesccargamin).HasColumnType("numeric(16, 2)").HasColumnName("meddesccargamin");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Cargapordesc).HasColumnType("numeric(16, 2)").HasColumnName("cargapordesc");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Meddescmin).HasColumnType("numeric(16, 2)").HasColumnName("meddescmin");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Pedidoid).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("pedidoid")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Percentagemdesc).HasColumnType("numeric(16, 2)").HasColumnName("percentagemdesc");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Planomanobrastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("planomanobrastamp")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Totaldesccarga).HasColumnType("numeric(16, 2)").HasColumnName("totaldesccarga");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Totalforn).HasColumnType("numeric(16, 2)").HasColumnName("totalforn");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Totalforncarga).HasColumnType("numeric(16, 2)").HasColumnName("totalforncarga");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Totdescarrngd).HasColumnType("numeric(16, 2)").HasColumnName("totdescarrngd");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Totpordesc).HasColumnType("numeric(16, 0)").HasColumnName("totpordesc");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Totvagdesc).HasColumnType("numeric(16, 0)").HasColumnName("totvagdesc");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoHistoricoDescarregamentoVagao e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<AdmnistracaoVizinha> entity)
		{
			entity.HasKey((AdmnistracaoVizinha e) => e.AdmnistracaoVizinhastamp).HasName("pk_u_fer050").IsClustered(clustered: false);
			entity.ToTable("u_fer050");
			entity.Property((AdmnistracaoVizinha e) => e.AdmnistracaoVizinhastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer050stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((AdmnistracaoVizinha e) => e.Codigo).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Design).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("design")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Fluxodesign).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("fluxodesign")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Fluxov).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("fluxov")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Locos).HasColumnName("locos");
			entity.Property((AdmnistracaoVizinha e) => e.Marcada).HasColumnName("marcada");
			entity.Property((AdmnistracaoVizinha e) => e.Operador).HasColumnName("operador");
			entity.Property((AdmnistracaoVizinha e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((AdmnistracaoVizinha e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((AdmnistracaoVizinha e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((AdmnistracaoVizinha e) => e.Vizinh).HasColumnName("vizinh");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<LocaisFerroviarios> entity)
		{
			entity.HasKey((LocaisFerroviarios e) => e.LocaisFerroviariosstamp).HasName("pk_u_fer000").IsClustered(clustered: false);
			entity.ToTable("u_fer000");
			entity.HasIndex((LocaisFerroviarios e) => new { e.Codigo, e.Estacao }, "in_U_FER000_User_33").IsUnique().HasFillFactor(80);
			entity.Property((LocaisFerroviarios e) => e.LocaisFerroviariosstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer000stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LocaisFerroviarios e) => e.Cdesvio).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("cdesvio")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Circulacao).HasColumnName("circulacao");
			entity.Property((LocaisFerroviarios e) => e.Codigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Design).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("design")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Designestac).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("designestac")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Desvio).HasColumnName("desvio");
			entity.Property((LocaisFerroviarios e) => e.Estacao).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Fluxos).HasColumnName("fluxos");
			entity.Property((LocaisFerroviarios e) => e.Fornec).HasColumnName("fornec");
			entity.Property((LocaisFerroviarios e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((LocaisFerroviarios e) => e.Isento).HasColumnName("isento");
			entity.Property((LocaisFerroviarios e) => e.Kpi).HasColumnName("kpi");
			entity.Property((LocaisFerroviarios e) => e.Linhafer).HasMaxLength(3).IsUnicode(unicode: false)
				.HasColumnName("linhafer")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Local).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Marcada).HasColumnName("marcada");
			entity.Property((LocaisFerroviarios e) => e.Mzn).HasColumnType("numeric(12, 0)").HasColumnName("mzn");
			entity.Property((LocaisFerroviarios e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LocaisFerroviarios e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Resguardo).HasColumnName("resguardo");
			entity.Property((LocaisFerroviarios e) => e.Retirad).HasColumnName("retirad");
			entity.Property((LocaisFerroviarios e) => e.Revisao).HasColumnName("revisao");
			entity.Property((LocaisFerroviarios e) => e.Servico).HasColumnName("servico");
			entity.Property((LocaisFerroviarios e) => e.Usd).HasColumnType("numeric(8, 0)").HasColumnName("usd");
			entity.Property((LocaisFerroviarios e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LocaisFerroviarios e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LocaisFerroviarios e) => e.Zar).HasColumnType("numeric(8, 0)").HasColumnName("zar");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<PatiosManobra> entity)
		{
			entity.HasKey((PatiosManobra e) => e.PatiosManobrastamp).HasName("pk_u_fer016").IsClustered(clustered: false);
			entity.ToTable("u_fer016");
			entity.Property((PatiosManobra e) => e.PatiosManobrastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer016stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((PatiosManobra e) => e.Codigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((PatiosManobra e) => e.Design).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("design")
				.HasDefaultValueSql("('')");
			entity.Property((PatiosManobra e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((PatiosManobra e) => e.Marcada).HasColumnName("marcada");
			entity.Property((PatiosManobra e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((PatiosManobra e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((PatiosManobra e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((PatiosManobra e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((PatiosManobra e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((PatiosManobra e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<DesengateComboio> entity)
		{
			entity.HasKey((DesengateComboio e) => e.DesengateComboiostamp).HasName("pk_u_fer013").IsClustered(clustered: false);
			entity.ToTable("u_fer013");
			entity.HasIndex((DesengateComboio e) => e.Ref, "in_U_FER013_User_21").IsUnique().HasFillFactor(80);
			entity.Property((DesengateComboio e) => e.DesengateComboiostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer013stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((DesengateComboio e) => e.Data).HasColumnType("datetime").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((DesengateComboio e) => e.Desiglocal).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("desiglocal")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Estacao).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Hora).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hora")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Local).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Marcada).HasColumnName("marcada");
			entity.Property((DesengateComboio e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((DesengateComboio e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Ref).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((DesengateComboio e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((DesengateComboio e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<URota> entity)
		{
			entity.HasKey((URota e) => e.URotastamp).HasName("pk_u_rota").IsClustered(clustered: false);
			entity.ToTable("u_rota");
			entity.Property((URota e) => e.URotastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_rotastamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((URota e) => e.Coddest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("coddest")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Codest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Destino).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("destino")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Estacao).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Linha).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("linha")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Marcada).HasColumnName("marcada");
			entity.Property((URota e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((URota e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((URota e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((URota e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UEstacnt> entity)
		{
			entity.HasKey((UEstacnt e) => e.UEstacntstamp).HasName("pk_u_estacnt").IsClustered(clustered: false);
			entity.ToTable("u_estacnt");
			entity.Property((UEstacnt e) => e.UEstacntstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_estacntstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UEstacnt e) => e.Bossa).HasColumnName("bossa");
			entity.Property((UEstacnt e) => e.Codigo).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((UEstacnt e) => e.Estacao).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((UEstacnt e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UEstacnt e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UEstacnt e) => e.Ordem).HasColumnType("numeric(16, 0)").HasColumnName("ordem");
			entity.Property((UEstacnt e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UEstacnt e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UEstacnt e) => e.Proximaestacao).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("proximaestacao")
				.HasDefaultValueSql("('')");
			entity.Property((UEstacnt e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UEstacnt e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UEstacnt e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UHistcons> entity)
		{
			entity.HasKey((UHistcons e) => e.UHistconsstamp).HasName("pk_u_histcons").IsClustered(clustered: false);
			entity.ToTable("u_histcons");
			entity.Property((UHistcons e) => e.UHistconsstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_histconsstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UHistcons e) => e.Admin).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Content).HasColumnType("text").HasColumnName("content")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Data).HasColumnType("DateTime?").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((UHistcons e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UHistcons e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UHistcons e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Provider).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("provider")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Response).HasColumnType("text").HasColumnName("response")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UHistcons e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UHistcons e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UNotific> entity)
		{
			entity.HasKey((UNotific e) => e.UNotificstamp).HasName("pk_u_notific").IsClustered(clustered: false);
			entity.ToTable("u_notific");
			entity.Property((UNotific e) => e.UNotificstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_notificstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UNotific e) => e.Entidade).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("entidade")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UNotific e) => e.Obs).HasColumnType("text").HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Operacao).HasMaxLength(4000).IsUnicode(unicode: false)
				.HasColumnName("operacao")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UNotific e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Sentido).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("sentido")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Status).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("status")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Tabela).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("tabela")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Tabstamp).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tabstamp")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UNotific e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UNotific e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<LinhasPlanoManobra> entity)
		{
			entity.HasKey((LinhasPlanoManobra e) => e.LinhasPlanoManobraStamp).HasName("pk_u_fer022").IsClustered(clustered: false);
			entity.ToTable("u_fer022");
			entity.HasIndex((LinhasPlanoManobra e) => new { e.CabecalhoPlanoManobraStamp, e.Novg }, "in_U_FER022_User_32").IsUnique().HasFillFactor(80);
			entity.Property((LinhasPlanoManobra e) => e.LinhasPlanoManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer022stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhasPlanoManobra e) => e.Carga).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Codcarga).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codcarga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Coddest).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("coddest")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Codorig).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codorig")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Destino).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("destino")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Loco).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("loco")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Marcada).HasColumnName("marcada");
			entity.Property((LinhasPlanoManobra e) => e.Novg).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("novg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Origem).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("origem")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhasPlanoManobra e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Stampevg).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampevg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Tipovg).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("tipovg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.CabecalhoPlanoManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer021stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhasPlanoManobra e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhasPlanoManobra e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasPlanoManobra e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<CabecalhoPedidoRetirada> entity)
		{
			entity.HasKey((CabecalhoPedidoRetirada e) => e.CabecalhoPedidoRetiradaStamp).HasName("pk_u_fer012").IsClustered(clustered: false);
			entity.ToTable("u_fer012");
			entity.HasIndex((CabecalhoPedidoRetirada e) => e.Nrped, "in_U_FER012_User_108").IsUnique().HasFillFactor(80);
			entity.HasIndex((CabecalhoPedidoRetirada e) => new { e.Data, e.Hora, e.Desvio }, "in_U_FER012_User_20").IsUnique().HasFillFactor(80);
			entity.Property((CabecalhoPedidoRetirada e) => e.CabecalhoPedidoRetiradaStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer012stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((CabecalhoPedidoRetirada e) => e.Datasub).HasColumnType("datetime").HasColumnName("datasub")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((CabecalhoPedidoRetirada e) => e.Horasub).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("horasub")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Aprovado).HasColumnName("aprovado");
			entity.Property((CabecalhoPedidoRetirada e) => e.Verificar).HasColumnName("verificar");
			entity.Property((CabecalhoPedidoRetirada e) => e.Automatica).HasColumnName("automatica");
			entity.Property((CabecalhoPedidoRetirada e) => e.Codest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Pedidoforn).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("pedidoforn")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Data).HasColumnType("datetime").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((CabecalhoPedidoRetirada e) => e.Dataaprovacao).HasColumnType("datetime").HasColumnName("dataaprovacao")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((CabecalhoPedidoRetirada e) => e.Desvio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("desvio")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Estacao).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Fluxo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("fluxo")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Hora).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Horaaprovacao).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("horaaprovacao")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Loco).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("loco")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Marcada).HasColumnName("marcada");
			entity.Property((CabecalhoPedidoRetirada e) => e.Nomeaprovacao).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("nomeaprovacao")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Nrped).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nrped")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Obs).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((CabecalhoPedidoRetirada e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Patio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("patio")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Pedidoid).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("pedidoid")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Seqporterm).HasColumnType("numeric(5, 0)").HasColumnName("seqporterm");
			entity.Property((CabecalhoPedidoRetirada e) => e.Tpforn).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tpforn")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((CabecalhoPedidoRetirada e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPedidoRetirada e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<HistoricoDescarregamentoVagao> entity)
		{
			entity.HasKey((HistoricoDescarregamentoVagao e) => e.HistoricoDescarregamentoVagaoStamp).HasName("pk_u_fer01002").IsClustered(clustered: false);
			entity.ToTable("u_fer01002");
			entity.Property((HistoricoDescarregamentoVagao e) => e.HistoricoDescarregamentoVagaoStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer01002stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((HistoricoDescarregamentoVagao e) => e.Stampcabhistorico).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampcabhistorico")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Datadescarregamento).HasColumnType("DateTime?").HasColumnName("datadescarregamento")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Marcada).HasColumnName("marcada");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Novag).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("novag")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Percentagemdescr).HasColumnType("numeric(16, 2)").HasColumnName("percentagemdescr");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Status).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("status")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.CabecalhoPlanoManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer021stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((HistoricoDescarregamentoVagao e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((HistoricoDescarregamentoVagao e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<CabecalhoPlanoManobra> entity)
		{
			entity.HasKey((CabecalhoPlanoManobra e) => e.CabecalhoPlanoManobraStamp).HasName("pk_u_fer021").IsClustered(clustered: false);
			entity.ToTable("u_fer021");
			entity.HasIndex((CabecalhoPlanoManobra e) => new { e.Estacao, e.Data, e.Turno }, "in_U_FER021_User_31").IsUnique().HasFillFactor(80);
			entity.Property((CabecalhoPlanoManobra e) => e.CabecalhoPlanoManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer021stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((CabecalhoPlanoManobra e) => e.Chfturno).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("chfturno")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Aprovado).HasColumnName("aprovado");
			entity.Property((CabecalhoPlanoManobra e) => e.Automatica).HasColumnName("automatica");
			entity.Property((CabecalhoPlanoManobra e) => e.Verificar).HasColumnName("verificar");
			entity.Property((CabecalhoPlanoManobra e) => e.Datasub).HasColumnType("datetime").HasColumnName("datasub")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((CabecalhoPlanoManobra e) => e.Horasub).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("horasub")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Chfturno).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("chfturno")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Codest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Codfer).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codfer")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Data).HasColumnType("datetime").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((CabecalhoPlanoManobra e) => e.Dataaprovacao).HasColumnType("datetime").HasColumnName("dataaprovacao")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((CabecalhoPlanoManobra e) => e.Estacao).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Horaaprovacao).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("horaaprovacao")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Loco).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("loco")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Marcada).HasColumnName("marcada");
			entity.Property((CabecalhoPlanoManobra e) => e.Nomeaprovacao).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("nomeaprovacao")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Nragente).HasColumnType("numeric(6, 0)").HasColumnName("nragente");
			entity.Property((CabecalhoPlanoManobra e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((CabecalhoPlanoManobra e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Patio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("patio")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Tipo).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Tpforn).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tpforn")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Turno).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("turno")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((CabecalhoPlanoManobra e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((CabecalhoPlanoManobra e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UOridest> entity)
		{
			entity.HasKey((UOridest e) => e.UOrideststamp).HasName("pk_u_oridest").IsClustered(clustered: false);
			entity.ToTable("u_oridest");
			entity.Property((UOridest e) => e.UOrideststamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_orideststamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UOridest e) => e.Abreviaestat).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("abreviaestat")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Base).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("base")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Ccusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ccusto")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Cidade).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("cidade")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Cidade2).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("cidade2")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Codccc).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codccc")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Codigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Comboio).HasColumnName("comboio");
			entity.Property((UOridest e) => e.Ebase).HasColumnName("ebase");
			entity.Property((UOridest e) => e.Fornec).HasColumnName("fornec");
			entity.Property((UOridest e) => e.Fronteira).HasColumnName("fronteira");
			entity.Property((UOridest e) => e.Fronteirac).HasColumnName("fronteirac");
			entity.Property((UOridest e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((UOridest e) => e.Km).HasColumnType("numeric(10, 2)").HasColumnName("km");
			entity.Property((UOridest e) => e.Linha).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("linha")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UOridest e) => e.Nacional).HasColumnName("nacional");
			entity.Property((UOridest e) => e.Notamat).HasColumnName("notamat");
			entity.Property((UOridest e) => e.Nrestacao).HasColumnType("numeric(2, 0)").HasColumnName("nrestacao");
			entity.Property((UOridest e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UOridest e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Pais).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("pais")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Patio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("patio")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Regiao).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("regiao")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Retirad).HasColumnName("retirad");
			entity.Property((UOridest e) => e.Selescala).HasColumnName("selescala");
			entity.Property((UOridest e) => e.Tipo).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Troca).HasColumnName("troca");
			entity.Property((UOridest e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UOridest e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UOridest e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<LinhasManobra> entity)
		{
			entity.HasKey((LinhasManobra e) => e.LinhasManobraStamp).HasName("pk_u_fer020").IsClustered(clustered: false);
			entity.ToTable("u_fer020");
			entity.Property((LinhasManobra e) => e.LinhasManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer020stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhasManobra e) => e.Carga).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Codcarga).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codcarga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Coddest).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("coddest")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Codest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Codori).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codori")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Codpatio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codpatio")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Datafim).HasColumnType("DateTime?").HasColumnName("datafim")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((LinhasManobra e) => e.Destino).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("destino")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Enviado).HasColumnName("enviado");
			entity.Property((LinhasManobra e) => e.Estacao).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Horafim).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horafim")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Marcada).HasColumnName("marcada");
			entity.Property((LinhasManobra e) => e.Novg).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("novg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Nravcheg).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nravcheg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Nrbm).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nrbm")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Origem).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("origem")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhasManobra e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Patio).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("patio")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Refcomb).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("refcomb")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Refcomboio).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("refcomboio")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Stampcomb).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampcomb")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Stampevg).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampevg")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Stamppedret).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamppedret")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Stampveic).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampveic")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Tipo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Tpforn).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tpforn")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.ManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer019stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhasManobra e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhasManobra e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhasManobra e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Manobras> entity)
		{
			entity.HasKey((Manobras e) => e.ManobraStamp).HasName("pk_u_fer019").IsClustered(clustered: false);
			entity.ToTable("u_fer019");
			entity.HasIndex((Manobras e) => new { e.Loco, e.Dataini, e.Horaini }, "in_U_FER019_User_28").IsUnique().HasFillFactor(80);
			entity.HasIndex((Manobras e) => e.No, "in_U_FER019_User_29").IsUnique().HasFillFactor(80);
			entity.Property((Manobras e) => e.Pedidoid).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("pedidoid")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Manobras e) => e.ManobraStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer019stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Manobras e) => e.Manobrainiciada).HasColumnName("manobrainiciada");
			entity.Property((Manobras e) => e.Manobrafinalizada).HasColumnName("manobrafinalizada");
			entity.Property((Manobras e) => e.Agente).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("agente")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Comboio).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("comboio")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Dataaceit).HasColumnType("DateTime?").HasColumnName("dataaceit")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((Manobras e) => e.Datafim).HasColumnType("DateTime?").HasColumnName("datafim")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((Manobras e) => e.Dataini).HasColumnType("DateTime?").HasColumnName("dataini")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((Manobras e) => e.Destino).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("destino")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Desvio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("desvio")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Estacao).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Horaaceit).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaaceit")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Horafim).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horafim")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Horaini).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaini")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Loco).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("loco")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Manobras e) => e.Nbreg).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nbreg")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.No).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Noagente).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("noagente")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Nomeaceita).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nomeaceita")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Noseq).HasColumnType("numeric(15, 0)").HasColumnName("noseq");
			entity.Property((Manobras e) => e.Npretirad).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("npretirad")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Npvazios).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("npvazios")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Nravcheg).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nravcheg")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Nrordman).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nrordman")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Obs).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Obsaceit).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("obsaceit")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Manobras e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Patio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("patio")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Ref).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Stampcomb).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampcomb")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Stampordman).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampordman")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Tipo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Tpforn).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tpforn")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Tpreg).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tpreg")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Manobras e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Manobras e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Cl> entity)
		{
			entity.HasKey((Cl e) => new { e.No, e.Estab }).HasName("pk_cl").IsClustered(clustered: false);
			entity.ToTable("cl");
			entity.HasIndex((Cl e) => e.Clstamp, "in_cl_stamp").IsUnique().HasFillFactor(80);
			entity.Property((Cl e) => e.No).HasColumnType("numeric(10, 0)").HasColumnName("no");
			entity.Property((Cl e) => e.Estab).HasColumnType("numeric(3, 0)").HasColumnName("estab");
			entity.Property((Cl e) => e.Ncont).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ncont")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Nome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Nome2).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("nome2")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Cl e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Cl e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<AgenteTransitario> entity)
		{
			entity.HasKey((AgenteTransitario e) => e.AgenteTransitarioStamp).HasName("pk_u_fer007").IsClustered(clustered: false);
			entity.ToTable("u_fer007");
			entity.Property((AgenteTransitario e) => e.AgenteTransitarioStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer007stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((AgenteTransitario e) => e.Agcontct).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("agcontct")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Agemail).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("agemail")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Agenderc).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("agenderc")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Agente).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("agente")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Agtelef).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("agtelef")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Cidade).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("cidade")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Contacto).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("contacto")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Email).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("email")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Endereco).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("endereco")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Marcada).HasColumnName("marcada");
			entity.Property((AgenteTransitario e) => e.No).HasColumnType("numeric(10, 0)").HasColumnName("no");
			entity.Property((AgenteTransitario e) => e.Nome).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("nome")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Nuit).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("nuit")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((AgenteTransitario e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Pais).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("pais")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Telef).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("telef")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((AgenteTransitario e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((AgenteTransitario e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bi> entity)
		{
			entity.HasKey((Bi e) => e.Bistamp).HasName("pk_bi").IsClustered(clustered: false);
			entity.ToTable("bi");
			entity.HasIndex((Bi e) => new { e.Bostamp, e.Lordem }, "in_bi_bostamp").HasFillFactor(70);
			entity.Property((Bi e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.Design).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("design")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Lordem).HasColumnType("numeric(10, 0)").HasColumnName("lordem");
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
			entity.Property((Bi e) => e.Obrano).HasColumnType("numeric(10, 0)").HasColumnName("obrano");
			entity.Property((Bi e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Qtt).HasColumnType("numeric(14, 4)").HasColumnName("qtt");
			entity.Property((Bi e) => e.Qtt2).HasColumnType("numeric(14, 4)").HasColumnName("qtt2");
			entity.Property((Bi e) => e.Ref).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi e) => e.UConta).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_conta")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UTrajcons> entity)
		{
			entity.HasKey((UTrajcons e) => e.UTrajconsstamp).HasName("pk_u_trajcons").IsClustered(clustered: false);
			entity.ToTable("u_trajcons");
			entity.Property((UTrajcons e) => e.UTrajconsstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_trajconsstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UTrajcons e) => e.Tempostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tempostamp")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Refcomboio).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("refcomboio")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Codest).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Codprxmst).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("codprxmst")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Consgno).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Dataop).HasColumnType("datetime").HasColumnName("dataop")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UTrajcons e) => e.Estacao).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Etaproximast).HasColumnType("datetime").HasColumnName("etaproximast")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UTrajcons e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UTrajcons e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTrajcons e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Proximast).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("proximast")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Sentido).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("sentido")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Tipoop).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tipoop")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UTrajcons e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UTrajcons e) => e.Vagno).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("vagno")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UStqueue> entity)
		{
			entity.HasKey((UStqueue e) => e.UStqueuestamp).HasName("pk_u_stqueue").IsClustered(clustered: false);
			entity.ToTable("u_stqueue");
			entity.Property((UStqueue e) => e.UStqueuestamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_stqueuestamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UStqueue e) => e.Tempostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("tempostamp")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Entity).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("entity")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Etabossa).HasColumnType("datetime").HasColumnName("etabossa")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UStqueue e) => e.Etadestination).HasColumnType("datetime").HasColumnName("etadestination")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UStqueue e) => e.Etanextst).HasColumnType("datetime").HasColumnName("etanextst")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UStqueue e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UStqueue e) => e.Nextstation).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("nextstation")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Nextstcode).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("nextstcode")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Destination).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("destination")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Coddest).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("coddest")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Operationdate).HasColumnType("datetime").HasColumnName("operationdate")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((UStqueue e) => e.Operationtype).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("operationtype")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Ord).HasColumnType("numeric(16, 0)").HasColumnName("ord");
			entity.Property((UStqueue e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UStqueue e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Stamptrainreg).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("stamptrainreg")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Station).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("station")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Stationcode).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("stationcode")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Train).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("train")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Trainnumber).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("trainnumber")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Trainorientation).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("trainorientation")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Trainreference).HasMaxLength(120).IsUnicode(unicode: false)
				.HasColumnName("trainreference")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UStqueue e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UStqueue e) => e.Wagonsdata).HasColumnType("text").HasColumnName("wagonsdata")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bi2> entity)
		{
			entity.HasKey((Bi2 e) => e.Bi2stamp).HasName("pk_bi2").IsClustered(clustered: false);
			entity.ToTable("bi2");
			entity.HasIndex((Bi2 e) => e.Bostamp, "in_bi2_bostamp").HasFillFactor(70);
			entity.Property((Bi2 e) => e.Bi2stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bi2stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bi2 e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi2 e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UAdmintr).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_admintr")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UCntdesc).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_cntdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPesagent).HasColumnName("u_pesagent");
			entity.Property((Bi2 e) => e.UCodactst).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_codactst")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UComboio).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_comboio")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UProxst).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_proxst")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UContdcod).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_contdcod")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDatcarrg).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_datcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UEtaini).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_etaini")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UEtanot).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_etanot")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UAgnome).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_agnome")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UAgnuit).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_agnuit")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UAgemail).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("u_agemail")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UAgtelef).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("u_agtelef")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UEstadorv).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("u_estadorv")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDsccmcag).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_dsccmcag")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDualcon).HasColumnName("u_dualcon");
			entity.Property((Bi2 e) => e.UEncerrno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_encerrno")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UEncrradm).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_encrradm")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UEta).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_eta")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UEtafrt).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_etafrt")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UFullempy).HasColumnName("u_fullempy");
			entity.Property((Bi2 e) => e.UChegfrt).HasColumnName("u_chegfrt");
			entity.Property((Bi2 e) => e.URevisto).HasColumnName("u_revisto");
			entity.Property((Bi2 e) => e.UHorcarrg).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_horcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UPeso).HasColumnType("numeric(16, 0)").HasColumnName("u_peso");
			entity.Property((Bi2 e) => e.UStact).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("u_stact")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UStatus).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_status")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDtcheg).HasColumnType("DateTime?").HasColumnName("u_dtcheg")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
			entity.Property((Bi2 e) => e.UTotcont).HasColumnType("numeric(16, 0)").HasColumnName("u_totcont");
			entity.Property((Bi2 e) => e.UUltdtrep).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_ultdtrep")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UDtop).HasColumnType("DateTime?").HasColumnName("u_dtop")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
			entity.Property((Bi2 e) => e.UUltmtmst).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_ultmtmst")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UVagcod).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_vagcod")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UVagdesc).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_vagdesc")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UVagno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_vagno")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UVagtip).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_vagtip")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.UVagvol).HasColumnType("numeric(16, 0)").HasColumnName("u_vagvol");
			entity.Property((Bi2 e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bi2 e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bi2 e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Bo> entity)
		{
			entity.HasKey((Bo e) => new { e.Ndos, e.Obrano, e.Boano }).HasName("pk_bo").IsClustered(clustered: false);
			entity.ToTable("bo");
			entity.HasIndex((Bo e) => new { e.Nmdos, e.Obrano, e.Dataobra, e.Nome, e.No, e.Totaldeb, e.Etotaldeb, e.Bostamp }, "in_bo_bolist").HasFillFactor(70);
			entity.HasIndex((Bo e) => new { e.Ndos, e.No, e.Obrano }, "in_bo_ndos_no").HasFillFactor(70);
			entity.HasIndex((Bo e) => e.No, "in_bo_no").HasFillFactor(70);
			entity.HasIndex((Bo e) => e.Obrano, "in_bo_obrano").HasFillFactor(70);
			entity.HasIndex((Bo e) => e.Pastamp, "in_bo_pastamp").HasFillFactor(70);
			entity.HasIndex((Bo e) => e.Snstamp, "in_bo_snstamp").HasFillFactor(70);
			entity.HasIndex((Bo e) => e.Ssstamp, "in_bo_ssstamp").HasFillFactor(70);
			entity.HasIndex((Bo e) => e.Bostamp, "in_bo_stamp").IsUnique().HasFillFactor(70);
			entity.HasIndex((Bo e) => e.Tpstamp, "in_bo_tpstamp").HasFillFactor(70);
			entity.Property((Bo e) => e.Ndos).HasColumnType("numeric(3, 0)").HasColumnName("ndos");
			entity.Property((Bo e) => e.Obrano).HasColumnType("numeric(10, 0)").HasColumnName("obrano");
			entity.Property((Bo e) => e.Boano).HasColumnType("numeric(4, 0)").HasColumnName("boano");
			entity.Property((Bo e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo e) => e.Alldescli).HasColumnName("alldescli");
			entity.Property((Bo e) => e.Alldesfor).HasColumnName("alldesfor");
			entity.Property((Bo e) => e.Aprovado).HasColumnName("aprovado");
			entity.Property((Bo e) => e.Bo11Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo11_bins");
			entity.Property((Bo e) => e.Bo11Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo11_iva");
			entity.Property((Bo e) => e.Bo12Bins).HasColumnType("numeric(18, 5)").HasColumnName("bo12_bins");
			entity.Property((Bo e) => e.Bo12Iva).HasColumnType("numeric(18, 5)").HasColumnName("bo12_iva");
			entity.Property((Bo e) => e.Bo1tvall).HasColumnType("numeric(18, 5)").HasColumnName("bo_1tvall");
			entity.Property((Bo e) => e.Userimpresso).HasMaxLength(60).IsUnicode(unicode: false)
				.HasColumnName("userimpresso")
				.HasDefaultValueSql("('')");
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
			entity.Property((Bo e) => e.Datafecho).HasColumnType("DateTime?").HasColumnName("datafecho")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
			entity.Property((Bo e) => e.Datafinal).HasColumnType("DateTime?").HasColumnName("datafinal")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
			entity.Property((Bo e) => e.Dataobra).HasColumnType("DateTime?").HasColumnName("dataobra")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
			entity.Property((Bo e) => e.Dataopen).HasColumnType("DateTime?").HasColumnName("dataopen")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
			entity.Property((Bo e) => e.Descc).HasColumnType("numeric(18, 5)").HasColumnName("descc");
			entity.Property((Bo e) => e.Dtclose).HasColumnType("DateTime?").HasColumnName("dtclose")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101',0))");
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
			entity.Property((Bo e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
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
			entity.Property((Bo e) => e.Sqtt12).HasColumnType("numeric(13, 2)").HasColumnName("sqtt12");
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
			entity.Property((Bo e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
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
			entity.Property((Bo2 e) => e.Bo2stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bo2stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Bo2 e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo2 e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo2 e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
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
			entity.Property((Bo3 e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo3 e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UAdmin).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_admin")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UStrackin).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_strackin")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UFccodstd).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("u_fccodstd")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UFcstdest).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("u_fcstdest")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UCoddesvg).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_coddesvg")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UStkdno).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_stkdno")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UStkdnome).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("u_stkdnome")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UCoddesvt).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_coddesvt")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UCodstcag).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_codstcag")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UCodstdet).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_codstdet")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UConsgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_consgno")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UConsgt).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_consgt")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UObs).IsUnicode(unicode: false).HasColumnName("u_obs")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UConsgtip).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_consgtip")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UNocl).IsUnicode(unicode: false).HasColumnName("u_nocl");
			entity.Property((Bo3 e) => e.UCliente).HasMaxLength(240).IsUnicode(unicode: false)
				.HasColumnName("u_cliente")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UStatus).HasMaxLength(40).IsUnicode(unicode: false)
				.HasColumnName("u_status")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UConsgtno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_consgtno")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UDesvcarg).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_desvcarg")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UDesvdest).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_desvdest")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UExped).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_exped")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UAgnome).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_agnome")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UAgnuit).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_agnuit")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UAgemail).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("u_agemail")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UAgtelef).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("u_agtelef")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UExpedno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_expedno")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UFluxocom).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_fluxocom")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UFluxotec).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("u_fluxotec")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UStcarrg).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_stcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UStdest).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("u_stdest")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.UTotveicl).HasColumnType("numeric(16, 0)").HasColumnName("u_totveicl");
			entity.Property((Bo3 e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Bo3 e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Bo3 e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Contentor> entity)
		{
			entity.HasKey((Contentor e) => e.UBocontstamp);
			entity.Property((Contentor e) => e.UBocontstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_bocontstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.ToTable("u_bocont");
			entity.Property((Contentor e) => e.Nocontent).IsUnicode(unicode: false).HasColumnName("nocontent")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Bistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bistamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Contentor e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Coddesvcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("coddesvcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Coddesvdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("coddesvdest")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Codstcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("codstcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Codstdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("codstdest")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Consgt).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("consgt")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Consgtno).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("consgtno")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Contentscode).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("contentscode")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Cargaperigosa).IsUnicode(unicode: false).HasColumnName("cargaperigosa")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Cargagranel).IsUnicode(unicode: false).HasColumnName("cargagranel")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Tamanho).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("tamanho")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Categoria).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("categoria")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Contordno).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("contordno")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Desvcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("desvcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Desvdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("desvdest")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Dtcarrg).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("dtcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Expedidor).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("expedidor")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Codcarg).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("codcarg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Dsccmcarg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("dsccmcarg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Expedidorno).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("expedidorno")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Hrcarrg).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("hrcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Contentor e) => e.Nocl).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("nocl")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Nocontent).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("nocontent")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.customerName).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("customerName")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Contentor e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Peso).HasColumnType("numeric(16, 0)").HasColumnName("peso");
			entity.Property((Contentor e) => e.Stcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("stcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Stdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("stdest")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Contentor e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Contentor e) => e.Vagno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("vagno")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<UBocont> entity)
		{
			entity.HasKey((UBocont e) => e.UBocontstamp).HasName("pk_u_bo2cont").IsClustered(clustered: false);
			entity.ToTable("u_bo2cont");
			entity.Property((UBocont e) => e.UBocontstamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_bo2contstamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UBocont e) => e.Bistamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bistamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((UBocont e) => e.Bostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("bostamp")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Coddesvcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("coddesvcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Coddesvdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("coddesvdest")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Codstcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("codstcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Codstdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("codstdest")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Consgt).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("consgt")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Consgtno).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("consgtno")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Contentscode).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("contentscode")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Cargaperigosa).IsUnicode(unicode: false).HasColumnName("cargaperigosa")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Cargagranel).IsUnicode(unicode: false).HasColumnName("cargagranel")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Tamanho).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("tamanho")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Categoria).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("categoria")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Contordno).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("contordno")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Desvcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("desvcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Desvcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("desvcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Desvdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("desvdest")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Dtcarrg).HasMaxLength(50).HasColumnName("dtcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Expedidor).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("expedidor")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Codcarg).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("codcarg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Dsccmcarg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("dsccmcarg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Expedidorno).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("expedidorno")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Hrcarrg).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("hrcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Marcada).HasColumnName("marcada");
			entity.Property((UBocont e) => e.Nocl).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("nocl")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Nocontent).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("nocontent")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.customerName).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("customerName")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UBocont e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Peso).HasColumnType("numeric(16, 0)").HasColumnName("peso");
			entity.Property((UBocont e) => e.Stcarrg).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("stcarrg")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Stdest).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("stdest")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((UBocont e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((UBocont e) => e.Vagno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("vagno")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<VeiculoFerroviario> entity)
		{
			entity.HasKey((VeiculoFerroviario e) => e.VeiculoFerroviariostamp).HasName("pk_u_fer001").IsClustered(clustered: false);
			entity.ToTable("u_fer001");
			entity.Property((VeiculoFerroviario e) => e.VeiculoFerroviariostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer001stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((VeiculoFerroviario e) => e.Admin).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Capmassa).HasColumnType("numeric(12, 3)").HasColumnName("capmassa");
			entity.Property((VeiculoFerroviario e) => e.Cappax).HasColumnType("numeric(12, 0)").HasColumnName("cappax");
			entity.Property((VeiculoFerroviario e) => e.Capvol).HasColumnType("numeric(12, 3)").HasColumnName("capvol");
			entity.Property((VeiculoFerroviario e) => e.Classe).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("classe")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Codest).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Codsubtipo).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("codsubtipo")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Desiglocal).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("desiglocal")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Estacao).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Freio).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("freio")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Frota).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("frota")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((VeiculoFerroviario e) => e.Local).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Marcada).HasColumnName("marcada");
			entity.Property((VeiculoFerroviario e) => e.Modelo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("modelo")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Motor).HasColumnName("motor");
			entity.Property((VeiculoFerroviario e) => e.No).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Obs).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((VeiculoFerroviario e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Rebocado).HasColumnName("rebocado");
			entity.Property((VeiculoFerroviario e) => e.Serie).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("serie")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Servico).HasColumnName("servico");
			entity.Property((VeiculoFerroviario e) => e.Status).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("status")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Subtipo).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("subtipo")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Tara).HasColumnType("numeric(12, 3)").HasColumnName("tara");
			entity.Property((VeiculoFerroviario e) => e.Tipo).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((VeiculoFerroviario e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((VeiculoFerroviario e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<EntidadeVagao> entity)
		{
			entity.HasKey((EntidadeVagao e) => e.EntidadeVagaostamp).HasName("pk_u_fer002").IsClustered(clustered: false);
			entity.ToTable("u_fer002");
			entity.Property((EntidadeVagao e) => e.EntidadeVagaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer002stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((EntidadeVagao e) => e.Admin).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("admin")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Cambio).HasColumnType("numeric(10, 3)").HasColumnName("cambio");
			entity.Property((EntidadeVagao e) => e.Carga).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Codesto).HasColumnName("codesto").HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Cliente).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("cliente")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Codest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Consorig).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consorig")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Consignat).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("consignat")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Contentor).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("contentor")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Contentor1).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contentor1")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Contentor2).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("contentor2")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Data2).HasColumnType("DateTime?").HasColumnName("data2")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((EntidadeVagao e) => e.Datafim).HasColumnType("DateTime?").HasColumnName("datafim")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((EntidadeVagao e) => e.Dataini).HasColumnType("DateTime?").HasColumnName("dataini")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((EntidadeVagao e) => e.Datapret).HasColumnType("DateTime?").HasColumnName("datapret")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((EntidadeVagao e) => e.Desiglocal).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("desiglocal")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Destdesig).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("destdesig")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Dthrfim).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("dthrfim")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Emcomboio).HasColumnName("emcomboio");
			entity.Property((EntidadeVagao e) => e.Verificar).HasColumnName("verificar");
			entity.Property((EntidadeVagao e) => e.Enceradmin).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("enceradmin")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Encerado).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("encerado")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Estacao).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Estado).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estado")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Expedidor).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("expedidor")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Fechado).HasColumnName("fechado");
			entity.Property((EntidadeVagao e) => e.Fer014stamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("fer014stamp")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Fluxo).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("fluxo")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Ftpeso).HasColumnName("ftpeso");
			entity.Property((EntidadeVagao e) => e.Ftunid).HasColumnName("ftunid");
			entity.Property((EntidadeVagao e) => e.Ftvol).HasColumnName("ftvol");
			entity.Property((EntidadeVagao e) => e.Hora2).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hora2")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Horafim).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("horafim")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Horaini).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaini")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Horapret).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horapret")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Local).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Marcada).HasColumnName("marcada");
			entity.Property((EntidadeVagao e) => e.Moeda).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("moeda")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Ncartao).HasColumnName("ncartao");
			entity.Property((EntidadeVagao e) => e.Ndos).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("ndos")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.New).HasColumnName("new");
			entity.Property((EntidadeVagao e) => e.Nguiadviz).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("nguiadviz")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.No).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Nrpedret).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nrpedret")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Nrreceb).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("nrreceb")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Obsentr).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("obsentr")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Obsrec).HasMaxLength(150).IsUnicode(unicode: false)
				.HasColumnName("obsrec")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Oridesig).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("oridesig")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((EntidadeVagao e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Pesoct1).HasColumnType("numeric(9, 3)").HasColumnName("pesoct1");
			entity.Property((EntidadeVagao e) => e.Pesoct2).HasColumnType("numeric(9, 3)").HasColumnName("pesoct2");
			entity.Property((EntidadeVagao e) => e.Prefact).HasColumnName("prefact");
			entity.Property((EntidadeVagao e) => e.Qtd).HasColumnType("numeric(9, 3)").HasColumnName("qtd");
			entity.Property((EntidadeVagao e) => e.Qtdm3).HasColumnType("numeric(9, 3)").HasColumnName("qtdm3");
			entity.Property((EntidadeVagao e) => e.Qtdton).HasColumnType("numeric(9, 3)").HasColumnName("qtdton");
			entity.Property((EntidadeVagao e) => e.Qtdunid).HasColumnType("numeric(6, 0)").HasColumnName("qtdunid");
			entity.Property((EntidadeVagao e) => e.Retirada).HasColumnName("retirada");
			entity.Property((EntidadeVagao e) => e.Selo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("selo")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampavch).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("stampavch")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampbo).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampbo")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampe2).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampe2")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampfluxo).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampfluxo")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampmanf).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampmanf")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampmanret).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampmanret")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stamppedret).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamppedret")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Stampveic).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampveic")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Tamct1).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("tamct1")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Tamct2).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("tamct2")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Tarifa).HasColumnType("numeric(12, 3)").HasColumnName("tarifa");
			entity.Property((EntidadeVagao e) => e.Teus).HasColumnName("teus");
			entity.Property((EntidadeVagao e) => e.Unid).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("unid")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((EntidadeVagao e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((EntidadeVagao e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Fluxo> entity)
		{
			entity.HasKey((Fluxo e) => e.Fluxostamp).HasName("pk_u_fer004").IsClustered(clustered: false);
			entity.ToTable("u_fer004");
			entity.HasIndex((Fluxo e) => e.Codigo, "in_U_FER004_User_97").IsUnique().HasFillFactor(80);
			entity.Property((Fluxo e) => e.Fluxostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer004stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Fluxo e) => e.Adminvz).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("adminvz")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Codfirma).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("codfirma")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Firmadesig).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("firmadesig")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Carga).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("carga")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Ccor).HasColumnName("ccor");
			entity.Property((Fluxo e) => e.Ccusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ccusto")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Codcarga).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("codcarga")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Codcli).HasColumnType("numeric(10, 0)").HasColumnName("codcli");
			entity.Property((Fluxo e) => e.Coddeste).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("coddeste")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Codigo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("codigo")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Codmer).HasColumnType("numeric(10, 0)").HasColumnName("codmer");
			entity.Property((Fluxo e) => e.Codorige).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codorige")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Consig).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("consig")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Contentor).HasColumnName("contentor");
			entity.Property((Fluxo e) => e.Cres).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("cres")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Datafim).HasColumnType("DateTime?").HasColumnName("datafim")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((Fluxo e) => e.Dataini).HasColumnType("DateTime?").HasColumnName("dataini")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((Fluxo e) => e.Descmer).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("descmer")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Design).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("design")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Designcli).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("designcli")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Destdesig).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("destdesig")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Destdesv).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("destdesv")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Expd).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("expd")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Fctestab).HasColumnType("numeric(3, 0)").HasColumnName("fctestab");
			entity.Property((Fluxo e) => e.Fctno).HasColumnType("numeric(10, 0)").HasColumnName("fctno");
			entity.Property((Fluxo e) => e.Fctnome).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("fctnome")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Inactivo).HasColumnName("inactivo");
			entity.Property((Fluxo e) => e.Linha).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("linha")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Fluxo e) => e.Moeda).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("moeda")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Nreports).HasColumnName("nreports");
			entity.Property((Fluxo e) => e.Obs).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Origdesig).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("origdesig")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Origdesv).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("origdesv")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Fluxo e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Pax).HasColumnName("pax");
			entity.Property((Fluxo e) => e.Porte).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("porte")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Prevcrg).HasColumnType("numeric(10, 0)").HasColumnName("prevcrg");
			entity.Property((Fluxo e) => e.Qtdm3).HasColumnName("qtdm3");
			entity.Property((Fluxo e) => e.Qtdton).HasColumnName("qtdton");
			entity.Property((Fluxo e) => e.Qtdunid).HasColumnName("qtdunid");
			entity.Property((Fluxo e) => e.Refdv).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("refdv")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Refst).HasMaxLength(18).IsUnicode(unicode: false)
				.HasColumnName("refst")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Sazonal).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("sazonal")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Sentido).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("sentido")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Tarifa).HasColumnType("numeric(12, 3)").HasColumnName("tarifa");
			entity.Property((Fluxo e) => e.Taxadest).HasColumnType("numeric(12, 0)").HasColumnName("taxadest");
			entity.Property((Fluxo e) => e.Taxaorig).HasColumnType("numeric(12, 0)").HasColumnName("taxaorig");
			entity.Property((Fluxo e) => e.Teus).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("teus")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Tipo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("tipo")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Tipovg).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("tipovg")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Trafego).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("trafego")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Txdv).HasColumnType("numeric(12, 3)").HasColumnName("txdv");
			entity.Property((Fluxo e) => e.Uni).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("uni")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Fluxo e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Vazio).HasColumnName("vazio");
			entity.Property((Fluxo e) => e.Viaestc).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("viaestc")
				.HasDefaultValueSql("('')");
			entity.Property((Fluxo e) => e.Viaestd).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("viaestd")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<LinhaVeiculoFerroviario> entity)
		{
			entity.HasKey((LinhaVeiculoFerroviario e) => e.LinhaVeiculoFerroviariostamp).HasName("pk_u_fer010").IsClustered(clustered: false);
			entity.ToTable("u_fer010");
			entity.Property((LinhaVeiculoFerroviario e) => e.LinhaVeiculoFerroviariostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer010stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhaVeiculoFerroviario e) => e.Datalarga).HasColumnType("DateTime?").HasColumnName("datalarga")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((LinhaVeiculoFerroviario e) => e.Datatoma).HasColumnType("DateTime?").HasColumnName("datatoma")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((LinhaVeiculoFerroviario e) => e.Decomposto).HasColumnName("decomposto");
			entity.Property((LinhaVeiculoFerroviario e) => e.Desacopl).HasColumnName("desacopl");
			entity.Property((LinhaVeiculoFerroviario e) => e.Estacodigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estacodigo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Estadesign).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estadesign")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Estdcodigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estdcodigo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Estddesign).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estddesign")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Estlcodigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estlcodigo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Estldesign).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("estldesign")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Esttcodigo).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("esttcodigo")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Esttdesign).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("esttdesign")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Horalarga).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horalarga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Horatoma).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horatoma")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Largado).HasColumnName("largado");
			entity.Property((LinhaVeiculoFerroviario e) => e.Local).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("local")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Locallargada).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("locallargada")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Marcada).HasColumnName("marcada");
			entity.Property((LinhaVeiculoFerroviario e) => e.Obsdesacopl).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obsdesacopl")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Obslargada).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obslargada")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhaVeiculoFerroviario e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Seq).HasColumnType("numeric(3, 0)").HasColumnName("seq");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stampcomboio).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampcomboio")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stampdecom).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampdecom")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stampdscpl).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampdscpl")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stampevag).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampevag")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stamplarga).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamplarga")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stampmanobr1).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampmanobr1")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Stamptoma).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamptoma")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Tomado).HasColumnName("tomado");
			entity.Property((LinhaVeiculoFerroviario e) => e.ComboioRegistoStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer025stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((LinhaVeiculoFerroviario e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((LinhaVeiculoFerroviario e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((LinhaVeiculoFerroviario e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Recebimento> entity)
		{
			entity.HasKey((Recebimento e) => e.Recebimentostamp).HasName("pk_u_fer014").IsClustered(clustered: false);
			entity.ToTable("u_fer014");
			entity.Property((Recebimento e) => e.Recebimentostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer014stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Recebimento e) => e.Codfirma).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("codfirma")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Comboio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("comboio")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Comconsig).HasColumnName("comconsig");
			entity.Property((Recebimento e) => e.Compositor).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("compositor")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Consgno).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("consgno")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Data).HasColumnType("datetime").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Recebimento e) => e.Desvio).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("desvio")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Estacao).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Firmadesig).HasMaxLength(200).IsUnicode(unicode: false)
				.HasColumnName("firmadesig")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Fluxo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("fluxo")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Hora).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hora")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Loco).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("loco")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Recebimento e) => e.No).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("no")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Obs).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Recebimento e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Verificar).HasColumnName("verificar");
			entity.Property((Recebimento e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Recebimento e) => e.Revisaostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("revisaostamp")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Recebimento e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<Tempos> entity)
		{
			entity.HasKey((Tempos e) => e.TemposStamp).HasName("pk_u_fer030").IsClustered(clustered: false);
			entity.ToTable("u_fer030");
			entity.Property((Tempos e) => e.TemposStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_FER030stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Tempos e) => e.Codest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codest")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Control).HasColumnType("numeric(1, 0)").HasColumnName("control");
			entity.Property((Tempos e) => e.Cruza1).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("cruza1")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Cruza2).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("cruza2")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Ctempo).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ctempo")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Datac).HasColumnType("datetime").HasColumnName("datac")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Tempos e) => e.Datap).HasColumnType("datetime").HasColumnName("datap")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Tempos e) => e.Descpectm).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("descpectm")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Dist).HasColumnType("numeric(10, 2)").HasColumnName("dist");
			entity.Property((Tempos e) => e.Dtprevc).HasColumnType("datetime").HasColumnName("dtprevc")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Tempos e) => e.Dtprevp).HasColumnType("datetime").HasColumnName("dtprevp")
				.HasDefaultValueSql("(CONVERT([datetime],'19000101'))");
			entity.Property((Tempos e) => e.Estacao).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("estacao")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Horac).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horac")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Horap).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horap")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Hrprevc).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hrprevc")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Hrprevp).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hrprevp")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Justparg).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("justparg")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Justperc).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("justperc")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Km).HasColumnType("numeric(10, 2)").HasColumnName("km");
			entity.Property((Tempos e) => e.Marcada).HasColumnName("marcada");
			entity.Property((Tempos e) => e.Marcado).HasColumnName("marcado");
			entity.Property((Tempos e) => e.Notifchegada).HasColumnName("notifchegada");
			entity.Property((Tempos e) => e.Notifpartida).HasColumnName("notifpartida");
			entity.Property((Tempos e) => e.Obs).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Ousrdata).HasColumnType("datetime").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Tempos e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Pectm).HasColumnType("numeric(10, 0)").HasColumnName("pectm");
			entity.Property((Tempos e) => e.Tempo).HasColumnType("numeric(10, 0)").HasColumnName("tempo");
			entity.Property((Tempos e) => e.Tperca).HasColumnType("numeric(3, 0)").HasColumnName("tperca");
			entity.Property((Tempos e) => e.ComboioRegistoStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer025stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((Tempos e) => e.Usrdata).HasColumnType("datetime").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((Tempos e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((Tempos e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<ComboioRegisto> entity)
		{
			entity.HasKey((ComboioRegisto e) => e.ComboioRegistoStamp).HasName("pk_u_fer025").IsClustered(clustered: false);
			entity.ToTable("u_fer025");
			entity.Property((ComboioRegisto e) => e.ComboioRegistoStamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer025stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((ComboioRegisto e) => e.Alugado).HasColumnName("alugado");
			entity.Property((ComboioRegisto e) => e.Cancelado).HasColumnName("cancelado");
			entity.Property((ComboioRegisto e) => e.Partidanotif).HasColumnName("partidanotif");
			entity.Property((ComboioRegisto e) => e.Chegnotif).HasColumnName("chegnotif");
			entity.Property((ComboioRegisto e) => e.Canceladom).HasMaxLength(70).IsUnicode(unicode: false)
				.HasColumnName("canceladom")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Notificar).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("notificar")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Cauda).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("cauda")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((ComboioRegisto e) => e.Caudaant).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("caudaant")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((ComboioRegisto e) => e.Telemetro).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("telemetro")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((ComboioRegisto e) => e.Cancelo).HasColumnName("cancelo");
			entity.Property((ComboioRegisto e) => e.Cancelom).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("cancelom")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Cancelp).HasColumnName("cancelp");
			entity.Property((ComboioRegisto e) => e.Cancelpm).HasMaxLength(80).IsUnicode(unicode: false)
				.HasColumnName("cancelpm")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Cclassif).HasMaxLength(1).IsUnicode(unicode: false)
				.HasColumnName("cclassif")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Ccusto).HasMaxLength(20).IsUnicode(unicode: false)
				.HasColumnName("ccusto")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Chegou).HasColumnName("chegou");
			entity.Property((ComboioRegisto e) => e.Cisolado).HasColumnName("cisolado");
			entity.Property((ComboioRegisto e) => e.Coddest).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("coddest")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Codorig).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("codorig")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Crota).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("crota")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Ctipo).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("ctipo")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Cumtrip).HasColumnName("cumtrip");
			entity.Property((ComboioRegisto e) => e.Data).HasColumnType("DateTime?").HasColumnName("data")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Datac).HasColumnType("DateTime?").HasColumnName("datac")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Dataes).HasColumnType("DateTime?").HasColumnName("dataes")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Dataff).HasColumnType("DateTime?").HasColumnName("dataff")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Dataif).HasColumnType("DateTime?").HasColumnName("dataif")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Datap).HasColumnType("DateTime?").HasColumnName("datap")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Datar).HasColumnType("DateTime?").HasColumnName("datar")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Datat).HasColumnType("DateTime?").HasColumnName("datat")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Datats).HasColumnType("DateTime?").HasColumnName("datats")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Dclassif).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("dclassif")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Desacoplou).HasColumnName("desacoplou");
			entity.Property((ComboioRegisto e) => e.Destino).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("destino")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Dlinha).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("dlinha")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Drota).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("drota")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Dtdesacopl).HasColumnType("DateTime?").HasColumnName("dtdesacopl")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Dtipo).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("dtipo")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Dtprev).HasColumnType("DateTime?").HasColumnName("dtprev")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Dttab).HasColumnType("DateTime?").HasColumnName("dttab")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Escolta).HasColumnName("escolta");
			entity.Property((ComboioRegisto e) => e.Estdesacopl).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("estdesacopl")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Hora).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hora")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horaap).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaap")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horaes).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaes")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horaff).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaff")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horaif).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horaif")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horap).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horap")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horar).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horar")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horat).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horat")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Horats).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("horats")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Hrdesacopl).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hrdesacopl")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Hrprev).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hrprev")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Hrtab).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("hrtab")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Linha).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("linha")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Loc1).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("loc1")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Loc2).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("loc2")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Loc3).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("loc3")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Localexp).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("localexp")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Locdesacopl).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("locdesacopl")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Marcada).HasColumnName("marcada");
			entity.Property((ComboioRegisto e) => e.Numero).HasMaxLength(10).IsUnicode(unicode: false)
				.HasColumnName("numero")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Obs).HasMaxLength(100).IsUnicode(unicode: false)
				.HasColumnName("obs")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Obsapro).HasMaxLength(240).IsUnicode(unicode: false)
				.HasColumnName("obsapro")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Obsaprov).HasMaxLength(240).IsUnicode(unicode: false)
				.HasColumnName("obsaprov")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Obsaprov1).HasMaxLength(240).IsUnicode(unicode: false)
				.HasColumnName("obsaprov1")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Origem).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("origem")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((ComboioRegisto e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Partiu).HasColumnName("partiu");
			entity.Property((ComboioRegisto e) => e.Pbruprt).HasColumnType("numeric(9, 3)").HasColumnName("pbruprt");
			entity.Property((ComboioRegisto e) => e.Pbruto).HasColumnType("numeric(5, 0)").HasColumnName("pbruto");
			entity.Property((ComboioRegisto e) => e.Perctot).HasColumnType("numeric(10, 2)").HasColumnName("perctot");
			entity.Property((ComboioRegisto e) => e.Qtdvg).HasColumnType("numeric(3, 0)").HasColumnName("qtdvg");
			entity.Property((ComboioRegisto e) => e.Qtdvguprt).HasColumnType("numeric(3, 0)").HasColumnName("qtdvguprt");
			entity.Property((ComboioRegisto e) => e.Recobs).HasMaxLength(250).IsUnicode(unicode: false)
				.HasColumnName("recobs")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Ref).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("ref")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Resprevis).HasMaxLength(55).IsUnicode(unicode: false)
				.HasColumnName("resprevis")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Revisado).HasColumnName("revisado");
			entity.Property((ComboioRegisto e) => e.Sentido).HasMaxLength(1).IsUnicode(unicode: false)
				.HasColumnName("sentido")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Stampcombf).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampcombf")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Stamphorario).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stamphorario")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Stampprev).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("stampprev")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Tracc).HasMaxLength(15).IsUnicode(unicode: false)
				.HasColumnName("tracc")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Tripapro).HasColumnName("tripapro");
			entity.Property((ComboioRegisto e) => e.Tripaprov).HasColumnName("tripaprov");
			entity.Property((ComboioRegisto e) => e.Tripaprov1).HasColumnName("tripaprov1");
			entity.Property((ComboioRegisto e) => e.Ultdatap).HasColumnType("DateTime?").HasColumnName("ultdatap")
				.HasDefaultValueSql("(CONVERT([DateTime?],'19000101'))");
			entity.Property((ComboioRegisto e) => e.Ulthorap).HasMaxLength(5).IsUnicode(unicode: false)
				.HasColumnName("ulthorap")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Ultlocal).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ultlocal")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((ComboioRegisto e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((ComboioRegisto e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<PrevisaoMensalCarga> entity)
		{
			entity.HasKey((PrevisaoMensalCarga e) => e.PrevisaoMensalCargastamp).HasName("pk_u_fer072").IsClustered(clustered: false);
			entity.ToTable("u_fer072");
			entity.Property((PrevisaoMensalCarga e) => e.PrevisaoMensalCargastamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer072stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((PrevisaoMensalCarga e) => e.Ano).HasMaxLength(4).IsUnicode(unicode: false)
				.HasColumnName("ano")
				.HasDefaultValueSql("('')");
			entity.Property((PrevisaoMensalCarga e) => e.Marcada).HasColumnName("marcada");
			entity.Property((PrevisaoMensalCarga e) => e.Mes).HasMaxLength(2).IsUnicode(unicode: false)
				.HasColumnName("mes")
				.HasDefaultValueSql("('')");
			entity.Property((PrevisaoMensalCarga e) => e.Ousrdata).HasColumnType("DateTime?").HasColumnName("ousrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((PrevisaoMensalCarga e) => e.Ousrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("ousrhora")
				.HasDefaultValueSql("('')");
			entity.Property((PrevisaoMensalCarga e) => e.Ousrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("ousrinis")
				.HasDefaultValueSql("('')");
			entity.Property((PrevisaoMensalCarga e) => e.Prevcrg).HasColumnType("numeric(10, 0)").HasColumnName("prevcrg");
			entity.Property((PrevisaoMensalCarga e) => e.Fluxostamp).HasMaxLength(25).IsUnicode(unicode: false)
				.HasColumnName("u_fer004stamp")
				.HasDefaultValueSql("('')")
				.IsFixedLength();
			entity.Property((PrevisaoMensalCarga e) => e.Usrdata).HasColumnType("DateTime?").HasColumnName("usrdata")
				.HasDefaultValueSql("(getdate())");
			entity.Property((PrevisaoMensalCarga e) => e.Usrhora).HasMaxLength(8).IsUnicode(unicode: false)
				.HasColumnName("usrhora")
				.HasDefaultValueSql("('')");
			entity.Property((PrevisaoMensalCarga e) => e.Usrinis).HasMaxLength(30).IsUnicode(unicode: false)
				.HasColumnName("usrinis")
				.HasDefaultValueSql("('')");
		});
		modelBuilder.Entity(delegate(EntityTypeBuilder<ULogs> entity)
		{
			entity.HasKey((ULogs e) => e.ULogsstamp).HasName("PK__u_logs__9803C30F60140409");
			entity.ToTable("u_logs");
			entity.Property((ULogs e) => e.ULogsstamp).HasMaxLength(50).IsUnicode(unicode: false)
				.HasColumnName("u_logsstamp");
			entity.Property((ULogs e) => e.Code).IsUnicode(unicode: false).HasColumnName("code");
			entity.Property((ULogs e) => e.Ip).IsUnicode(unicode: false).HasColumnName("ip");
			entity.Property((ULogs e) => e.Content).IsUnicode(unicode: false).HasColumnName("content");
			entity.Property((ULogs e) => e.ResponseText).IsUnicode(unicode: false).HasColumnName("responsetext");
			entity.Property((ULogs e) => e.Data).HasColumnType("DateTime?").HasColumnName("data");
			entity.Property((ULogs e) => e.Operation).IsUnicode(unicode: false).HasColumnName("operation");
			entity.Property((ULogs e) => e.RequestId).IsUnicode(unicode: false).HasColumnName("requestId");
			entity.Property((ULogs e) => e.ResponseDesc).IsUnicode(unicode: false).HasColumnName("responseDesc");
		});
	}
}
