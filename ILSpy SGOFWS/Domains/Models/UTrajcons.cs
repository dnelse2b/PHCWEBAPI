using System;
using SGOFWS.Extensions;

namespace SGOFWS.Domains.Models;

public class UTrajcons
{
	public string UTrajconsstamp { get; set; } = 25.UseThisSizeForStamp();

	public DateTime? Dataop { get; set; }

	public string Consgno { get; set; }

	public string Vagno { get; set; }

	public string Tempostamp { get; set; }

	public DateTime? Etaproximast { get; set; }

	public string Proximast { get; set; }

	public string Codprxmst { get; set; }

	public string Refcomboio { get; set; }

	public string Tipoop { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public string Estacao { get; set; }

	public string Codest { get; set; }

	public string Sentido { get; set; }
}
