using System;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ConfiguracaoDossier
{
	public decimal ndos { get; set; }

	public string codigo { get; set; }

	public string nome { get; set; }

	public string customerName { get; set; }

	public decimal no { get; set; }

	public decimal boano { get; set; }

	public decimal obrano { get; set; }

	public DateTime data { get; set; }

	public DateTime dataobra { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
