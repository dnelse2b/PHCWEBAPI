using System;

namespace SGOFWS.Domains.Models;

public class UEstacnt
{
	public string UEstacntstamp { get; set; }

	public string Estacao { get; set; }

	public string Proximaestacao { get; set; }

	public string Codigo { get; set; }

	public bool Bossa { get; set; }

	public string Ousrinis { get; set; }

	public decimal Ordem { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }
}
