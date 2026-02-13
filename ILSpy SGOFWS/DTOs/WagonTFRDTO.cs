using System.Collections.Generic;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class WagonTFRDTO
{
	public object load_date { get; set; }

	public object load_time { get; set; }

	public object wagon_no { get; set; }

	public object wagon_owner { get; set; }

	public object wagon_type { get; set; }

	public object wagon_desc { get; set; }

	public object wagon_code { get; set; }

	public object dual_contract { get; set; }

	public object contents_code { get; set; }

	public object contents_desc { get; set; }

	public object commercial_comdty_desc { get; set; }

	public object load_empty_ind { get; set; }

	public object wagon_mass { get; set; }

	public object wagon_vol { get; set; }

	public object tarpaulin_no { get; set; }

	public object tarpaulin_adm { get; set; }

	public object train_no { get; set; }

	public object current_station_code { get; set; }

	public object current_station_name { get; set; }

	public object last_reported_date_time { get; set; }

	public object enroute_status_to_ktr { get; set; }

	public object eta_to_ktr { get; set; }

	public object last_update_timestamp { get; set; }

	public object container_count { get; set; }

	public List<ContainerTFRDTO> containers { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
