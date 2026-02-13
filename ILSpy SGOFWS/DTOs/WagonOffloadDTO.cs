using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class WagonOffloadDTO
{
	public string mpdcSupplyRequestNumber { get; set; }

	public decimal totalSuppliedWagons { get; set; }

	public decimal cargoOffloadPercent { get; set; }

	public decimal totalOffloadedWagons { get; set; }

	public decimal wagonsToBeOffloaded { get; set; }

	public decimal wagonsOffloading { get; set; }

	public decimal avgWagonsOffloadPerMinute { get; set; }

	public decimal totalSuppliedCargo { get; set; }

	public decimal totalOffloadedCargo { get; set; }

	public decimal cargoToBeOffloaded { get; set; }

	public decimal avgCargoOffloadPerMinute { get; set; }

	public List<WagonOffloadingDetailDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
