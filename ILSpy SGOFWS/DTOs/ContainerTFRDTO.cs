using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ContainerTFRDTO
{
	public object customer_acc_no { get; set; }

	public object customer_name { get; set; }

	public object load_station_name { get; set; }

	public object load_sdg_name { get; set; }

	public object load_date { get; set; }

	public object load_time { get; set; }

	public object load_station_code { get; set; }

	public object load_sdg_code { get; set; }

	public object dest_station_name { get; set; }

	public object dest_sdg_name { get; set; }

	public object dest_sdg_code { get; set; }

	public object dest_station_code { get; set; }

	public object consignor_name { get; set; }

	public object consignor_no { get; set; }

	public object consignee_name { get; set; }

	public object consignee_no { get; set; }

	public object container_no { get; set; }

	public object container_mass { get; set; }

	public object sap_order_no { get; set; }

	public object commercial_comdty_desc { get; set; }

	public object commdty_code { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
