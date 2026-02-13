using System;

namespace SGOFWS.Domains.Models;

public class PrevisaoMensalCarga
{
	public string PrevisaoMensalCargastamp { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public string Ano { get; set; }

	public string Mes { get; set; }

	public decimal? Prevcrg { get; set; }

	public string Fluxostamp { get; set; }
}
