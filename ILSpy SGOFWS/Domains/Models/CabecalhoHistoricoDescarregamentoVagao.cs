using System;

namespace SGOFWS.Domains.Models;

public class CabecalhoHistoricoDescarregamentoVagao
{
	public string CabecalhoHistoricoDescarregamentoVagaostamp { get; set; }

	public string Pedidoid { get; set; }

	public decimal? Totalforn { get; set; }

	public decimal? Percentagemdesc { get; set; }

	public decimal? Totvagdesc { get; set; }

	public decimal? Totpordesc { get; set; }

	public decimal? Totdescarrngd { get; set; }

	public decimal? Meddescmin { get; set; }

	public decimal? Totalforncarga { get; set; }

	public decimal? Totaldesccarga { get; set; }

	public decimal? Meddesccargamin { get; set; }

	public decimal? Cargapordesc { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; }

	public string Ousrhora { get; set; }

	public string Usrinis { get; set; }

	public DateTime? Usrdata { get; set; }

	public string Usrhora { get; set; }

	public bool? Marcada { get; set; }

	public string Manobrastamp { get; set; }

	public string Planomanobrastamp { get; set; }
}
