using System;

namespace SGOFWS.Domains.Models;

public class LocaisFerroviarios
{
	public string LocaisFerroviariosstamp { get; set; }

	public string Codigo { get; set; }

	public string Design { get; set; }

	public string Estacao { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public string Designestac { get; set; }

	public bool Desvio { get; set; }

	public string Local { get; set; }

	public string Linhafer { get; set; }

	public bool Fluxos { get; set; }

	public bool Isento { get; set; }

	public string Cdesvio { get; set; }

	public decimal Mzn { get; set; }

	public decimal Usd { get; set; }

	public decimal Zar { get; set; }

	public bool Circulacao { get; set; }

	public bool Resguardo { get; set; }

	public bool Servico { get; set; }

	public bool Inactivo { get; set; }

	public bool Revisao { get; set; }

	public bool Kpi { get; set; }

	public bool Fornec { get; set; }

	public bool Retirad { get; set; }
}
