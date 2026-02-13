using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SGOFWS.DTOs;
using SGOFWS.Helper;

namespace SGOFWS.MiddleWares;

public class GlobalResponses : IAsyncResultFilter, IFilterMetadata
{
	private readonly LogHelper logHelper = new LogHelper();

	public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		try
		{
			if (((context.Result is ObjectResult result) ? result.Value : null) is ResponseDTO)
			{
				_ = context.HttpContext.Request.Headers["requestId"];
				_ = context.HttpContext.Request.Path;
				ResponseDTO resp2 = new ResponseDTO(new ResponseCodesDTO("00888", "Test", 1222), null, null);
				JsonSerializer.Serialize(resp2);
			}
		}
		catch (Exception ex)
		{
			_ = ex;
			await next();
		}
		await next();
	}
}
