using System;

namespace SGOFWS.Domains.Models;

public class AdmnistracaoVizinha
{
	public string AdmnistracaoVizinhastamp { get; set; }

	public string Codigo { get; set; }

	public string Design { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public bool Vizinh { get; set; }

	public bool Locos { get; set; }

	public bool Operador { get; set; }

	public string Fluxov { get; set; }

	public string Fluxodesign { get; set; }
}
