using System;

namespace SGOFWS.Domains.Models;

public class HistoricoDescarregamentoVagao
{
	public string HistoricoDescarregamentoVagaoStamp { get; set; }

	public string Novag { get; set; }

	public string Status { get; set; }

	public string Stampcabhistorico { get; set; }

	public string Consgno { get; set; }

	public decimal? Percentagemdescr { get; set; }

	public DateTime? Datadescarregamento { get; set; }

	public string CabecalhoPlanoManobraStamp { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }
}
