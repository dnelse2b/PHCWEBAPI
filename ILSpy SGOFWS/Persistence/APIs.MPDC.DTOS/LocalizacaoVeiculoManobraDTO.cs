using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class LocalizacaoVeiculoManobraDTO
{
	public string codori { get; set; }

	public string origem { get; set; }

	public string coddest { get; set; }

	public string codpatio { get; set; }

	public string destino { get; set; }

	public string estacao { get; set; }

	public string codest { get; set; }

	public string patio { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
