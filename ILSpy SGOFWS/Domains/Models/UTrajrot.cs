using System;

namespace SGOFWS.Domains.Models;

public class UTrajrot
{
	public string UTrajrotstamp { get; set; }

	public string Estacao { get; set; }

	public string Codigo { get; set; }

	public bool Bossa { get; set; }

	public decimal Ordem { get; set; }

	public string Codprxst { get; set; }

	public string Proximaestacao { get; set; }

	public string URotastamp { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public bool Destaque { get; set; }

	public bool Destino { get; set; }

	public bool Fronteira { get; set; }

	public string Admin { get; set; }

	public string Descesta { get; set; }

	public string Hora { get; set; }
}
