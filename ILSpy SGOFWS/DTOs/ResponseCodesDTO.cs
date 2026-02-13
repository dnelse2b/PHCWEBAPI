using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ResponseCodesDTO
{
	public string cod { get; set; }

	public decimal? id { get; set; }

	public string codDesc { get; set; }

	public ResponseCodesDTO()
	{
	}

	public ResponseCodesDTO(string cod, string codDesc, decimal? id)
	{
		this.cod = cod;
		this.codDesc = codDesc;
		this.id = id;
	}

	public ResponseCodesDTO(string cod, string codDesc)
	{
		this.cod = cod;
		this.codDesc = codDesc;
	}

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
