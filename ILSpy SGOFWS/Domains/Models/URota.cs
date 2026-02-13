using System;

namespace SGOFWS.Domains.Models;

public class URota
{
	public string URotastamp { get; set; }

	public string Linha { get; set; }

	public string Codest { get; set; }

	public string Estacao { get; set; }

	public string Destino { get; set; }

	public string Coddest { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }
}
