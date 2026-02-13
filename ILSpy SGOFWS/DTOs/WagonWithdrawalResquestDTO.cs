using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SGOFWS.DTOs;

public class WagonWithdrawalResquestDTO
{
	[Required]
	public string station { get; set; }

	public string stationCode { get; set; } = "";

	[Required]
	public DateTime? expectedWithdrawalDate { get; set; }

	[Required]
	public DateTime firstSubmissionTentative { get; set; }

	[Required]
	public string requestNumber { get; set; }

	[Required]
	public string mpdcRequestForWithdrawalNumber { get; set; }

	[Required]
	public bool requireApproval { get; set; } = true;

	public DateTime SubmittedAt { get; set; } = new DateTime(1900, 1, 1);

	public string Destination { get; set; } = "";

	public bool isTopUp { get; set; }

	public List<WagonRequestDTO> wagons { get; set; }
}
