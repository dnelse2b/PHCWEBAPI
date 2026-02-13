using System;

namespace SGOFWS.Domains.Models;

public class CabecalhoPedidoRetirada
{
	public string CabecalhoPedidoRetiradaStamp { get; set; }

	public DateTime? Data { get; set; }

	public string Hora { get; set; }

	public string Fluxo { get; set; }

	public string Estacao { get; set; }

	public string Desvio { get; set; }

	public string Pedidoforn { get; set; }

	public string Obs { get; set; }

	public string Ousrinis { get; set; }

	public DateTime? Ousrdata { get; set; } = DateTime.Now.Date;

	public DateTime? Datasub { get; set; } = new DateTime(1900, 1, 1);

	public string Horasub { get; set; } = "";

	public string Ousrhora { get; set; } = DateTime.Now.ToString("HH:mm:ss");

	public string Usrinis { get; set; } = "";

	public string Pedidoid { get; set; } = "";

	public DateTime? Usrdata { get; set; } = DateTime.Now.Date;

	public string Usrhora { get; set; } = DateTime.Now.ToString("HH:mm:ss");

	public bool? Verificar { get; set; } = false;

	public bool? Automatica { get; set; } = false;

	public bool? Aprovado { get; set; }

	public bool? Marcada { get; set; }

	public string Nrped { get; set; }

	public decimal? Seqporterm { get; set; }

	public string Nomeaprovacao { get; set; }

	public string Horaaprovacao { get; set; }

	public DateTime? Dataaprovacao { get; set; }

	public string Codest { get; set; }

	public string Patio { get; set; }

	public string Loco { get; set; }

	public string Tpforn { get; set; }
}
