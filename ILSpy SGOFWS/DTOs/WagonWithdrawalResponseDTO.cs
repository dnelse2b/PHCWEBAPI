using System;
using System.Collections.Generic;

namespace SGOFWS.DTOs;

public class WagonWithdrawalResponseDTO
{
	public DateTime? SubmittedAt { get; set; } = new DateTime(1900, 1, 1);

	public string bolletin { get; set; } = "";

	public string mpdcRequestForWithdrawalNumber { get; set; }

	public List<WagonRequestResultDTO> wagons { get; set; }
}
