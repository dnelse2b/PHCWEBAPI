using System;

namespace SGOFWS.Domains.Models;

public class PatiosManobra
{
	public string PatiosManobrastamp { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public string Codigo { get; set; }

	public string Design { get; set; }

	public bool Inactivo { get; set; }
}
