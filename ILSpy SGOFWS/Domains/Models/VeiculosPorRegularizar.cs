using System;

namespace SGOFWS.Domains.Models;

public class VeiculosPorRegularizar
{
	public string VeiculosPorRegularizarstamp { get; set; }

	public string Novg { get; set; }

	public string Tipo { get; set; }

	public string Stamppedido { get; set; }

	public string Coderr { get; set; }

	public string Descerr { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public bool Regularizado { get; set; }
}
