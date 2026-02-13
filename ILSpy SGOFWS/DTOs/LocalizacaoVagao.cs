using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class LocalizacaoVagao
{
	public string wagonNumber { get; set; }

	public string consignmentNumber { get; set; }

	public string wagonStatus { get; set; }

	public string cargo { get; set; } = "";

	public decimal? weight { get; set; }

	public bool? detached { get; set; } = false;

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
