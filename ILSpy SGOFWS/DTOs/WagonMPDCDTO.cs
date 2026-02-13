using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class WagonMPDCDTO
{
	[Required]
	public string wagonNumber { get; set; }

	[RegularExpression("^(empty|loaded)$", ErrorMessage = "The wagon status must be either 'empty' or 'loaded'.")]
	public string wagonStatus { get; set; }

	public string commodityCategory { get; set; }

	public decimal? weight { get; set; }

	public string commodity { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
