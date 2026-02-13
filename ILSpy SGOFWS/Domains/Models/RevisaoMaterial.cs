using System;

namespace SGOFWS.Domains.Models;

public class RevisaoMaterial
{
	public string RevisaoMaterialstamp { get; set; }

	public string Comboio { get; set; }

	public DateTime? Dtcomb { get; set; }

	public string Hrcomb { get; set; }

	public DateTime? Dtirev { get; set; }

	public string Hrirev { get; set; }

	public DateTime? Dtfrev { get; set; }

	public string Hrfrev { get; set; }

	public string Revcfm { get; set; }

	public decimal Norev { get; set; }

	public string Revoutr { get; set; }

	public string Estacao { get; set; }

	public string Ref { get; set; }

	public string No { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public bool? Sync { get; set; } = true;

	public string Bistamp { get; set; }

	public string Codrev { get; set; }

	public string Statusrev { get; set; }

	public string Lado { get; set; }

	public string Codest { get; set; }

	public string Sentido { get; set; }

	public string Docori { get; set; }
}
