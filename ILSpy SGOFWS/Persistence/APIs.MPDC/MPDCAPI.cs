using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Persistence.APIs.MPDC.DTOS;

namespace SGOFWS.Persistence.APIs.MPDC;

public class MPDCAPI
{
	private readonly APIHelper apiHelper = new APIHelper();

	private readonly LogHelper logHelper = new LogHelper();

	private readonly HTTPWSHelper hTTPWSHelper = new HTTPWSHelper();

	public async Task<string> authenticate()
	{
		string requestId = KeysExtension.generateRequestId();
		object data = new object();
		try
		{
			API apiData = apiHelper.getApiEntity("MPDC", "AUTH");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("I-500", apiData.message), null, null);
				logHelper.generateLogJB(response, requestId, "MPDCAPI.authenticate");
				return "UNAUTHORIZED";
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "AUTH").FirstOrDefault();
			HttpWebRequest httpWebRequest = hTTPWSHelper.GetHttpWebRequestByEntityAndRoute("MPDC", "AUTH");
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				data = new
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
			return JsonConvert.DeserializeObject<MPDCAuthResponse>(result)?.token;
		}
		catch (WebException ex)
		{
			if (ex?.Response?.GetResponseStream() == null)
			{
				ResponseDTO responseex = new ResponseDTO(new ResponseCodesDTO("I-500", "GetResponseStreamSemResposta"), null, JsonConvert.SerializeObject(data));
				logHelper.generateLogJB(responseex, requestId, "MPDCAPI.authenticate");
				return "UNAUTHORIZED";
			}
			StreamReader reader = new StreamReader(ex.Response.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ResponseDTO responseex2 = new ResponseDTO(new ResponseCodesDTO("I-500", rawresp.ToString()), null, null);
			logHelper.generateLogJB(responseex2, requestId, "MPDCAPI.getAllConsignments");
			logHelper.generateLogJB(responseex2, requestId, "MPDCAPI.authenticate");
			return "UNAUTHORIZED";
		}
	}

	public async Task<ResponseDTO> NotifyConsignment(ConsignmentNotificationDTO consignmentNotificationDTO)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			HttpWebRequest httpWebRequest = hTTPWSHelper.GetHttpWebRequestByEntityAndRoute("MPDC", "NOTIFYCONSIGMENTS");
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(consignmentNotificationDTO)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			JsonConvert.DeserializeObject<NotifyConsignmentResponse>(result);
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(consignmentNotificationDTO));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(consignmentNotificationDTO));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.NotifyConsignmentResponse");
			return finalResponse;
		}
		catch (Exception ex)
		{
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};

			Debug.Print($"ERROR NOTIFY CONSIGNMENT {errorDTO.ToString()}");
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(consignmentNotificationDTO));
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.NotifyConsignmentResponse");
			return finalResponse2;
		}
	}

	public async Task<ResponseDTO> NotifyConsignmentDeparture(MineDepartureNotificationMPDCDTO mineDepartureNotificationMPDCDTO, string consgno)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			API apiData = apiHelper.getApiEntity("MPDC", "NOTIFYCONSIGMENTSDEPARTURE");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				throw new Exception("API_DATA_NOT_FOUND");
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "NOTIFYCONSIGMENTSDEPARTURE").FirstOrDefault();
			string urlToBeModified = endpoint2.url?.Trim();
			string finalUrl = urlToBeModified.Replace("CONSIGNMENTTOREPLACE$", consgno);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(finalUrl);
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(mineDepartureNotificationMPDCDTO)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(mineDepartureNotificationMPDCDTO));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(mineDepartureNotificationMPDCDTO));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.NotifyConsignmentDeparture");
			return finalResponse;
		}
		catch (WebException ex)
		{
			StreamReader reader = new StreamReader(ex?.Response?.GetResponseStream());
			reader.ReadToEnd();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(mineDepartureNotificationMPDCDTO));
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.NotifyConsignmentDeparture");
			return finalResponse2;
		}
	}

	public async Task<ResponseDTO> StartShunting(ShuntingDTO shuntingDTO)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			API apiData = apiHelper.getApiEntity("MPDC", "StartShunting");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				throw new Exception("API_DATA_NOT_FOUND");
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "StartShunting").FirstOrDefault();
			string urlToBeModified = endpoint2.url?.Trim();
			string finalUrl = urlToBeModified.Replace("REQUESTREPLACE$", shuntingDTO.wagonsRequestId);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(finalUrl);
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(shuntingDTO)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(shuntingDTO));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(shuntingDTO));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.StartShunting");
			return finalResponse;
		}
		catch (WebException ex)
		{
			StreamReader reader = new StreamReader(ex?.Response?.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message + " " + rawresp,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(shuntingDTO));
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.StartShunting");
			return finalResponse2;
		}
	}

	public async Task<ResponseDTO> CloseShunting(ShuntingDTO shuntingDTO)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			API apiData = apiHelper.getApiEntity("MPDC", "CloseShunting");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				throw new Exception("API_DATA_NOT_FOUND");
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "CloseShunting").FirstOrDefault();
			string urlToBeModified = endpoint2.url?.Trim();
			string finalUrl = urlToBeModified.Replace("REQUESTREPLACE$", shuntingDTO.wagonsRequestId);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(finalUrl);
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(shuntingDTO)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(shuntingDTO));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(shuntingDTO));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.CloseShunting");
			return finalResponse;
		}
		catch (WebException ex)
		{
			StreamReader reader = new StreamReader(ex?.Response?.GetResponseStream());
			reader.ReadToEnd();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(shuntingDTO));
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.CloseShunting");
			return finalResponse2;
		}
	}

	public async Task<ResponseDTO> ApproveWagonsSupply(WagonAproveDTO wagonSupplyApprove)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			API apiData = apiHelper.getApiEntity("MPDC", "ApproveWagonOperation");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				throw new Exception("API_DATA_NOT_FOUND");
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "ApproveWagonOperation").FirstOrDefault();
			string urlToBeModified = endpoint2.url?.Trim();
			string finalUrl = urlToBeModified.Replace("REQUESTREPLACE$", wagonSupplyApprove.mpdcWagonsRequisitionNumber);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(finalUrl);
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(wagonSupplyApprove)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(wagonSupplyApprove));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(wagonSupplyApprove));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.ApproveWagonsSupply");
			return finalResponse;
		}
		catch (WebException ex)
		{
			StreamReader reader = new StreamReader(ex?.Response?.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message + " " + rawresp,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(wagonSupplyApprove));
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.ApproveWagonsSupply");
			return finalResponse2;
		}
	}

	public async Task<ResponseDTO> ApproveTopUp(TopUpApprovalDTO topUpApproval, string requestNumber)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			API apiData = apiHelper.getApiEntity("MPDC", "NotifyTopUp");
			if (apiData == null || apiData.status == null || apiData?.status == "0")
			{
				throw new Exception("API_DATA_NOT_FOUND");
			}
			Endpoint endpoint2 = apiData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == "NotifyTopUp").FirstOrDefault();
			string urlToBeModified = endpoint2.url?.Trim();
			string finalUrl = urlToBeModified.Replace("REQUESTREPLACE$", requestNumber);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(finalUrl);
			httpWebRequest.ContentType = endpoint2.contentType;
			httpWebRequest.Method = endpoint2.method;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(topUpApproval)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(topUpApproval));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(topUpApproval));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.ApproveTopUp");
			return finalResponse;
		}
		catch (WebException ex)
		{
			StreamReader reader = new StreamReader(ex?.Response?.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message + " " + rawresp,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), topUpApproval.ToString());
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.ApproveTopUp");
			return finalResponse2;
		}
		catch (Exception ex2)
		{
			ErrorDTO errorDTO2 = new ErrorDTO
			{
				message = ex2?.Message + " ",
				stack = ex2?.StackTrace?.ToString(),
				inner = ex2?.InnerException?.ToString()
			};
			ResponseDTO finalResponse3 = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO2.ToString(), topUpApproval.ToString());
			logHelper.generateLogJB(finalResponse3, requestId, "MPDCAPI.ApproveTopUp");
			return finalResponse3;
		}
	}

	public async Task<ResponseDTO> UpdateStation(UpdateStationRequest updateStation)
	{
		string requestId = KeysExtension.generateRequestId();
		try
		{
			string authResult = await authenticate();
			HttpWebRequest httpWebRequest = hTTPWSHelper.GetHttpWebRequestByEntityAndRoute("MPDC", "UPDATESTATION");
			httpWebRequest.Headers.Add("Authorization", "Bearer " + authResult);
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(await Task.Run(() => JsonConvert.SerializeObject(updateStation)));
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
			string result = streamReader.ReadToEnd();
			int statuscode = (int)httpResponse.StatusCode;
			using (new StreamReader(httpResponse.GetResponseStream()))
			{
			}
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(updateStation));
			if (statuscode != 200)
			{
				finalResponse = new ResponseDTO(new ResponseCodesDTO("0006", "Error", logHelper.generateResponseID()), result, JsonConvert.SerializeObject(updateStation));
			}
			logHelper.generateLogJB(finalResponse, requestId, "MPDCAPI.UpdateStation");
			return finalResponse;
		}
		catch (WebException ex)
		{
			int? statusCode = null;
			string responseCode = "0007";
			if (ex.Response is HttpWebResponse response)
			{
				statusCode = (int)response.StatusCode;
			}
			if (statusCode.HasValue)
			{
				responseCode = ((statusCode.Value >= 500 && statusCode.Value <= 599) ? "I-500" : "0007");
			}
			StreamReader reader = new StreamReader(ex?.Response?.GetResponseStream());
			string rawresp = reader.ReadToEnd();
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString() + "  " + rawresp
			};
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO(responseCode, errorDTO.ToString(), logHelper.generateResponseID()), errorDTO.ToString(), JsonConvert.SerializeObject(updateStation));
			logHelper.generateLogJB(finalResponse2, requestId, "MPDCAPI.UpdateStation");
			return finalResponse2;
		}
	}
}
