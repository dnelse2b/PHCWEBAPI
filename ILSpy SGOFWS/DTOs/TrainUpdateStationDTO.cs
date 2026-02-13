using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGOFWS.DTOs;

public class TrainUpdateStationDTO
{
	[Required]
	public string tail { get; set; }

	[Required]
	public string station { get; set; }

	[Required]
	public string date { get; set; }

	[Required]
	public string entity { get; set; }

	[Required]
	public string train { get; set; }

	public string telemeter { get; set; } = "";

	[Required]
	public string operation { get; set; }

	public List<TrainUpdateStationWagonsDTO> wagons { get; set; }
}
