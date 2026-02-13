using System;

namespace SGOFWS.Domains.Models;

public class Composicao
{
	public string Composicaostamp { get; set; }

	public string Estacao { get; set; }

	public string Codest { get; set; }

	public string Comboio { get; set; }

	public string No { get; set; }

	public DateTime? Dataini { get; set; }

	public DateTime? Datafim { get; set; }

	public string Nomecomp { get; set; }

	public decimal Nocomp { get; set; }

	public string Sentido { get; set; }

	public string Docori { get; set; }

	public string Horaini { get; set; } = "";

	public string Horafim { get; set; } = "";

	public string Ref { get; set; }

	public string Obs { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public string Sequencia { get; set; }

	public bool? Marcada { get; set; } = true;

	public bool? Sync { get; set; } = true;
}
