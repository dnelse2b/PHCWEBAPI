using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ConsignacaoTFRDTO
{
	public object consignment_no { get; set; }

	public object consignment_type { get; set; }

	public object customer_acc_no { get; set; }

	public object customer_name { get; set; }

	public object load_station_name { get; set; }

	public object load_sdg_name { get; set; }

	public object load_station_code { get; set; }

	public object load_sdg_code { get; set; }

	public object dest_station_name { get; set; }

	public object dest_sdg_name { get; set; }

	public object dest_sdg_code { get; set; }

	public object stakeholder_name { get; set; }

	public object stakeholder_no { get; set; }

	public object dest_station_code { get; set; }

	public object consignor_name { get; set; }

	public object consignor_no { get; set; }

	public object consignee_name { get; set; }

	public object consignee_no { get; set; }

	public object wagon_count { get; set; }

	public List<WagonTFRDTO> wagons { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
