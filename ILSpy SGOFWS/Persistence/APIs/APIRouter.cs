using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGOFWS.Persistence.APIs;

public class APIRouter
{
	private readonly TFRAPI TFRAPI = new TFRAPI();

	public async Task<List<object>> getListDataFromApi(string admin)
	{
		if (admin == "TFR")
		{
			return await TFRAPI.getAllConsignments();
		}
		throw new Exception("a Admnistração para obtenção dos dados Da API não foi encontrada");
	}
}
