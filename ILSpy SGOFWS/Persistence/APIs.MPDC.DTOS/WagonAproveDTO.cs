using Newtonsoft.Json;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class WagonAproveDTO
{
	public string mpdcWagonsRequisitionNumber { get; set; }

	public string approvedAt { get; set; }

	public string approvedWagonActionDate { get; set; }

	public string approvedDate { get; set; }

	public string approvalRemarks { get; set; }

	public string status { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
