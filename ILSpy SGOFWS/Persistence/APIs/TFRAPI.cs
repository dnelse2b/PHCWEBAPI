using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;

namespace SGOFWS.Persistence.APIs;

public class TFRAPI
{
	private readonly APIHelper apiHelper = new APIHelper();

	private readonly LogHelper logHelper = new LogHelper();

	public async Task<string> authenticate()
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			API apiData = apiHelper.getApiEntity("TFR", "AUTH");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("I-500", apiData.message), null, null);
				logHelper.generateLogJB(response, requestId, "TFRAPI.authenticate");
				return "UNAUTHORIZED";
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "AUTH").FirstOrDefault();
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint2.url?.Trim());
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				var data = new
				{
					username = endpoint2.credentials.username.ToString().Trim(),
					password = endpoint2.credentials.password.ToString().Trim()
				};
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(data)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			httpResponse.StatusCode.ToString();
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			return JsonConvert.DeserializeObject<TFRAuthResponseDTO>(result)?.token;
		}
		catch (WebException ex)
		{
			if (ex?.Response?.GetResponseStream() == null)
			{
				ResponseDTO responseex = new ResponseDTO(new ResponseCodesDTO("I-500", "GetResponseStreamSemResposta"), null, null);
				logHelper.generateLogJB(responseex, requestId, "TFRAPI.authenticate");
				return "UNAUTHORIZED";
			}
			StreamReader reader = new StreamReader(ex.Response.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ResponseDTO responseex2 = new ResponseDTO(new ResponseCodesDTO("I-500", rawresp.ToString()), null, null);
			logHelper.generateLogJB(responseex2, requestId, "TFRAPI.getAllConsignments");
			logHelper.generateLogJB(responseex2, requestId, "TFRAPI.authenticate");
			return "UNAUTHORIZED";
		}
	}

	public async Task<List<object>> getAllConsignments()
	{
		string requestId = KeysExtension.generateRequestId();
		List<object> consignacoes = new List<object>();
		try
		{
			API apiData = apiHelper.getApiEntity("TFR", "AUTH");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("I-500", apiData.message), null, null);
				logHelper.generateLogJB(response, requestId, "TFRAPI.getAllConsignments");
				return consignacoes;
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "GET_ALL_CONSIGNMENTS").FirstOrDefault();
			string authorization = await authenticate();
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint2.url?.Trim());
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authorization);
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			if (streamReader == null)
			{
				return null;
			}
			string result = streamReader?.ReadToEnd();
			httpResponse.StatusCode.ToString();
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			consignacoes = JsonConvert.DeserializeObject<List<object>>(result);
			return consignacoes;
		}
		catch (WebException ex)
		{
			if (ex?.Response?.GetResponseStream() == null)
			{
				ResponseDTO responseex = new ResponseDTO(new ResponseCodesDTO("I-500", "GetResponseStreamSemResposta"), null, null);
				logHelper.generateLogJB(responseex, requestId, "TFRAPI.getAllConsignments");
				return consignacoes;
			}
			StreamReader reader = new StreamReader(ex.Response.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ResponseDTO responseex2 = new ResponseDTO(new ResponseCodesDTO("I-500", rawresp.ToString()), null, null);
			logHelper.generateLogJB(responseex2, requestId, "TFRAPI.getAllConsignments");
			return consignacoes;
		}
	}
}
