using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class UpdateStationRequest
{
	public string station { get; set; }

	public string stationCode { get; set; }

	public string destination { get; set; } = "TCM";

	public string trainNumber { get; set; }

	public string trainReference { get; set; }

	public string operationType { get; set; }

	public string operationDate { get; set; }

	public string time { get; set; }

	public string trainOrientation { get; set; }

	public string etaToNextStation { get; set; }

	public string nextStation { get; set; }

	public string nextStationCode { get; set; }

	public string etaToLastStation { get; set; }

	public List<UpdateStationRequestWagon> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
