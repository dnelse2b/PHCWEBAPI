using System;
using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class LinhasPlanoManobra
{
	public string LinhasPlanoManobraStamp { get; set; } = "";

	public string Novg { get; set; } = "";

	public string Stampevg { get; set; } = "";

	public string Codorig { get; set; } = "";

	public string Consgno { get; set; } = "";

	public string Origem { get; set; } = "";

	public string Coddest { get; set; } = "";

	public string Destino { get; set; } = "";

	public string Tipovg { get; set; } = "";

	public string Codcarga { get; set; } = "";

	public string Carga { get; set; } = "";

	public string Loco { get; set; } = "";

	public string CabecalhoPlanoManobraStamp { get; set; } = "";

	public string Ousrinis { get; set; } = "";

	public DateTime? Ousrdata { get; set; } = DateTime.Now;

	public string Ousrhora { get; set; } = DateTime.Now.ToString("HH:mm:ss");

	public string Usrinis { get; set; } = "";

	public DateTime? Usrdata { get; set; } = DateTime.Now;

	public string Usrhora { get; set; } = DateTime.Now.ToString("HH:mm:ss");

	public bool? Marcada { get; set; } = false;

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
