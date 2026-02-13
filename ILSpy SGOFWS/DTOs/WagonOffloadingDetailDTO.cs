using System;

namespace SGOFWS.DTOs;

public class WagonOffloadingDetailDTO
{
	public string wagonNumber { get; set; }

	public string status { get; set; }

	public string consignmentNumber { get; set; }

	public decimal cargoOffloadPercent { get; set; }

	public string mpdcSupplyRequestNumber { get; set; }

	public DateTime? offloadStartedAt { get; set; }

	public DateTime? offloadFinishedAt { get; set; }

	public decimal weight { get; set; }

	public decimal offloadedWeight { get; set; }

	public decimal toBeOffloadedWeight { get; set; }

	public decimal offloadDuration { get; set; }

	public decimal avgCargoOffloadPerMinute { get; set; }

	public decimal totalTransactions { get; set; }
}
