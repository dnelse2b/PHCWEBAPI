using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using SGOFWS.DTOs;

namespace SGOFWS.Helper;

public class HTTPWSHelper
{
	private readonly APIHelper apiHelper = new APIHelper();

	private readonly LogHelper logHelper = new LogHelper();

	public ResponseDTO ParseToDefaultResponse(string responseText)
	{
		try
		{
			return JsonConvert.DeserializeObject<ResponseDTO>(responseText);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public HttpWebRequest GetHttpWebRequestByEntityAndRoute(string entity, string route)
	{
		API apiData = apiHelper.getApiEntity(entity, route);
		if (apiData == null || apiData.status == null || apiData?.status == "0")
		{
			throw new Exception("API_DATA_NOT_FOUND");
		}
		Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == route).FirstOrDefault();
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint2.url?.Trim());
		httpWebRequest.ContentType = endpoint2.contentType;
		httpWebRequest.Method = endpoint2.method;
		return httpWebRequest;
	}
}
