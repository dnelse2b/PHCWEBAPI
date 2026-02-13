using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class NotifyConsignmentResponse
{
	public decimal? id { get; set; }

	public string consignmentNumber { get; set; }

	public string eta { get; set; }

	public string arrivalDate { get; set; }

	public string departureDate { get; set; }

	public string loadingDate { get; set; }

	public string loadingSite { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
