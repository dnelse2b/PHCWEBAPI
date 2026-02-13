using System;

namespace SGOFWS.Domains.Models;

public class Tempos
{
	public string Codest { get; set; }

	public string Estacao { get; set; }

	public DateTime Datac { get; set; }

	public string Horac { get; set; }

	public DateTime Datap { get; set; }

	public string Horap { get; set; }

	public decimal Tperca { get; set; }

	public string Justperc { get; set; }

	public string Justparg { get; set; }

	public string Cruza1 { get; set; }

	public string Cruza2 { get; set; }

	public string Obs { get; set; }

	public DateTime Dtprevc { get; set; }

	public string ComboioRegistoStamp { get; set; }

	public DateTime Dtprevp { get; set; }

	public string Hrprevp { get; set; }

	public string Hrprevc { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public string TemposStamp { get; set; }

	public decimal Km { get; set; }

	public decimal Control { get; set; }

	public bool Marcado { get; set; }

	public decimal Dist { get; set; }

	public decimal Tempo { get; set; }

	public string Ctempo { get; set; }

	public decimal Pectm { get; set; }

	public string Descpectm { get; set; }

	public bool Notifchegada { get; set; }

	public bool Notifpartida { get; set; }
}
