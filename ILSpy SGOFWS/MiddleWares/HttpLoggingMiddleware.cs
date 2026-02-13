using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Request.Body.Peeker;
using SGOFWS.DTOs;
using SGOFWS.Helper;

namespace SGOFWS.MiddleWares;

public class HttpLoggingMiddleware
{
	private readonly ILogger _logger;

	private readonly RequestDelegate _next;

	private readonly HTTPWSHelper parseToDefaultResponse = new HTTPWSHelper();

	private readonly LogHelper logHelper = new LogHelper();

	public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		string userIpAddress = context.Connection.RemoteIpAddress?.ToString();
		try
		{
			MemoryStream responseBody = null;
			Stream originalResponseBody = null;
			try
			{
				string content = await context.Request.PeekBodyAsync();
				originalResponseBody = context.Response.Body;
				responseBody = new MemoryStream();
				context.Response.Body = responseBody;
				await _next(context);
				responseBody.Seek(0L, SeekOrigin.Begin);
				string responseText = await new StreamReader(responseBody).ReadToEndAsync();
				ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", "Success....", logHelper.generateResponseID()), null, content);
				ResponseDTO tmpResponse = parseToDefaultResponse.ParseToDefaultResponse(responseText);
				if (tmpResponse != null)
				{
					finalResponse = tmpResponse;
					finalResponse.Content = content;
				}
				string controllerName = context.GetRouteData().Values["controller"]?.ToString();
				string operationName = context.GetRouteData().Values["action"]?.ToString();
				logHelper.generateResponseLog(finalResponse, finalResponse?.response?.id.ToString(), controllerName + "/" + operationName, responseText, userIpAddress);
				finalResponse.Content = null;
				if (tmpResponse?.response != null)
				{
					string responseJson = JsonConvert.SerializeObject(finalResponse);
					byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
					await originalResponseBody.WriteAsync(responseBytes, 0, responseBytes.Length);
				}
				else
				{
					byte[] responseBytes2 = Encoding.UTF8.GetBytes(responseText);
					await originalResponseBody.WriteAsync(responseBytes2, 0, responseBytes2.Length);
				}
			}
			finally
			{
				if (responseBody != null)
				{
					context.Response.Body = originalResponseBody;
				}
				responseBody?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			string content = "";
			try
			{
				content = await context.Request.PeekBodyAsync();
			}
			catch (Exception)
			{
			}
			string controllerName2 = context.GetRouteData().Values["controller"]?.ToString();
			string operationName2 = context.GetRouteData().Values["action"]?.ToString();
			ResponseDTO finalResponse2 = new ResponseDTO(new ResponseCodesDTO("0007", "Exp", 1222), null, content);
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex2?.Message,
				stack = ex2?.StackTrace?.ToString(),
				inner = ex2?.InnerException?.ToString()
			};
			logHelper.generateResponseLog(finalResponse2, finalResponse2?.response?.id.ToString(), controllerName2 + "/" + operationName2, errorDTO.ToString(), userIpAddress);
			string errorResponseJson = JsonConvert.SerializeObject(finalResponse2);
			context.Response.StatusCode = 500;
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(errorResponseJson);
		}
	}
}
