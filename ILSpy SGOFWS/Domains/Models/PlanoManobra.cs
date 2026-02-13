using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.Domains.Models;

public class PlanoManobra
{
	public CabecalhoPlanoManobra cabecalhoPlanoManobra { get; set; }

	public List<LinhasPlanoManobra> linhasPlanoManobras { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
