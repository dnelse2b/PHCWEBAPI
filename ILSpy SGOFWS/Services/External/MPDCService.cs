using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
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
using SGOFWS.Persistence.APIs.MPDC.DTOS;
using SGOFWS.Persistence.Contexts;
using SGOFWS.Persistence.DTOS;
using SGOFWS.Templates;

namespace SGOFWS.Services.External;

public class MPDCService : IMPDCService
{
	private readonly IConsignacaoRepository _consignacaoRepository;

	private readonly RouterMapper routerMapper = new RouterMapper();

	private readonly IConfiguration _configuration;

	private readonly LogHelper logHelper = new LogHelper();

	private readonly SGOFCTX _SGOFCTX;

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	private readonly ILogger<MPDCService> _logger;

	private readonly EmailHelper emailHelper = new EmailHelper();

	private readonly StringTemplates stringTemplates = new StringTemplates();

	private readonly MPDCMapper mpdcMapper = new MPDCMapper();

	private readonly IGenericRepository _genericRepository;

	private readonly MPDCAPI mPDCAPI = new MPDCAPI();

	private readonly APIRouter apiRouter = new APIRouter();

	private readonly GeoHelper geoHelper = new GeoHelper();

	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	private readonly IConsignmentService _consignmentService;

	public MPDCService(IConsignacaoRepository consignacaoRepository, IGenericRepository genericRepository, IConsignmentService consignmentService, SGOFCTX SGOFCTX)
	{
		_consignacaoRepository = consignacaoRepository;
		_genericRepository = genericRepository;
		_SGOFCTX = SGOFCTX;
		_consignmentService = consignmentService;
	}

	public MPDCService()
	{
	}

	public string GetFirmaForMPDC(string fluxo)
	{
		return (_consignacaoRepository.GetFluxoByCodigo(fluxo)?.Codfirma)?.Trim() switch
		{
			"ACE04.TCM" => "TCM", 
			"ACE01.MPDC" => "Port", 
			"ACE01.GML" => "GML", 
			_ => "N/A", 
		};
	}

	public async Task NotifyMPDCMineDeparture()
	{
		try
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			_ = DateTime.Now;
			decimal? requestID = logHelper.generateResponseID();
			ConfiguracaoDossier configuracaoDossier = configuracaoHelper.getConfiguracaoDossier("CONSIGNACAO");
			IEnumerable<Dossier> consignacoesPorNotificar = from cons in _consignacaoRepository.GetConsignacoesPorNotificarIncluirOperacao("Envio da notificação de  partida da consigação da mina -MPDC", "Envio de Novas consignações para o MPDC", "MPDC", "ENVIO", configuracaoDossier.data)
				where cons.bo3.UAdmin != "NRZ" && cons.bo3.UAdmin != "CFM"
				select cons;
			consignacoesPorNotificar.Select((Dossier dossier) => new UNotific
			{
				Entidade = "MPDC",
				Status = "NOTIFICADO",
				Tabela = "BO",
				Sentido = "ENVIO",
				Operacao = "Envio da notificação de  partida da consigação da mina -MPDC",
				Tabstamp = dossier.bo.Bostamp,
				Obs = "Envio da notificação de  partida da consigação da mina ao MPDC."
			});
			DateTime result;
			DateTime result2;
			IEnumerable<MineDepartureNotificationMPDCDTO> consignacoesPorEnviar = consignacoesPorNotificar.Select((Dossier dossier) => new MineDepartureNotificationMPDCDTO
			{
				consignmentNumber = dossier.bo3.UConsgno,
				origin = dossier.bo3.UStcarrg,
				departureDate = (from b in dossier.bi2
					orderby DateTime.TryParseExact(b.UDatcarrg.Trim() + " " + b.UHorcarrg, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result) ? result : new DateTime(1900, 1, 1) descending
					select DateTime.TryParseExact(b.UDatcarrg.Trim() + " " + b.UHorcarrg, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result2) ? result2 : new DateTime(1900, 1, 1)).FirstOrDefault()
			});
			foreach (MineDepartureNotificationMPDCDTO consignacaoPorNotificar in consignacoesPorEnviar)
			{
				string origin = consignacaoPorNotificar.origin;
				if (origin != null && origin.Trim().Length > 0)
				{
					ResponseDTO notificationResult = await mPDCAPI.NotifyConsignmentDeparture(consignacaoPorNotificar, consignacaoPorNotificar?.consignmentNumber);
					if (notificationResult?.response?.cod == "0000")
					{
						UNotific notific = new UNotific
						{
							Entidade = "MPDC",
							Status = "NOTIFICADO",
							Tabela = "BO",
							Sentido = "ENVIO",
							Operacao = "Envio da notificação de  partida da consigação da mina -MPDC",
							Tabstamp = consignacaoPorNotificar?.consignmentNumber,
							Obs = "Envio da notificação de  partida da consigação da mina ao MPDC."
						};
						_genericRepository.Add(notific);
						_genericRepository.SaveChanges();
					}
					logHelper.generateLogJB(notificationResult, requestID.ToString(), "MPDCService.NotifyConsignmentDeparture");
				}
			}
			watch.Stop();
		}
		catch (Exception)
		{
		}
	}

	[DisableConcurrentExecution(600)]
	public async Task NotifyMPDCConsignments()
	{
		try
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			decimal? requestID = logHelper.generateResponseID();
			_ = DateTime.Now;
			ConfiguracaoDossier configuracaoDossier = configuracaoHelper.getConfiguracaoDossier("CONSIGNACAO");
			List<Dossier> consignacoesPorNotificar = _consignacaoRepository.GetConsignacoesPorNotificar("Envio de Novas consignações para o MPDC", "MPDC", "ENVIO", configuracaoDossier.data);
			consignacoesPorNotificar.Select((Dossier dossier) => new UNotific
			{
				Entidade = "MPDC",
				Status = "NOTIFICADO",
				Tabela = "BO",
				Sentido = "ENVIO",
				Operacao = "Envio de Novas consignações para o MPDC",
				Tabstamp = dossier.bo.Bostamp,
				Obs = "Notificação sobre novas consignações ao MPDC"
			});

			Debug.Print($"LISTA DE CONSIGNAÇÕES ENCONTRADAS  {consignacoesPorNotificar.Count()}");
			IEnumerable<ConsignmentNotificationDTO> consignacoesPorEnviar = from dossier in consignacoesPorNotificar
				select new ConsignmentNotificationDTO
				{
					consignmentNumber = dossier.bo3.UConsgno,
					countryCode = "ZA",
					destination = GetFirmaForMPDC(dossier?.bo3?.UFluxocom),
					wagons = dossier.bi2.Select((Bi2 vagao) => new WagonNotificationMPDCDTO
					{
						wagonNumber = vagao.UVagno,
						cargo = vagao.UDsccmcag,
						weight = vagao.UPeso,
						wagonStatus = ((vagao.UFullempy == true) ? "loaded" : "empty")
					}).ToList()
				} into cenv
				where cenv.destination != "N/A"
				select cenv;
			foreach (ConsignmentNotificationDTO consignacao in consignacoesPorEnviar)
			{
				Debug.Print($"Total de consignações por enviar {consignacoesPorEnviar.Count()}");
				ResponseDTO notificationResult = await mPDCAPI.NotifyConsignment(consignacao);
				if (notificationResult?.response?.cod == "0000")
				{
					UNotific notific = new UNotific
					{
						Entidade = "MPDC",
						Status = "NOTIFICADO",
						Tabela = "BO",
						Sentido = "ENVIO",
						Operacao = "Envio de Novas consignações para o MPDC",
						Tabstamp = consignacao?.consignmentNumber,
						Obs = "Notificação sobre novas consignações ao MPDC"
					};
					_genericRepository.Add(notific);
					_genericRepository.SaveChanges();
				}
				logHelper.generateLogJB(notificationResult, requestID.ToString(), "MPDCService.NotifyMPDCConsignments");
			}
			watch.Stop();
		}
		catch (Exception ex)
		{
			Debug.Print($"ERRO AO ENVIAR CONSIGNACOES {ex.Message}  {ex.InnerException}");
		}
	}

	public void HandleConsignmentUpdate(UpdateConsignmentDataMPDCDTO updateConsignment)
	{
		try
		{
			List<Bo> listaBo = new List<Bo>();
			List<Bo3> listaBo3 = new List<Bo3>();
			List<AgenteTransitario> agentes = new List<AgenteTransitario>();
			List<Cl> clientes = new List<Cl>();
			Dossier dadosConsignacao = _consignacaoRepository.getConsignacao(updateConsignment?.consignmentNumber);
			Bo3 consignacaoExistente = dadosConsignacao?.bo3;
			Dossier dossier = routerMapper.mapData("MPDC", updateConsignment);
			if (dadosConsignacao == null)
			{
				ConfiguracaoDossier configuracaoDossier = configuracaoHelper.getConfiguracaoDossier("CONSIGNACAO");
				decimal ndos = configuracaoDossier.ndos;
				string nmdos = "Consignação";
				string clnome = configuracaoDossier.customerName;
				decimal no = configuracaoDossier.no;
				decimal? obrano = _consignacaoRepository.getMaxObrano(ndos);
				string bostamp = 25.UseThisSizeForStamp();
				dossier.bo3.UObs = updateConsignment.ToString();
				dossier.bo.Ndos = ndos;
				dossier.bo.Obrano = obrano;
				dossier.bo.Nmdos = nmdos;
				dossier.bo.Nome = clnome;
				dossier.bo.No = no;
				dossier.bo3.UAdmin = geoHelper.getAdminByCountryCode(updateConsignment?.countryCode);
				dossier.novo = true;
				_consignacaoRepository.criarConsignacao(dossier);
			}
			if (consignacaoExistente != null)
			{
				string consignacaostamp = consignacaoExistente.Bo3stamp;
				decimal? agenteno = _consignacaoRepository.getMaxAgente();
				AgenteTransitario agente = _consignacaoRepository.getAgenteByNuit(updateConsignment?.agentNuit);
				string agentestamp = 25.UseThisSizeForStamp();
				if (agente != null)
				{
					agenteno = agente.No;
					agentestamp = agente.AgenteTransitarioStamp;
				}
				foreach (WagonMPDCDTO wagon2 in updateConsignment.wagons)
				{
					Bi2 dadosVagao = _consignacaoRepository.getVagao(consignacaostamp, wagon2.wagonNumber);
					if (dadosVagao != null)
					{
						dadosVagao.UAgemail = updateConsignment?.agentEmail;
						dadosVagao.UAgnome = updateConsignment?.agentName;
						dadosVagao.UAgtelef = updateConsignment?.agentPhone;
						dadosVagao.UAgnuit = updateConsignment?.agentNuit;
						dadosVagao.UPesagent = wagon2.weight;
					}
					if (dadosVagao == null && updateConsignment?.countryCode == "ZW")
					{
						dossier.bo = dadosConsignacao.bo;
						dossier.bo2 = dadosConsignacao.bo2;
						dossier.bo3 = dadosConsignacao.bo3;
						Bi2 vagaoDossier = dossier.bi2.Where((Bi2 bi2) => bi2.UVagno.Trim() == wagon2.wagonNumber).FirstOrDefault();
						_consignacaoRepository.inserirVagaoSeNaoExiste(vagaoDossier, consignacaoExistente.Bo3stamp, dossier);
					}
				}
				AgenteTransitario agenteTransitario = new AgenteTransitario
				{
					AgenteTransitarioStamp = agentestamp,
					Nuit = updateConsignment?.agentNuit,
					No = agenteno,
					Nome = updateConsignment?.agentName
				};
				agentes.Add(agenteTransitario);
				_genericRepository.BulkUpsertEntity(agentes, new List<string> { "Nuit" }, saveChanges: false);
				string[] wagonNumbers = updateConsignment.wagons.Select((WagonMPDCDTO wagon) => wagon.wagonNumber).ToArray();
				dadosConsignacao.bi2 = dadosConsignacao.bi2.Where((Bi2 bi2) => wagonNumbers.Contains(bi2.UVagno)).ToList();
				UNotific notificacao = new UNotific
				{
					Entidade = "MPDC",
					Status = "NOTIFICADO",
					Tabela = "BO",
					Sentido = "RECEBIMENTO",
					Operacao = "Actualização de consignações-MPDC",
					Tabstamp = consignacaostamp,
					Obs = "Notificação sobre a actualização dos dados ad consignações do MPDC"
				};
				_genericRepository.Add(notificacao);
			}
			_genericRepository.SaveChanges();
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message.ToString());
		}
	}

	public ResponseDTO UpdateConsignment(UpdateConsignmentDataMPDCDTO updateConsignment)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		BackgroundJob.Enqueue(() => HandleConsignmentUpdate(updateConsignment));
		return response;
	}

	public async Task<ResponseDTO> StartShuntingAsync(ShuntingDTO shunting)
	{
		decimal? responseID = logHelper.generateResponseID();
		new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		Manobras manobra = _consignacaoRepository.GetManobra(shunting.wagonsRequestId);
		List<LinhasManobra> linhasManobra = _consignacaoRepository.GetLinhasManobrasByPedidoId(shunting.wagonsRequestId);
		List<string> wagons = new List<string>();
		foreach (LinhasManobra linha in linhasManobra)
		{
			wagons.Add(linha.Novg);
		}
		shunting.wagons = wagons;
		shunting.shuntingId = manobra.No;
		ResponseDTO startShuntingResponse = await mPDCAPI.StartShunting(shunting);
		if (startShuntingResponse.response.cod != "0000")
		{
			if (startShuntingResponse.Data.ToString().Contains("cannot be prior to request approval date") || startShuntingResponse.Data.ToString().Contains("cannot be greater"))
			{
				startShuntingResponse.response.cod = "000509";
				startShuntingResponse.response.codDesc = "Mensagem do MPDC: A data de início da manobra não pode ser inferior a data de aprovação do pedido";
			}
			return startShuntingResponse;
		}
		_consignacaoRepository.IniciarManobra(shunting);
		return startShuntingResponse;
	}

	public async Task<ResponseDTO> CloseShuntingAsync(ShuntingDTO shunting)
	{
		decimal? responseID = logHelper.generateResponseID();
		new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		Manobras manobra = _consignacaoRepository.GetManobra(shunting.wagonsRequestId);
		List<LinhasManobra> linhasManobra = _consignacaoRepository.GetLinhasManobrasByPedidoId(shunting.wagonsRequestId);
		List<string> wagons = new List<string>();
		foreach (LinhasManobra linha in linhasManobra)
		{
			wagons.Add(linha.Novg);
		}
		shunting.wagons = wagons;
		shunting.shuntingId = manobra.No;
		ResponseDTO closeShuntingResponse = await mPDCAPI.CloseShunting(shunting);
		if (closeShuntingResponse.response.cod != "0000")
		{
			return closeShuntingResponse;
		}
		_consignacaoRepository.FinalizarManobra(shunting);
		BackgroundJob.Enqueue(() => _consignmentService.FinalizarManobra(shunting, "MPDC"));
		return closeShuntingResponse;
	}

	public async Task NotifyUpdateStation(List<UpdateStationRequest> updateStationRequests, decimal requestID)
	{
		foreach (UpdateStationRequest updateStationRequestBody in updateStationRequests)
		{
			if (DateTime.Parse(updateStationRequestBody.operationDate).Year != 1900)
			{
				ResponseDTO updateStationsResult = await mPDCAPI.UpdateStation(updateStationRequestBody);
				if (updateStationsResult?.response?.cod == "0000")
				{
					_genericRepository.SaveChanges();
					ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0000", updateStationsResult.ToString(), logHelper.generateResponseID()), updateStationsResult.ToString(), null);
					logHelper.generateLogJB(finalResponse, requestID.ToString(), "MPDCService.UpdateStation");
				}
			}
		}
	}

	public async Task UpdateStation()
	{
		logHelper.generateResponseID();
		List<UStqueue> stQueue = _consignacaoRepository.GetStQueueByEntity("MPDC");
		List<UStqueue> orderedStations = stQueue.OrderBy((UStqueue s) => s.Ord).ToList();
		List<UStqueue> stqueuesToRemove = new List<UStqueue>();
		foreach (UStqueue updateStationRequestBody in orderedStations)
		{
			if (updateStationRequestBody == null || updateStationRequestBody.Operationdate.Year != 1900)
			{
				List<UpdateStationWagonDTO> wagons = JsonConvert.DeserializeObject<List<UpdateStationWagonDTO>>(updateStationRequestBody.Wagonsdata);
				MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
				{
					cfg.CreateMap<UpdateStationWagonDTO, UpdateStationRequestWagon>();
				});
				Mapper mapper = new Mapper(config);
				List<UpdateStationRequestWagon> mappedWagons = mapper.Map<List<UpdateStationRequestWagon>>(wagons);
				List<UpdateStationRequestWagon> updateRequestWagons = new List<UpdateStationRequestWagon>();
				foreach (UpdateStationRequestWagon wagon in mappedWagons)
				{
					wagon.cargo = _consignacaoRepository.GetVagaoByConsgnoAndVagno(wagon.wagonNumber, wagon.consignmentNumber)?.UDsccmcag;
					UNotific notificacaoData = _consignacaoRepository.GetNotificacaoByTabstampAndEntidade(wagon.consignmentNumber, "MPDC");
					if (notificacaoData != null)
					{
						updateRequestWagons.Add(wagon);
					}
				}
				if (updateRequestWagons != null && updateRequestWagons.Count() > 0)
				{
					UpdateStationRequest upd = new UpdateStationRequest
					{
						station = updateStationRequestBody.Station,
						stationCode = updateStationRequestBody.Stationcode,
						etaToNextStation = updateStationRequestBody.Etanextst.ToString(),
						operationDate = updateStationRequestBody.Operationdate.ToString(),
						nextStation = updateStationRequestBody?.Nextstation,
						nextStationCode = updateStationRequestBody?.Nextstcode,
						etaToLastStation = updateStationRequestBody?.Etadestination.ToString(),
						trainNumber = updateStationRequestBody.Trainnumber,
						trainReference = updateStationRequestBody.Trainreference,
						trainOrientation = updateStationRequestBody.Trainorientation,
						operationType = updateStationRequestBody?.Operationtype,
						wagons = updateRequestWagons
					};
					if ((await mPDCAPI.UpdateStation(upd))?.response?.cod != "I-500")
					{
						stqueuesToRemove.Add(updateStationRequestBody);
					}
				}
			}
			if (updateStationRequestBody != null && updateStationRequestBody.Operationdate.Year == 1900)
			{
				stqueuesToRemove.Add(updateStationRequestBody);
			}
		}
		_SGOFCTX.RemoveRange(stqueuesToRemove);
		_SGOFCTX.SaveChanges();
	}

	public ResponseDTO RequestForWagonSupply(WagonSupplyRequestDTO wagonSupplyRequestDTO)
	{
		decimal? responseID = logHelper.generateResponseID();
		return _consignmentService.ProcessarPedidoFornecimento(wagonSupplyRequestDTO, "MPDC");
	}

	public ResponseDTO RequestForWagonWithdrawal(WagonWithdrawalResquestDTO wagonWithdrawalRequestDTO)
	{
		decimal? responseID = logHelper.generateResponseID();
		return _consignmentService.ProcessarPedidoRetirada(wagonWithdrawalRequestDTO, "MPDC");
	}

	public ResponseDTO ValidateRequest(ApproveWagonOperationDTO ApproveWagonOperationDTO, bool external)
	{
		decimal? responseID = logHelper.generateResponseID();
		string stampPedido = "";
		PlanoManobra cabecalhoPlanoManobra = new PlanoManobra();
		CabecalhoPedidoRetirada pedidoRetirada = new CabecalhoPedidoRetirada();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		if (ApproveWagonOperationDTO?.requestType?.Trim() == "Retirada")
		{
			pedidoRetirada = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(ApproveWagonOperationDTO.requestNumber);
			stampPedido = pedidoRetirada.CabecalhoPedidoRetiradaStamp;
		}
		if (ApproveWagonOperationDTO?.requestType?.Trim() == "Fornecimento")
		{
			cabecalhoPlanoManobra = _consignacaoRepository.GetPlanoMabobraByPedidoID(ApproveWagonOperationDTO.requestNumber);
			stampPedido = cabecalhoPlanoManobra?.cabecalhoPlanoManobra?.CabecalhoPlanoManobraStamp;
		}
		List<VeiculosPorRegularizar> veiculosPorRegularizar = _consignacaoRepository.GetVeiculosPorRegularizarByPedidoStamp(stampPedido);
		if (veiculosPorRegularizar.Count > 0)
		{
			string mensagem = (external ? "There are wagons that have not been regularized yet" : "Não é possível aprovar o pedido pois existem vagões que ainda não foram regularizados.");
			return new ResponseDTO(new ResponseCodesDTO("000509", mensagem, responseID), null, null);
		}
		if (ApproveWagonOperationDTO?.requestType?.Trim() == "Fornecimento")
		{
			foreach (LinhasPlanoManobra linha in cabecalhoPlanoManobra?.linhasPlanoManobras)
			{
				LinhaVeiculoFerroviario linhaveiculo = _consignacaoRepository.GetLinhaVeiculoByStampVag(linha.Stampevg);
				if (linhaveiculo == null)
				{
					string mensagem2 = (external ? (" The wagon " + linha?.Novg + " is not associated to any train ") : ("A entidade vagão Nº " + linha?.Novg + " não está associada a nenhum  comboio"));
					return new ResponseDTO(new ResponseCodesDTO("000509", mensagem2, responseID), null, null);
				}
				DesengateComboio desengateComboio = _consignacaoRepository.GetDesengateComboioByComboioStamp(linhaveiculo?.Stampcomboio);
				if (desengateComboio == null)
				{
					string mensagem3 = (external ? ("The wagon " + linha?.Novg + " is associated with a train that has not been uncoupled yet") : ("A entidade vagão Nº " + linha?.Novg + " está associada a um comboio que não tem o desengate feito."));
					return new ResponseDTO(new ResponseCodesDTO("000509", mensagem3, responseID), null, null);
				}
			}
		}
		return response;
	}

	public async Task<string> HandleApproveWagonOperation(ApproveWagonOperationDTO ApproveWagonOperationDTO)
	{
		decimal? requestID = logHelper.generateResponseID();
		try
		{
			string stampManobra = 25.useThisSizeForStamp();
			Manobras dadosManobra = _consignacaoRepository.GetManobraById(ApproveWagonOperationDTO.requestNumber, ApproveWagonOperationDTO.requestType);
			Manobras manobra = new Manobras();
			List<LinhasManobra> linhasManobra = new List<LinhasManobra>();
			PlanoManobra cabecalhoPlanoManobra = _consignacaoRepository.GetPlanoMabobraByPedidoID(ApproveWagonOperationDTO.requestNumber);
			string numeroBoletim = _consignmentService.GerarNumeroSequencialManobra(DateTime.Now);
			if (dadosManobra != null)
			{
				stampManobra = dadosManobra?.ManobraStamp;
				numeroBoletim = dadosManobra?.No;
				_consignacaoRepository.RemoveLinhasManobra(stampManobra);
			}
			if (ApproveWagonOperationDTO.requestType.Trim() == "Fornecimento")
			{
				manobra = new Manobras
				{
					Pedidoid = ApproveWagonOperationDTO.requestNumber,
					No = numeroBoletim,
					Dataini = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? new DateTime?(DateTime.Now.Date) : ApproveWagonOperationDTO?.autoShuntingStartDate.Date),
					Datafim = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? new DateTime?(new DateTime(1900, 1, 1)) : ApproveWagonOperationDTO?.autoShuntingEndDate.Date),
					Horaini = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? "" : ApproveWagonOperationDTO?.autoShuntingStartDate.ToString("HH:mm")),
					Horafim = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? "" : ApproveWagonOperationDTO?.autoShuntingEndDate.ToString("HH:mm")),
					Manobrafinalizada = ApproveWagonOperationDTO?.autoShuntingStartEnd,
					Manobrainiciada = ApproveWagonOperationDTO?.autoShuntingStartEnd,
					Tipo = "Fornecimento",
					Tpforn = cabecalhoPlanoManobra.cabecalhoPlanoManobra?.Tpforn,
					ManobraStamp = stampManobra,
					Estacao = cabecalhoPlanoManobra.cabecalhoPlanoManobra?.Estacao,
					Patio = cabecalhoPlanoManobra.cabecalhoPlanoManobra?.Patio,
					Loco = cabecalhoPlanoManobra.cabecalhoPlanoManobra?.Loco,
					Ousrdata = DateTime.Now.Date,
					Usrdata = DateTime.Now.Date,
					Ousrhora = DateTime.Now.ToString("HH:mm"),
					Usrhora = DateTime.Now.ToString("HH:mm")
				};
				foreach (LinhasPlanoManobra linha in cabecalhoPlanoManobra?.linhasPlanoManobras)
				{
					LinhaVeiculoFerroviario linhaveiculo = _consignacaoRepository.GetLinhaVeiculoByStampVag(linha.Stampevg);
					string descprocedencia = "";
					string codprocedencia = "";
					string patio = "";
					string destino = "";
					PatiosManobra dadosPatio = _consignacaoRepository.GetPatioManobraByCodigo(cabecalhoPlanoManobra.cabecalhoPlanoManobra.Patio);
					LocaisFerroviarios localFerroviario = _consignacaoRepository.GetLocalFerroviario(cabecalhoPlanoManobra.cabecalhoPlanoManobra.Codest);
					if (localFerroviario != null)
					{
						destino = localFerroviario.Design;
					}
					if (dadosPatio != null)
					{
						patio = dadosPatio.Design;
					}
					if (linhaveiculo != null)
					{
						DesengateComboio desengateComboio = _consignacaoRepository.GetDesengateComboioByComboioStamp(linhaveiculo?.Stampcomboio);
						if (desengateComboio != null)
						{
							codprocedencia = desengateComboio.Local;
							descprocedencia = desengateComboio.Desiglocal;
						}
					}
					EntidadeVagao dadosEntidade = _consignacaoRepository.GetEntidadeVagaoByStamp(linha.Stampevg);
					LinhasManobra linhaManobra = new LinhasManobra
					{
						ManobraStamp = stampManobra,
						Nrbm = numeroBoletim,
						LinhasManobraStamp = 25.useThisSizeForStamp(),
						Novg = linha.Novg,
						Stampevg = linha?.Stampevg,
						Ousrdata = DateTime.Now.Date,
						Usrdata = DateTime.Now.Date,
						Ousrhora = DateTime.Now.ToString("HH:mm"),
						Usrhora = DateTime.Now.ToString("HH:mm"),
						Codori = codprocedencia,
						Origem = descprocedencia,
						Coddest = cabecalhoPlanoManobra.cabecalhoPlanoManobra.Codest,
						Codpatio = cabecalhoPlanoManobra.cabecalhoPlanoManobra.Patio,
						Destino = destino,
						Patio = patio,
						Stampveic = dadosEntidade?.Stampveic
					};
					linhaManobra.AssignDefaultEntityValues();
					linhasManobra.Add(linhaManobra);
				}
				_consignacaoRepository.AprovarFornecimentoVagao(ApproveWagonOperationDTO.requestNumber);
			}
			if (ApproveWagonOperationDTO.requestType.Trim() == "Retirada")
			{
				CabecalhoPedidoRetirada pedidoRetiradaResult = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(ApproveWagonOperationDTO.requestNumber);
				List<EntidadeVagao> entidadesVagao = _consignacaoRepository.GetEntidadeVagaoByPedidoRetiradaStamp(pedidoRetiradaResult?.CabecalhoPedidoRetiradaStamp);
				UOridest dadosEstacao = _consignacaoRepository.GetDadosEstacaoByCodigo(pedidoRetiradaResult?.Estacao);
				entidadesVagao.FirstOrDefault();
				PatiosManobra dadosPatio2 = _consignacaoRepository.GetPatioManobraByCodigo(dadosEstacao?.Patio);
				manobra = new Manobras
				{
					Pedidoid = ApproveWagonOperationDTO.requestNumber,
					No = numeroBoletim,
					Tpforn = "Vazios",
					Dataini = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? new DateTime?(DateTime.Now.Date) : ApproveWagonOperationDTO?.autoShuntingStartDate.Date),
					Datafim = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? new DateTime?(new DateTime(1900, 1, 1)) : ApproveWagonOperationDTO?.autoShuntingEndDate.Date),
					Horaini = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? "" : ApproveWagonOperationDTO?.autoShuntingStartDate.ToString("HH:mm")),
					Horafim = ((!(ApproveWagonOperationDTO?.autoShuntingStartEnd ?? false)) ? "" : ApproveWagonOperationDTO?.autoShuntingEndDate.ToString("HH:mm")),
					Manobrafinalizada = ApproveWagonOperationDTO?.autoShuntingStartEnd,
					Manobrainiciada = ApproveWagonOperationDTO?.autoShuntingStartEnd,
					Tipo = "Retirada",
					Patio = dadosEstacao?.Patio,
					Desvio = pedidoRetiradaResult?.Desvio,
					Loco = pedidoRetiradaResult?.Loco,
					ManobraStamp = stampManobra,
					Npretirad = pedidoRetiradaResult?.Nrped,
					Estacao = pedidoRetiradaResult?.Estacao,
					Ousrhora = DateTime.Now.ToString("HH:mm"),
					Usrhora = DateTime.Now.ToString("HH:mm")
				};
				LocaisFerroviarios localFerroviario2 = _consignacaoRepository.GetLocalFerroviario(pedidoRetiradaResult?.Desvio);
				_ = localFerroviario2?.Design;
				foreach (EntidadeVagao vagao in entidadesVagao)
				{
					Fluxo dadosFluxo = _consignacaoRepository.GetFluxoByCodigo(vagao?.Fluxo);
					LinhasManobra linhaManobra2 = new LinhasManobra
					{
						ManobraStamp = stampManobra,
						Nrbm = numeroBoletim,
						LinhasManobraStamp = 25.useThisSizeForStamp(),
						Codori = pedidoRetiradaResult.Desvio,
						Origem = localFerroviario2?.Design,
						Codest = pedidoRetiradaResult?.Estacao,
						Codcarga = dadosFluxo?.Codcarga,
						Carga = dadosFluxo?.Carga,
						Patio = dadosPatio2?.Design,
						Codpatio = dadosPatio2?.Codigo,
						Estacao = dadosEstacao?.Cidade,
						Coddest = "",
						Destino = "",
						Novg = vagao.No,
						Stampevg = vagao.EntidadeVagaostamp,
						Ousrdata = DateTime.Now.Date,
						Usrdata = DateTime.Now.Date,
						Ousrhora = DateTime.Now.ToString("HH:mm"),
						Usrhora = DateTime.Now.ToString("HH:mm"),
						Stampveic = vagao?.Stampveic,
						Stamppedret = pedidoRetiradaResult?.CabecalhoPedidoRetiradaStamp
					};
					linhaManobra2.AssignDefaultEntityValues();
					linhasManobra.Add(linhaManobra2);
				}
				_consignacaoRepository.AprovarRetiradaVagao(ApproveWagonOperationDTO.requestNumber);
			}
			List<KeyValuePair<string, object>> conditionsManobra = new List<KeyValuePair<string, object>>();
			conditionsManobra.Add(new KeyValuePair<string, object>("ManobraStamp", stampManobra));
			List<string> keysToExcludeManobra = new List<string>();
			keysToExcludeManobra.Add("ManobraStamp");
			_genericRepository.UpsertEntity(manobra, keysToExcludeManobra, conditionsManobra, saveChanges: false);
			_genericRepository.BulkAdd(linhasManobra);
			_genericRepository.SaveChanges();
			return numeroBoletim;
		}
		catch (Exception ex)
		{
			ErrorDTO errorDTO = new ErrorDTO
			{
				message = ex?.Message,
				stack = ex?.StackTrace?.ToString(),
				inner = ex?.InnerException?.ToString()
			};
			ResponseDTO finalResponse = new ResponseDTO(new ResponseCodesDTO("0007", "Error", logHelper.generateResponseID()), errorDTO.ToString(), ApproveWagonOperationDTO.ToString());
			logHelper.generateLogJB(finalResponse, requestID.ToString(), "MPDCService.HandleApproveWagonOperation");
			throw new Exception(errorDTO.ToString());
		}
	}

	public async Task<ResponseDTO> ApproveWagonOperation(ApproveWagonOperationDTO ApproveWagonOperationDTO)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		if (ApproveWagonOperationDTO.requestType == "Fornecimento")
		{
			PlanoManobra pedidoResult = _consignacaoRepository.GetPlanoMabobraByPedidoID(ApproveWagonOperationDTO.requestNumber);
			if (pedidoResult == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0404", "Request(Fornecimento) " + ApproveWagonOperationDTO.requestNumber + " not found", responseID), null, null);
			}
		}
		if (ApproveWagonOperationDTO.requestType == "Retirada")
		{
			CabecalhoPedidoRetirada pedidoResult2 = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(ApproveWagonOperationDTO.requestNumber);
			if (pedidoResult2 == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0404", "Request(Retirada) " + ApproveWagonOperationDTO.requestNumber + " not found", responseID), null, null);
			}
		}
		ResponseDTO validateRequestResult = ValidateRequest(ApproveWagonOperationDTO, external: false);
		if (validateRequestResult?.response?.cod != "0000")
		{
			return validateRequestResult;
		}
		string approvedActionDate = ApproveWagonOperationDTO.shuntingDate + " " + ApproveWagonOperationDTO.shuntingTime;
		WagonAproveDTO approveWagonsSupply = new WagonAproveDTO
		{
			mpdcWagonsRequisitionNumber = ApproveWagonOperationDTO.requestNumber,
			approvedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
			approvedWagonActionDate = approvedActionDate,
			approvalRemarks = "Approved",
			status = "Approved"
		};
		ResponseDTO approveResult = await mPDCAPI.ApproveWagonsSupply(approveWagonsSupply);
		if (approveResult.response.cod != "0000")
		{
			if (approveResult.Data.ToString().Contains("cannot be prior to submission date") || approveResult.Data.ToString().Contains("cannot be greater"))
			{
				approveResult.response.cod = "000509";
				approveResult.response.codDesc = "Mensagem do MPDC: A data de início da manobra não pode ser inferior a data de aprovação do pedido";
			}
			return approveResult;
		}
		BackgroundJob.Enqueue(() => HandleApproveWagonOperation(ApproveWagonOperationDTO));
		return response;
	}

	public ResponseDTO WagonOffload(WagonOffloadDTO wagonOffloadDTO)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		PlanoManobra requestForSupplyResult = _consignacaoRepository.GetPlanoMabobraByPedidoID(wagonOffloadDTO.mpdcSupplyRequestNumber);
		Manobras manobraFornecimento = _consignacaoRepository.GetManobraById(wagonOffloadDTO.mpdcSupplyRequestNumber, "Fornecimento");
		if (requestForSupplyResult == null)
		{
			return new ResponseDTO(new ResponseCodesDTO("0404", "Request " + wagonOffloadDTO.mpdcSupplyRequestNumber + " not found", responseID), null, null);
		}
		CabecalhoHistoricoDescarregamentoVagao cabecalhoHistorico = new CabecalhoHistoricoDescarregamentoVagao
		{
			CabecalhoHistoricoDescarregamentoVagaostamp = 25.UseThisSizeForStamp(),
			Pedidoid = wagonOffloadDTO.mpdcSupplyRequestNumber,
			Totalforn = wagonOffloadDTO.totalSuppliedWagons,
			Percentagemdesc = wagonOffloadDTO.cargoOffloadPercent,
			Totvagdesc = wagonOffloadDTO.totalOffloadedWagons,
			Totpordesc = wagonOffloadDTO.wagonsToBeOffloaded,
			Totdescarrngd = wagonOffloadDTO.wagonsOffloading,
			Meddescmin = wagonOffloadDTO.avgWagonsOffloadPerMinute,
			Totalforncarga = wagonOffloadDTO.totalSuppliedCargo,
			Totaldesccarga = wagonOffloadDTO.totalOffloadedCargo,
			Cargapordesc = wagonOffloadDTO.cargoToBeOffloaded,
			Meddesccargamin = wagonOffloadDTO.avgCargoOffloadPerMinute,
			Manobrastamp = manobraFornecimento?.ManobraStamp,
			Planomanobrastamp = requestForSupplyResult?.cabecalhoPlanoManobra?.CabecalhoPlanoManobraStamp
		};
		List<KeyValuePair<string, object>> conditionsCabecalhoHistorico = new List<KeyValuePair<string, object>>();
		conditionsCabecalhoHistorico.Add(new KeyValuePair<string, object>("Pedidoid", wagonOffloadDTO.mpdcSupplyRequestNumber));
		List<string> keysToExcludeCabecalhoHistorico = new List<string>();
		keysToExcludeCabecalhoHistorico.Add("CabecalhoHistoricoDescarregamentoVagaostamp");
		_genericRepository.UpsertEntity(cabecalhoHistorico, keysToExcludeCabecalhoHistorico, conditionsCabecalhoHistorico, saveChanges: false);
		foreach (WagonOffloadingDetailDTO wagon in wagonOffloadDTO.wagons)
		{
			LinhasPlanoManobra wagonResult = _consignacaoRepository.GetVagaoFornecimentoByVagnoAndPedido(wagon.consignmentNumber, wagonOffloadDTO.mpdcSupplyRequestNumber, wagon.wagonNumber);
			HistoricoDescarregamentoVagao wagonOffloadHistory = new HistoricoDescarregamentoVagao
			{
				HistoricoDescarregamentoVagaoStamp = 25.useThisSizeForStamp(),
				CabecalhoPlanoManobraStamp = requestForSupplyResult?.cabecalhoPlanoManobra?.CabecalhoPlanoManobraStamp,
				Consgno = wagon.consignmentNumber,
				Percentagemdescr = wagon.cargoOffloadPercent,
				Datadescarregamento = wagon.offloadStartedAt.GetValueOrDefault(),
				Novag = wagon.wagonNumber,
				Status = wagon.status
			};
			_genericRepository.Add(wagonOffloadHistory);
		}
		_genericRepository.SaveChanges();
		return response;
	}

	public async Task<ResponseDTO> TopUpRequest(TopUpDTO topUp)
	{
		UTopup topUpData = _consignacaoRepository.GetTopUpByPedidoAndTipo(topUp.requestNumber, topUp.requestType);
		string Bulletin = "";
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		Manobras manobraData = _consignacaoRepository.GetManobraById(topUp.requestNumber, topUp.requestType);
		if (manobraData != null)
		{
			Bulletin = manobraData?.No;
		}
		List<string> wagons = new List<string>();
		if (topUpData == null)
		{
			return new ResponseDTO(new ResponseCodesDTO("0404", "Request " + topUp.requestNumber + " not found", responseID), null, null);
		}
		List<UTopuplin> linhasTopUp = _consignacaoRepository.GetLinhasTopUp(topUpData.UTopupstamp);
		foreach (UTopuplin linhaTopUp in linhasTopUp)
		{
			wagons.Add(linhaTopUp.Vagno);
		}
		TopUpApprovalDTO topApprovalData = new TopUpApprovalDTO
		{
			wagonsSupplyId = 0m,
			approvedAt = DateTime.Now.Date,
			Bulletin = Bulletin,
			wagons = wagons
		};
		_consignmentService.ProcessarTopUp(topUpData.UTopupstamp, topUp, linhasTopUp);
		ResponseDTO topPupApprovalResult = await mPDCAPI.ApproveTopUp(topApprovalData, topUp.requestNumber);
		if (topPupApprovalResult?.response?.cod == "0000")
		{
			_consignacaoRepository.ApproveTopUp(topUpData.UTopupstamp);
			return response;
		}
		return topPupApprovalResult;
	}

	public RequestTypeHandleResult handleShuntingRequestType(string requestType)
	{
		if (requestType?.Trim()?.ToLower() == "supply")
		{
			return new RequestTypeHandleResult
			{
				valid = true,
				result = "Fornecimento"
			};
		}
		if (requestType?.Trim()?.ToLower() == "withdrawal")
		{
			return new RequestTypeHandleResult
			{
				valid = true,
				result = "Retirada"
			};
		}
		return new RequestTypeHandleResult
		{
			valid = false,
			result = ""
		};
	}

	public async Task<ResponseDTO> SubmitShunting(ShuntingDTO shunting)
	{
		decimal? responseID = logHelper.generateResponseID();
		RequestTypeHandleResult handleRequestTypeResult = handleShuntingRequestType(shunting?.requestType);
		if (handleRequestTypeResult != null && !handleRequestTypeResult.valid)
		{
			return new ResponseDTO(new ResponseCodesDTO("00400", "Invalid Shunting request type", responseID), null, null);
		}
		ApproveWagonOperationDTO approveDTO = new ApproveWagonOperationDTO
		{
			autoShuntingStartEnd = true,
			autoShuntingStartDate = DateTime.Parse(shunting?.shuntingStartDate.ToString()),
			autoShuntingEndDate = DateTime.Parse(shunting?.shuntingEndDate),
			requestType = handleRequestTypeResult?.result,
			requestNumber = shunting?.wagonsRequestId
		};
		ResponseDTO validateRequestResult = ValidateRequest(approveDTO, external: true);
		if (validateRequestResult?.response?.cod != "0000")
		{
			return validateRequestResult;
		}
		string handleApproveResult = await HandleApproveWagonOperation(approveDTO);
		_ = new
		{
			shuntingID = handleApproveResult
		};
		shunting.shuntingId = handleApproveResult;
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), shunting, null);
	}
}
