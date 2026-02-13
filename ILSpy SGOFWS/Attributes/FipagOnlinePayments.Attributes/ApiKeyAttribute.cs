using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGOFWS.Domains.Contracts;
using SGOFWS.DTOs;

namespace SGOFWS.Attributes.FipagOnlinePayments.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter, IFilterMetadata
{
	private const string APIKEYNAME = "ApiKey";

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (string.IsNullOrEmpty(context.HttpContext.Request.Headers["requestId"]))
		{
			context.Result = new ContentResult
			{
				StatusCode = 401,
				Content = new ResponseDTO(WebTransactionCodes.MUSTPROVIDEREQUESTID, null, null).ToString()
			};
			return;
		}
		if (!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
		{
			context.Result = new ContentResult
			{
				StatusCode = 401,
				Content = new ResponseDTO(WebTransactionCodes.MUSTPROVIDEREQUESTID, null, null).ToString()
			};
			return;
		}
		IConfiguration appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
		if (!(from k in appSettings.GetSection("ApiKey").Get<IEnumerable<ApiKey>>()
			where k.key == extractedApiKey
			select k.key).Any())
		{
			context.Result = new ContentResult
			{
				StatusCode = 401,
				Content = "Api Key is not valid"
			};
		}
		else
		{
			await next();
		}
	}
}
