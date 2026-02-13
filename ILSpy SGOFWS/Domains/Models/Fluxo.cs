using System;

namespace SGOFWS.Domains.Models;

public class Fluxo
{
	public string Fluxostamp { get; set; }

	public string Origdesig { get; set; }

	public string Destdesig { get; set; }

	public string Codigo { get; set; }

	public string Codfirma { get; set; }

	public string Firmadesig { get; set; }

	public string Tipo { get; set; }

	public string Expd { get; set; }

	public string Consig { get; set; }

	public string Codorige { get; set; }

	public string Coddeste { get; set; }

	public string Origdesv { get; set; }

	public string Destdesv { get; set; }

	public string Tipovg { get; set; }

	public decimal? Tarifa { get; set; }

	public string Moeda { get; set; }

	public string Codcarga { get; set; }

	public string Carga { get; set; }

	public string Porte { get; set; }

	public DateTime? Dataini { get; set; }

	public DateTime? Datafim { get; set; }

	public decimal? Codcli { get; set; }

	public string Designcli { get; set; }

	public decimal? Fctno { get; set; }

	public decimal? Fctestab { get; set; }

	public string Fctnome { get; set; }

	public string Sentido { get; set; }

	public string Trafego { get; set; }

	public string Cres { get; set; }

	public string Linha { get; set; }

	public decimal? Prevcrg { get; set; }

	public string Sazonal { get; set; }

	public bool? Ccor { get; set; }

	public string Ccusto { get; set; }

	public bool? Inactivo { get; set; }

	public string Design { get; set; }

	public string Obs { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public string Uni { get; set; }

	public decimal? Codmer { get; set; }

	public string Descmer { get; set; }

	public bool? Contentor { get; set; }

	public string Teus { get; set; }

	public bool? Vazio { get; set; }

	public string Adminvz { get; set; }

	public string Viaestc { get; set; }

	public string Viaestd { get; set; }

	public bool? Qtdton { get; set; }

	public bool? Qtdm3 { get; set; }

	public bool? Qtdunid { get; set; }

	public bool? Pax { get; set; }

	public string Refst { get; set; }

	public string Refdv { get; set; }

	public decimal? Txdv { get; set; }

	public bool? Nreports { get; set; }

	public decimal? Taxaorig { get; set; }

	public decimal? Taxadest { get; set; }
}
