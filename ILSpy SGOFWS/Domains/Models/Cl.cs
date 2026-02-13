using System;
using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class Cl
{
	public string Clstamp { get; set; }

	public string Nome { get; set; }

	public decimal? No { get; set; }

	public decimal? Estab { get; set; }

	public string Ncont { get; set; }

	public string Nome2 { get; set; } = "";

	public string Ousrinis { get; set; } = "";

	public DateTime? Ousrdata { get; set; } = DateTime.Now.Date;

	public string Ousrhora { get; set; } = "";

	public string Usrinis { get; set; } = "";

	public DateTime? Usrdata { get; set; } = DateTime.Now.Date;

	public string Usrhora { get; set; } = "";

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
