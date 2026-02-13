using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OPPWS.Extensions;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Mappers;
using SGOFWS.Persistence.APIs;
using SGOFWS.Persistence.APIs.MPDC;
using SGOFWS.Persistence.DTOS;
using SGOFWS.Templates;

namespace SGOFWS.Services.External;

public class TFRService : ITFRService
{
	private readonly IConsignacaoRepository _consignacaoRepository;

	private readonly RouterMapper routerMapper = new RouterMapper();

	private readonly IConfiguration _configuration;

	private readonly LogHelper logHelper = new LogHelper();

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	private readonly ILogger<TFRService> _logger;

	private readonly EmailHelper emailHelper = new EmailHelper();

	private readonly StringTemplates stringTemplates = new StringTemplates();

	private readonly MPDCMapper mpdcMapper = new MPDCMapper();

	private readonly IGenericRepository _genericRepository;

	private readonly IConsignmentService _consignmentService;

	private readonly MPDCAPI mPDCAPI = new MPDCAPI();

	private readonly APIRouter apiRouter = new APIRouter();

	private readonly LocationHelper locationHelper = new LocationHelper();

	public TFRService(IConsignacaoRepository consignacaoRepository, IGenericRepository genericRepository, IConsignmentService consignmentService)
	{
		_consignacaoRepository = consignacaoRepository;
		_genericRepository = genericRepository;
		_consignmentService = consignmentService;
	}

	public TFRService()
	{
	}

	public void deleteTest()
	{
		_consignacaoRepository.testCons();
	}

	public async Task TFRConsignmentsHandler()
	{
		Stopwatch watch = new Stopwatch();
		watch.Start();
		_ = DateTime.Now;
		BulkConsignmentHandler("TFR", await apiRouter.getListDataFromApi("TFR"), recovery: false);
		watch.Stop();
	}

	public void gravarLogDeDadosComProblemas(List<ResponseDTO> dadosComProblemas)
	{
		try
		{
			string requestId = KeysExtension.generateRequestId();
			if (dadosComProblemas.Count() <= 0)
			{
				return;
			}
			foreach (ResponseDTO dadoComProblema in dadosComProblemas)
			{
				logHelper.generateLogJB(dadoComProblema, requestId, "TFRService.ProcessarConsignacao");
			}
		}
		catch (Exception)
		{
		}
	}

	public void trackingReport(List<Dossier> novasConsignacoes, List<VagaoDTO> vagoesActualizados)
	{
		int totalNovasConsignacoes = novasConsignacoes.Count();
		int totalVagoesActualizados = vagoesActualizados.Count();
		string strdate = DateTime.Now.ToString("yyyy-MM-dd");
		DateTime dataTrans = default(DateTime);
		StringBuilder template = stringTemplates.getTrackingReportTemplate();
		string to = "SGOF.API@CFM.CO.MZ";
		TrackingReport templateData = new TrackingReport
		{
			data = DateTime.Now,
			novasConsignacoes = novasConsignacoes,
			vagoesActualizados = vagoesActualizados,
			totalNovasConsignacoes = totalNovasConsignacoes,
			totalVagoesActualizados = totalVagoesActualizados
		};
		try
		{
			emailHelper.sendEmailUsingTemplateAsync(template, "NOREPLY", "SGOF.API@CFM.CO.MZ", to, "Tracking report", templateData);
		}
		catch (Exception)
		{
		}
	}

	public void recoveryConsignacoes()
	{
		Dossier consignacao = _consignacaoRepository.getConsignacao("8097543242");
		if (consignacao != null)
		{
			string consignacaoStr = "[" + consignacao.bo3.UObs + "]";
			List<object> consignacoes = JsonConvert.DeserializeObject<List<object>>(consignacaoStr);
			BulkConsignmentHandler("TFR", consignacoes, recovery: true);
		}
	}

	public ResponseDTO BulkConsignmentHandler(string admin, List<object> data, bool recovery)
	{
		List<ResponseDTO> dadosComProblemas = new List<ResponseDTO>();
		List<Dossier> consignacoesProcessadas = new List<Dossier>();
		List<Dossier> novasConsignacoes = new List<Dossier>();
		List<VagaoDTO> vagoesActualizados = new List<VagaoDTO>();
		ConfiguracaoDossier configuracaoDossier = configuracaoHelper.getConfiguracaoDossier("CONSIGNACAO");
		decimal ndos = configuracaoDossier.ndos;
		string nmdos = "Consignação";
		string clnome = configuracaoDossier.customerName;
		decimal no = configuracaoDossier.no;
		decimal? obrano = _consignacaoRepository.getMaxObrano(ndos);
		foreach (object item in data)
		{
			try
			{
				Dossier dossier = routerMapper.mapData(admin, item);
				dossier.bo3.UObs = item.ToString();
				dossier.bo.Ndos = ndos;
				dossier.bo.Obrano = obrano;
				dossier.bo.Nmdos = nmdos;
				dossier.bo.Nome = clnome;
				dossier.bo.No = no;
				dossier.bo3.UAdmin = admin;
				Dossier consignacaoResult = processarConsignacao(dossier, admin, novasConsignacoes, vagoesActualizados, recovery);
				decimal? responseId = logHelper.generateResponseID();
				UHistcons historicoConsignacao = new UHistcons
				{
					UHistconsstamp = 25.useThisSizeForStamp(recovery),
					Content = item?.ToString(),
					Data = DateTime.Now,
					Provider = admin,
					Admin = admin,
					Consgno = consignacaoResult?.bo3?.UConsgno,
					Response = consignacaoResult?.processamentoResponse?.ToString()
				};
				historicoConsignacao.AssignDefaultValues();
				_genericRepository.Add(historicoConsignacao);
				_genericRepository.SaveChanges();
				if (consignacaoResult?.processamentoResponse?.cod != "0000")
				{
					dadosComProblemas.Add(new ResponseDTO(new ResponseCodesDTO(consignacaoResult?.processamentoResponse?.cod, consignacaoResult?.processamentoResponse?.codDesc), item, item));
					continue;
				}
				if (consignacaoResult.novo == true)
				{
					obrano += 1m;
				}
				consignacoesProcessadas.Add(consignacaoResult);
			}
			catch (Exception ex)
			{
				dadosComProblemas.Add(new ResponseDTO(new ResponseCodesDTO("0007", "Excep Handle Consignação " + ex.Message.ToString() + ex.Message.ToString()), ex.Message.ToString(), item));
			}
		}
		if (novasConsignacoes.Count() > 0 || vagoesActualizados.Count() > 0)
		{
			trackingReport(novasConsignacoes, vagoesActualizados);
		}
		gravarLogDeDadosComProblemas(dadosComProblemas);
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success"), consignacoesProcessadas, null);
	}

	public Dossier processarConsignacao(Dossier dossier, string admin, List<Dossier> novasConsignacoes, List<VagaoDTO> vagoesActualizados, bool recovery)
	{
		decimal? requestID = logHelper.generateResponseID();
		try
		{
			Dossier consignacao = _consignacaoRepository.getConsignacao(dossier?.bo3?.UConsgno);
			if (consignacao == null)
			{
				TrackConsignmentDTO trackConsignmentData = locationHelper.GetTrackConsignment(admin, dossier?.bo3?.UDesvdest?.Trim(), "LRG");
				novasConsignacoes.Add(dossier);
				dossier.novo = true;
				dossier.bo3.UStrackin = trackConsignmentData.trackIn;
				dossier.bo3.UFccodstd = trackConsignmentData.destinationCode;
				dossier.bo3.UFcstdest = trackConsignmentData.destination;
				_consignacaoRepository.criarConsignacao(dossier);
				dossier.processamentoResponse = new ResponseCodesDTO("0000", "Success");
				return dossier;
			}
			dossier.novo = false;
			if (!recovery)
			{
				_consignacaoRepository.actualizarHistoricoConsignacao(dossier.bo3.UObs, consignacao?.bo3?.Bo3stamp);
			}
			dossier.bo.Bostamp = consignacao?.bo3?.Bo3stamp;
			if (dossier.bo3.UConsgno.Substring(0, 1) == "E" && admin == "TFR")
			{
				dossier.bo = consignacao.bo;
				_consignacaoRepository.overrideVagoes(dossier);
				dossier.processamentoResponse = new ResponseCodesDTO("0000", "Success");
				return dossier;
			}
			List<Bi2> vagoes = dossier.bi2;
			List<UStqueue> stQueues = new List<UStqueue>();
			foreach (Bi2 vagao in vagoes)
			{
				vagao.Bostamp = consignacao?.bo3?.Bo3stamp;
				Bi2 vagaoAlterado = _consignacaoRepository.getVagaoActualizado(consignacao?.bo3.Bo3stamp, vagao.UVagno, vagao.UUltdtrep, vagao.UStatus);
				if (vagaoAlterado != null)
				{
					VagaoDTO vagaoActualizado = new VagaoDTO
					{
						UVagcod = vagao?.UVagcod,
						UAdmintr = vagao?.UAdmintr,
						UEta = vagao?.UEta,
						UPeso = vagao.UPeso,
						UStatus = vagao?.UStatus,
						UUltdtrep = vagao?.UUltdtrep,
						UUltmtmst = vagao?.UUltmtmst,
						UVagdesc = vagao?.UVagdesc,
						UVagno = vagao?.UVagno,
						UVagtip = vagao?.UVagtip,
						UVagvol = vagao.UVagvol,
						Consgno = consignacao?.bo3?.UConsgno
					};
					if (vagao.UStatus.Contains("Arrived"))
					{
						vagao.UChegfrt = true;
						vagoesActualizados.Add(vagaoActualizado);
						try
						{
							_consignmentService.ActualizarTrajecto(new UTrajcons
							{
								Vagno = vagao?.UVagno,
								Consgno = consignacao?.bo3?.UConsgno,
								Codest = "LRE07",
								Estacao = "Estação Komatipoort",
								Codprxmst = "",
								Refcomboio = "",
								Etaproximast = new DateTime(1900, 1, 1),
								Proximast = "",
								Dataop = DateTime.Parse(vagao?.UUltdtrep),
								Tipoop = "Arrival",
								Sentido = "Descending",
								Tempostamp = ""
							});
							DateTime etaNextStation = DateTime.Parse(vagao?.UUltdtrep).AddMinutes(36.0);
							DateTime etaDestination = DateTime.Parse(vagao?.UUltdtrep).AddMinutes(360.0);
							try
							{
								List<UpdateStationWagonDTO> updateRequestWagons = new List<UpdateStationWagonDTO>();
								updateRequestWagons.Add(new UpdateStationWagonDTO
								{
									wagonNumber = vagao?.UVagno,
									wagonStatus = "loaded",
									consignmentNumber = dossier?.bo3?.UConsgno,
									weight = vagao?.UPeso,
									detached = false
								});
								UStqueue stationQueue = new UStqueue
								{
									UStqueuestamp = 25.UseThisSizeForStamp(),
									Stationcode = "LRE07",
									Station = "Estação Komatipoort",
									Ord = 0m,
									Etanextst = etaNextStation,
									Operationdate = DateTime.Parse(vagao?.UUltdtrep),
									Nextstation = "Estação de Ressano Garcia",
									Nextstcode = "LRE06",
									Etadestination = etaDestination,
									Trainnumber = vagao?.UComboio,
									Trainreference = vagao?.UComboio,
									Tempostamp = "",
									Trainorientation = "Descending",
									Operationtype = "Arrival",
									Wagonsdata = JsonConvert.SerializeObject(updateRequestWagons, Formatting.Indented),
									Entity = "MPDC"
								};
								stQueues.Add(stationQueue);
							}
							catch (Exception ex)
							{
								ErrorDTO errorDTO = new ErrorDTO
								{
									message = ex?.Message,
									stack = ex?.StackTrace?.ToString(),
									inner = ex?.InnerException?.ToString()
								};
								ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0007", errorDTO.ToString(), logHelper.generateResponseID()), errorDTO.ToString(), null);
								logHelper.generateLogJB(finalResponse, requestID.ToString(), "TFRSERVICE.NOTIFYWAGONARRIVALSCOPE");
							}
						}
						catch (Exception)
						{
						}
					}
					_consignacaoRepository.actualizarVagao(vagao);
				}
				_consignacaoRepository.inserirVagaoSeNaoExiste(vagao, consignacao?.bo3?.Bo3stamp, consignacao);
			}
			_consignacaoRepository.actualizarTotalVeiculos(dossier.bo3.UTotveicl, consignacao?.bo3.Bo3stamp);
			_genericRepository.BulkAdd(stQueues);
			_genericRepository.SaveChanges();
			dossier.processamentoResponse = new ResponseCodesDTO("0000", "Success");
			return dossier;
		}
		catch (Exception ex3)
		{
			return new Dossier
			{
				processamentoResponse = new ResponseCodesDTO("0007", ex3?.Message + ex3.InnerException?.Message + ex3?.StackTrace)
			};
		}
	}

	public void getMaxObrano()
	{
	}
}
