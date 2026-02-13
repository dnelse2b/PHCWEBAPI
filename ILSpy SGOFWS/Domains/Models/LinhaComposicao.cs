using System;

namespace SGOFWS.Domains.Models;

public class LinhaComposicao
{
	public string LinhaComposicaostamp { get; set; }

	public string Composicaostamp { get; set; }

	public string Vagtip { get; set; }

	public string Oristamp { get; set; }

	public decimal Volume { get; set; }

	public string Carga { get; set; }

	public decimal Peso { get; set; }

	public decimal Sequencia { get; set; }

	public string Admin { get; set; }

	public bool Fullempty { get; set; }

	public string Consgno { get; set; }

	public string Docno { get; set; }

	public string Docori { get; set; }

	public string Ousrinis { get; set; }

	public DateTime Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool Marcada { get; set; }

	public bool Adicionado { get; set; }

	public string Estadocomp { get; set; }

	public string Novg { get; set; }
}
