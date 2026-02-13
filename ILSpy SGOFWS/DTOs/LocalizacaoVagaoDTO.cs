using System;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class LocalizacaoVagaoDTO
{
	public string vagao { get; set; }

	public string comboio { get; set; }

	public string consgno { get; set; }

	public string codest { get; set; }

	public string estacao { get; set; }

	public string tipoOp { get; set; }

	public decimal ordem { get; set; }

	public string proximaEstacao { get; set; }

	public string estado { get; set; }

	public string etaProximaEstacao { get; set; }

	public DateTime dataOperacao { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
