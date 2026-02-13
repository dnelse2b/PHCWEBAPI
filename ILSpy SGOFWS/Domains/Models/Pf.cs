using System;

namespace SGOFWS.Domains.Models;

public class Pf
{
	public string Pfstamp { get; set; }

	public decimal? Codigo { get; set; }

	public string Resumo { get; set; }

	public string Descricao { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public bool? Canselectproducts { get; set; }
}
