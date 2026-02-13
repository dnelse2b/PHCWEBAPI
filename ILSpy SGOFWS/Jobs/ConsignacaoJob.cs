using System;
using System.Diagnostics;
using System.IO;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Persistence.APIs;
using SGOFWS.Persistence.APIs.MPDC;
using SGOFWS.Persistence.Contexts;
using SGOFWS.Persistence.Repositories;
using SGOFWS.Services;
using SGOFWS.Services.External;

namespace SGOFWS.Jobs;

public class ConsignacaoJob
{
	private readonly APIRouter apiRouter = new APIRouter();

	private readonly MPDCService mPDCService = new MPDCService();

	private readonly ConsignmentService consignmentService = new ConsignmentService();

	private readonly SyncService syncService = new SyncService();

	private TFRService TFRService = new TFRService();

	private readonly MPDCAPI mPDCAPI = new MPDCAPI();

	private SGOFCTX sgofctx = new SGOFCTX();

	private IConsignacaoRepository consignacaoRepository;

	public async void processarConsignacoes()
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		_ = DateTime.Now;
		await apiRouter.getListDataFromApi("TFR");
		consignacaoRepository = new ConsignacaoRepository(sgofctx);
		watch.Stop();
	}

	public void jobsHandler()
	{
		processarConsignacoes();
	}

	public void test()
	{
	}

	public void executeJobs()
	{
		//DbContextOptionsBuilder<SGOFCTX> optionsBuilder = new DbContextOptionsBuilder<SGOFCTX>();
		//IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
		//IConfigurationRoot config = configuration.Build();
		//string connString = config.GetConnectionString("DBconnect");
		//optionsBuilder.UseSqlServer(connString);
		RecurringJob.AddOrUpdate("notifyMPDCConsignmentsMine", () => mPDCService.NotifyMPDCMineDeparture(), Cron.MinuteInterval(40));
		RecurringJob.AddOrUpdate("ColocarEstacoesNaFila", () => consignmentService.ActualizacaoEstacoes(), Cron.MinuteInterval(10));
		//RecurringJob.AddOrUpdate("ActualizarEstacoesCFM", () => consignmentService.ActualizacaoEstacaoCFM(), Cron.MinuteInterval(15));
		RecurringJob.AddOrUpdate("ActualizarEstacoes", () => mPDCService.UpdateStation(), Cron.MinuteInterval(30));
		RecurringJob.AddOrUpdate("NotificarNovasConsignacoes", () => mPDCService.NotifyMPDCConsignments(), Cron.MinuteInterval(40));
		//RecurringJob.AddOrUpdate("SincronizarGuias", () => consignmentService.SincronzarGuias(), Cron.MinuteInterval(30));
		//RecurringJob.AddOrUpdate("SyncRecords", () => syncService.HandleSync(), Cron.MinuteInterval(1));
	}
}
