using System;

namespace SGOFWS.Domains.Models;

public class VeiculoFerroviario
{
	public string VeiculoFerroviariostamp { get; set; }

	public string Desiglocal { get; set; }

	public string Codest { get; set; }

	public string Estacao { get; set; }

	public string No { get; set; }

	public string Tipo { get; set; }

	public string Subtipo { get; set; }

	public string Serie { get; set; }

	public string Modelo { get; set; }

	public string Classe { get; set; }

	public string Admin { get; set; }

	public decimal? Tara { get; set; }

	public decimal? Capmassa { get; set; }

	public decimal? Capvol { get; set; }

	public decimal? Cappax { get; set; }

	public string Freio { get; set; }

	public string Frota { get; set; }

	public string Status { get; set; }

	public string Local { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public bool? Motor { get; set; }

	public bool? Rebocado { get; set; }

	public bool? Servico { get; set; }

	public string Obs { get; set; }

	public bool? Inactivo { get; set; }

	public string Codsubtipo { get; set; }
}
