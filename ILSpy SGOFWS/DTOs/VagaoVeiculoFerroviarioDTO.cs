using Newtonsoft.Json;
using SGOFWS.Domains.Models;

namespace SGOFWS.DTOs;

public class VagaoVeiculoFerroviarioDTO
{
	public EntidadeVagao entidadeVagao { get; set; }

	public VeiculoFerroviario veiculoFerroviario { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
