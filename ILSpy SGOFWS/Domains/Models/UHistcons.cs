using System;

namespace SGOFWS.Domains.Models;

public class UHistcons
{
	public string UHistconsstamp { get; set; }

	public string Consgno { get; set; }

	public string Content { get; set; }

	public DateTime? Data { get; set; }

	public string Provider { get; set; }

	public string Admin { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public string Response { get; set; }
}
