using System;

namespace SGOFWS.Domains.Models;

public class USync
{
	public string USyncstamp { get; set; }

	public string Data { get; set; }

	public string? Processcode { get; set; }

	public string? Processid { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public DateTime Dataproc { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public DateTime Date { get; set; }

	public string Status { get; set; }

	public string Responseproc { get; set; }
}
