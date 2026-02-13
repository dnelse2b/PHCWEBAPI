using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.Persistence.DTOS;

public class UpdateStationDTO
{
	public string station { get; set; }

	public string stationCode { get; set; }

	public string trainNumber { get; set; }

	public string operationType { get; set; }

	public string operationDate { get; set; }

	public bool isBossa { get; set; }

	public string time { get; set; }

	public decimal order { get; set; }

	public string trainOrientation { get; set; }

	public string etaToNextStation { get; set; }

	public string nextStation { get; set; }

	public string etaToBossa { get; set; }

	public List<UpdateStationWagonDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
