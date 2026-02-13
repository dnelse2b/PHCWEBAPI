using System;
using System.IO;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Helper;

public class LogHelper
{
	public void generateLog(ResponseDTO response, string requestId, string operation)
	{
		try
		{
			BackgroundJob.Enqueue(() => generateLogJB(response, requestId, operation));
		}
		catch (Exception)
		{
		}
	}

	public decimal? generateResponseID()
	{
		return decimal.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("D16"));
	}

	public void generateLogJB(ResponseDTO response, string requestId, string operation)
	{
		DbContextOptionsBuilder<SGOFCTX> optionsBuilder = new DbContextOptionsBuilder<SGOFCTX>();
		IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
		IConfigurationRoot config = configuration.Build();
		string connString = config.GetConnectionString("DBconnect");
		optionsBuilder.UseSqlServer(connString);
		using SGOFCTX context = new SGOFCTX(optionsBuilder.Options);
		ULogs Ulogs = new ULogs
		{
			Code = response?.response?.cod,
			RequestId = requestId,
			ResponseDesc = response?.response.codDesc,
			Data = DateTime.Now,
			ResponseText = response?.Data?.ToString(),
			Content = response?.Content?.ToString(),
			Operation = operation
		};
		context.ULogs.Add(Ulogs);
		context.SaveChanges();
	}

	public void generateResponseLog(ResponseDTO response, string requestId, string operation, string responseText, string ip)
	{
		try
		{
			BackgroundJob.Enqueue(() => generateResponseLogJB(response, requestId, operation, responseText, ip));
		}
		catch (Exception)
		{
		}
	}

	public void generateResponseLogJB(ResponseDTO response, string requestId, string operation, string responseText, string ip)
	{
		DbContextOptionsBuilder<SGOFCTX> optionsBuilder = new DbContextOptionsBuilder<SGOFCTX>();
		IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
		IConfigurationRoot config = configuration.Build();
		string connString = config.GetConnectionString("DBconnect");
		optionsBuilder.UseSqlServer(connString);
		using SGOFCTX context = new SGOFCTX(optionsBuilder.Options);
		ULogs Ulogs = new ULogs
		{
			Code = response?.response?.cod,
			RequestId = requestId,
			ResponseDesc = response?.response?.codDesc,
			Data = DateTime.Now,
			Content = response?.Content?.ToString(),
			Operation = operation,
			ResponseText = responseText,
			Ip = ip
		};
		context.ULogs.Add(Ulogs);
		context.SaveChanges();
	}
}
