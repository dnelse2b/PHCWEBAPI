using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class ResponseDTO
{
	public ResponseCodesDTO response { get; set; }

	public object? Data { get; set; }

	public object? Content { get; set; }

	public ResponseDTO(ResponseCodesDTO response, object? data, object? content)
	{
		this.response = response;
		Data = data;
		Content = content;
	}

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
