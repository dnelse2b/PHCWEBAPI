using System;

namespace SGOFWS.Domains.Models;

public class UTopup
{
	public string UTopupstamp { get; set; }

	public string Pedidoid { get; set; }

	public bool Aprovado { get; set; }

	public string Tipo { get; set; }

	public DateTime Data { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }
}
