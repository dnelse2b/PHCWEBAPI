using System;

namespace SGOFWS.Domains.Models;

public class Bo3
{
	public string Bo3stamp { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool UVerif { get; set; }

	public string UCoddesvg { get; set; }

	public string UCmpstamp { get; set; } = "";

	public string UCoddesvt { get; set; }

	public string UCodstcag { get; set; }

	public string UCodstdet { get; set; }

	public string UConsgt { get; set; }

	public string UConsgtip { get; set; }

	public string UConsgtno { get; set; }

	public string UDesvcarg { get; set; }

	public string UDesvdest { get; set; }

	public string UExped { get; set; }

	public string UExpedno { get; set; }

	public string UStcarrg { get; set; }

	public string UStkdno { get; set; }

	public string UStkdnome { get; set; }

	public string UCliente { get; set; }

	public decimal? UNocl { get; set; }

	public string UStdest { get; set; }

	public string UAgnome { get; set; } = "";

	public string UAgemail { get; set; } = "";

	public string UAgtelef { get; set; } = "";

	public string UAgnuit { get; set; }

	public string UStatus { get; set; }

	public string UFluxocom { get; set; }

	public string UFluxotec { get; set; }

	public string UAdmin { get; set; }

	public string UStrackin { get; set; }

	public string UFccodstd { get; set; }

	public string UFcstdest { get; set; }

	public string UObs { get; set; }

	public string UConsgno { get; set; }

	public decimal? UTotveicl { get; set; }
}
