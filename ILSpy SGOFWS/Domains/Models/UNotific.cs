using System;
using SGOFWS.Extensions;

namespace SGOFWS.Domains.Models;

public class UNotific
{
	public string UNotificstamp { get; set; } = 25.UseThisSizeForStamp();

	public string Entidade { get; set; }

	public string Tabela { get; set; }

	public string Tabstamp { get; set; }

	public string Status { get; set; }

	public string Obs { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public string Sentido { get; set; }

	public string Operacao { get; set; }
}
