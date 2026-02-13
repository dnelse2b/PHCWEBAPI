using System;
using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class CabecalhoPlanoManobra
{
	public string CabecalhoPlanoManobraStamp { get; set; } = "";

	public DateTime? Data { get; set; }

	public string Turno { get; set; } = "";

	public string Chfturno { get; set; } = "";

	public string Ousrinis { get; set; } = "";

	public DateTime? Ousrdata { get; set; } = DateTime.Now;

	public string Ousrhora { get; set; } = DateTime.Now.ToString("HH:mm:ss");

	public string Usrinis { get; set; } = "";

	public DateTime? Usrdata { get; set; } = DateTime.Now;

	public DateTime? Datasub { get; set; } = new DateTime(1900, 1, 1);

	public string Horasub { get; set; } = "";

	public string Usrhora { get; set; } = DateTime.Now.ToString("HH:mm:ss");

	public bool? Marcada { get; set; } = false;

	public decimal? Nragente { get; set; } = default(decimal);

	public string Codfer { get; set; } = "";

	public bool? Aprovado { get; set; } = false;

	public bool? Automatica { get; set; } = false;

	public bool? Verificar { get; set; } = false;

	public string PedidoId { get; set; } = "";

	public string Tipo { get; set; } = "";

	public string Estacao { get; set; } = "";

	public string Nomeaprovacao { get; set; }

	public DateTime Dataaprovacao { get; set; }

	public string Horaaprovacao { get; set; } = "";

	public string Patio { get; set; } = "";

	public string Tpforn { get; set; } = "";

	public string Loco { get; set; } = "";

	public string Codest { get; set; } = "";

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
