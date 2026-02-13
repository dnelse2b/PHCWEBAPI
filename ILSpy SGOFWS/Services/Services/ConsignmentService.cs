using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OPPWS.Extensions;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.Domains.Models.CfmMain;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;
using SGOFWS.Persistence.APIs.MPDC.DTOS;
using SGOFWS.Persistence.Contexts;
using SGOFWS.Persistence.DTOS;
using Z.EntityFramework.Plus;

namespace SGOFWS.Services;

public class ConsignmentService : IConsignmentService
{
	private readonly IGenericRepository _genericRepository;

	private readonly ICFMMAinBDRepository _cfmMAinBDRepository;

	private readonly CFMMAINCONTEXT _cFMMAINCONTEXT;

	private readonly SGOFCTX _SGOFCTX;

	private readonly IConsignacaoRepository _consignacaoRepository;

	private readonly ConfiguracaoHelper configuracaoHelper = new ConfiguracaoHelper();

	private readonly LogHelper logHelper = new LogHelper();

	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	private readonly LocationHelper locationHelper = new LocationHelper();

	private readonly IMPDCHelper _MPDCHelper;

	public ConsignmentService(IGenericRepository genericRepository, SGOFCTX SGOFCTX, IConsignacaoRepository consignacaoRepository, ICFMMAinBDRepository cfmMAinBDRepository, CFMMAINCONTEXT cFMMAINCONTEXT, IMPDCHelper MPDCHelper)
	{
		_genericRepository = genericRepository;
		_SGOFCTX = SGOFCTX;
		_consignacaoRepository = consignacaoRepository;
		_cfmMAinBDRepository = (_cfmMAinBDRepository = cfmMAinBDRepository);
		_cFMMAINCONTEXT = cFMMAINCONTEXT;
		_MPDCHelper = MPDCHelper;
	}

	public ConsignmentService()
	{
	}

	public ResponseDTO GetRequestByID(string requestType, string requestID)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		if (requestType?.Trim() == "Supply")
		{
			PlanoManobra cabecalhoPlanoManobra = _consignacaoRepository.GetPlanoMabobraByPedidoID(requestID?.Trim());
			if (cabecalhoPlanoManobra == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0000", $"The {requestType} with ID {requestID} was not found", responseID), null, null);
			}
			DateTime submitedAt = conversionExtension.StringToDateTime(cabecalhoPlanoManobra?.cabecalhoPlanoManobra?.Ousrdata.Value.ToString("yyyy-MM-dd") + " " + cabecalhoPlanoManobra?.cabecalhoPlanoManobra?.Ousrhora);
			string boletim = "";
			Manobras manobraData = _consignacaoRepository.GetManobra(requestID?.Trim());
			if (manobraData != null)
			{
				boletim = manobraData?.No;
			}
			MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<LinhasPlanoManobra, WagonRequestResultDTO>().ForPath((WagonRequestResultDTO dest) => dest.wagonNumber, delegate(IPathConfigurationExpression<LinhasPlanoManobra, WagonRequestResultDTO, string> act)
				{
					act.MapFrom((LinhasPlanoManobra src) => src.Novg);
				}).ForPath((WagonRequestResultDTO dest) => dest.consignmentNumber, delegate(IPathConfigurationExpression<LinhasPlanoManobra, WagonRequestResultDTO, string> act)
				{
					act.MapFrom((LinhasPlanoManobra src) => src.Consgno);
				})
					.ForPath((WagonRequestResultDTO dest) => dest.processed, delegate(IPathConfigurationExpression<LinhasPlanoManobra, WagonRequestResultDTO, bool> act)
					{
						act.MapFrom((LinhasPlanoManobra src) => true);
					});
			});
			Mapper mapper = new Mapper(config);
			List<WagonRequestResultDTO> mappedWagons = mapper.Map<List<WagonRequestResultDTO>>(cabecalhoPlanoManobra?.linhasPlanoManobras);
			WagonSupplyRequestResponseDTO wagonSupplyRequest = new WagonSupplyRequestResponseDTO
			{
				SubmittedAt = submitedAt,
				bolletin = boletim,
				mpdcRequestForSupplyNumber = cabecalhoPlanoManobra?.cabecalhoPlanoManobra?.PedidoId,
				wagons = mappedWagons
			};
			return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), wagonSupplyRequest, null);
		}
		if (requestType?.Trim() == "Withdrawal")
		{
			CabecalhoPedidoRetirada pedidoRetiradaResult = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(requestID?.Trim());
			if (pedidoRetiradaResult == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0000", $"The {requestType} with ID {requestID} was not found", responseID), null, null);
			}
			DateTime submitedAt2 = conversionExtension.StringToDateTime(pedidoRetiradaResult?.Ousrdata.Value.ToString("yyyy-MM-dd") + " " + pedidoRetiradaResult?.Ousrhora);
			List<EntidadeVagao> entidades = _consignacaoRepository.GetEntidadesByStampRetirada(pedidoRetiradaResult?.CabecalhoPedidoRetiradaStamp);
			MapperConfiguration config2 = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<EntidadeVagao, WagonRequestResultDTO>().ForPath((WagonRequestResultDTO dest) => dest.wagonNumber, delegate(IPathConfigurationExpression<EntidadeVagao, WagonRequestResultDTO, string> act)
				{
					act.MapFrom((EntidadeVagao src) => src.No);
				}).ForPath((WagonRequestResultDTO dest) => dest.consignmentNumber, delegate(IPathConfigurationExpression<EntidadeVagao, WagonRequestResultDTO, string> act)
				{
					act.MapFrom((EntidadeVagao src) => src.Consorig);
				})
					.ForPath((WagonRequestResultDTO dest) => dest.processed, delegate(IPathConfigurationExpression<EntidadeVagao, WagonRequestResultDTO, bool> act)
					{
						act.MapFrom((EntidadeVagao src) => true);
					});
			});
			Mapper mapper2 = new Mapper(config2);
			List<WagonRequestResultDTO> mappedWagons2 = mapper2.Map<List<WagonRequestResultDTO>>(entidades);
			string boletim2 = "";
			Manobras manobraData2 = _consignacaoRepository.GetManobra(requestID?.Trim());
			if (manobraData2 != null)
			{
				boletim2 = manobraData2?.No;
			}
			WagonWithdrawalResponseDTO wagonsWithdrawal = new WagonWithdrawalResponseDTO
			{
				SubmittedAt = submitedAt2,
				bolletin = boletim2,
				mpdcRequestForWithdrawalNumber = pedidoRetiradaResult?.Pedidoid,
				wagons = mappedWagons2
			};
			return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), wagonsWithdrawal, null);
		}
		return new ResponseDTO(new ResponseCodesDTO("0000", "Invalid request type " + requestType + " ", responseID), null, null);
	}

	public ResponseDTO GetStationsByRange(DateTime startDate, DateTime endDate, string entity)
	{
		decimal? responseID = logHelper.generateResponseID();
		List<ComboioRegisto> comboios = _consignacaoRepository.GetComboiosByRangeDate(startDate, endDate);
		List<UStqueue> stationQueueData = new List<UStqueue>();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		foreach (ComboioRegisto comboio in comboios)
		{
			RotaTrack rota = _consignacaoRepository.GetRotaTrack(comboio?.Codorig?.Trim(), comboio?.Coddest?.Trim());
			if (rota == null)
			{
				continue;
			}
			ComboioNotificacao dadosComboio = _consignacaoRepository.GetComboioNotificacao(comboio.ComboioRegistoStamp);
			ComboioNotificacao comboioNotificacao = dadosComboio;
			if (comboioNotificacao == null || comboioNotificacao.vagoes.Count() <= 0)
			{
				continue;
			}
			List<UTrajrot> trajecto = rota.trajecto;
			List<UEntrot> entidadesNotificacao = rota.entidades;
			dadosComboio.comboio = comboio;
			ComboioRegisto comboioRegisto = comboio;
			if (comboioRegisto != null && comboioRegisto.Partiu == true)
			{
				List<UStqueue> estacQueue = CreateStQueue(dadosComboio, partida: true, trajecto, entidadesNotificacao);
				stationQueueData = stationQueueData.Concat(estacQueue).ToList();
			}
			ComboioRegisto comboioRegisto2 = comboio;
			if (comboioRegisto2 != null && comboioRegisto2.Chegou == true)
			{
				List<UStqueue> estacQueue2 = CreateStQueue(dadosComboio, partida: false, trajecto, entidadesNotificacao);
				stationQueueData = stationQueueData.Concat(estacQueue2).ToList();
			}
			foreach (Tempos tempo in dadosComboio.tempos)
			{
				UTrajrot estacaoNotificacao = trajecto.Where((UTrajrot trj) => trj.Codigo.Trim() == tempo.Codest.Trim()).FirstOrDefault();
				if (estacaoNotificacao == null)
				{
					continue;
				}
				foreach (UEntrot dest in entidadesNotificacao)
				{
					UTrajrot nextStationNotificacao = new UTrajrot();
					nextStationNotificacao = trajecto.Where((UTrajrot trj) => trj?.Codigo.Trim() == estacaoNotificacao?.Codprxst.Trim()).FirstOrDefault();
					UTrajrot bossaDataNot = trajecto.Where((UTrajrot trj) => trj != null && trj.Bossa && trj?.Codigo?.Trim() == comboio?.Coddest?.Trim()).FirstOrDefault();
					decimal? ordem = estacaoNotificacao?.Ordem;
					string etaNextStation = "1900-01-01 00:00";
					string etaToBossa = "1900-01-01 00:00";
					string etaToDestination = "1900-01-01 00:00";
					Tempos proximaEstacao = new Tempos();
					proximaEstacao = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == nextStationNotificacao?.Codigo.Trim()).FirstOrDefault();
					Tempos bossa = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == bossaDataNot?.Codigo.Trim()).FirstOrDefault();
					Tempos destinationStation = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == dadosComboio.comboio?.Coddest.Trim()).FirstOrDefault();
					if (proximaEstacao != null)
					{
						etaNextStation = proximaEstacao.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + proximaEstacao.Hrprevc;
					}
					if (bossa != null)
					{
						etaToBossa = bossa.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + bossa.Hrprevc;
					}
					if (destinationStation != null)
					{
						etaToDestination = destinationStation.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + destinationStation.Hrprevc;
					}
					string station = estacaoNotificacao?.Estacao;
					string stationCode = estacaoNotificacao?.Codigo;
					string nexStation = estacaoNotificacao?.Proximaestacao;
					string trainOrentation = comboio?.Sentido;
					MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
					{
						cfg.CreateMap<LocalizacaoVagao, UpdateStationWagonDTO>();
					});
					Mapper mapper = new Mapper(config);
					List<LocalizacaoVagao> localizacaoVagoes = dadosComboio.vagoes;
					List<UpdateStationWagonDTO> updateRequestWagons = mapper.Map<List<UpdateStationWagonDTO>>(localizacaoVagoes);
					string horac = tempo.Horac;
					if (horac != null && horac.Length > 0)
					{
						UStqueue stationQueue = new UStqueue
						{
							UStqueuestamp = 25.UseThisSizeForStamp(),
							Station = station,
							Stationcode = stationCode,
							Destination = dadosComboio.comboio.Destino,
							Coddest = dadosComboio.comboio.Coddest,
							Ord = ordem.Value,
							Etanextst = conversionExtension.ParseToDate(etaNextStation),
							Operationdate = conversionExtension.ParseToDate(tempo.Datac.Date.ToString("yyyy-MM-dd") + " " + tempo.Horac),
							Nextstation = nextStationNotificacao?.Estacao,
							Nextstcode = nextStationNotificacao?.Codigo,
							Etabossa = conversionExtension.ParseToDate(etaToBossa),
							Etadestination = DateTime.Parse(etaToDestination),
							Trainnumber = comboio.Numero,
							Trainreference = comboio.Ref,
							Tempostamp = tempo.TemposStamp,
							Trainorientation = ((trainOrentation == "D") ? "Descending" : "Ascending"),
							Operationtype = "Arrival",
							Wagonsdata = JsonConvert.SerializeObject(updateRequestWagons, Formatting.Indented),
							Entity = dest?.Entidade
						};
						stationQueueData.Add(stationQueue);
					}
					string horap = tempo.Horap;
					if (horap != null && horap.Length > 0)
					{
						UStqueue stationQueue2 = new UStqueue
						{
							UStqueuestamp = 25.UseThisSizeForStamp(),
							Station = station,
							Stationcode = stationCode,
							Destination = dadosComboio.comboio.Destino,
							Coddest = dadosComboio.comboio.Coddest,
							Ord = ordem.Value,
							Tempostamp = tempo.TemposStamp,
							Etanextst = conversionExtension.ParseToDate(etaNextStation),
							Operationdate = conversionExtension.ParseToDate(tempo.Datap.Date.ToString("yyyy-MM-dd") + " " + tempo.Horap),
							Nextstation = nextStationNotificacao?.Estacao,
							Nextstcode = nextStationNotificacao?.Codigo,
							Etabossa = conversionExtension.ParseToDate(etaToBossa),
							Etadestination = DateTime.Parse(etaToDestination),
							Trainnumber = comboio.Numero,
							Trainreference = comboio.Ref,
							Trainorientation = ((trainOrentation == "D") ? "Descending" : "Ascending"),
							Operationtype = "Departure",
							Wagonsdata = JsonConvert.SerializeObject(updateRequestWagons, Formatting.Indented),
							Entity = dest?.Entidade
						};
						stationQueueData.Add(stationQueue2);
					}
				}
			}
		}
		if (entity == "MPDC")
		{
			return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), _MPDCHelper.MapStations(stationQueueData), null);
		}
		return response;
	}

	public LocalizacaoVeiculoManobraDTO GetLocalizacaoVeiculoManobra(LocalizacaoVagaoRequestDTO localizacaoVagaoRequestDTO)
	{
		LocalizacaoVeiculoManobraDTO localizacaoVeiculoManobra = new LocalizacaoVeiculoManobraDTO();
		if (localizacaoVagaoRequestDTO?.ApproveWagonOperationDTO?.requestType == "Fornecimento")
		{
			LinhaVeiculoFerroviario linhaveiculo = _consignacaoRepository.GetLinhaVeiculoByStampVag(localizacaoVagaoRequestDTO?.linhaPlanoManobra?.Stampevg);
			string descprocedencia = "";
			string codprocedencia = "";
			string patio = "";
			string destino = "";
			PatiosManobra dadosPatio = _consignacaoRepository.GetPatioManobraByCodigo(localizacaoVagaoRequestDTO?.planoManobra?.cabecalhoPlanoManobra?.Patio);
			LocaisFerroviarios localFerroviario = _consignacaoRepository.GetLocalFerroviario(localizacaoVagaoRequestDTO?.planoManobra?.cabecalhoPlanoManobra.Codest);
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
			localizacaoVeiculoManobra = new LocalizacaoVeiculoManobraDTO
			{
				codori = codprocedencia,
				origem = descprocedencia,
				coddest = localizacaoVagaoRequestDTO?.planoManobra?.cabecalhoPlanoManobra?.Codest,
				codpatio = localizacaoVagaoRequestDTO?.planoManobra?.cabecalhoPlanoManobra?.Patio,
				destino = destino,
				patio = patio,
				estacao = localizacaoVagaoRequestDTO?.planoManobra?.cabecalhoPlanoManobra?.Estacao,
				codest = localizacaoVagaoRequestDTO?.planoManobra?.cabecalhoPlanoManobra?.Codest
			};
		}
		if (localizacaoVagaoRequestDTO?.ApproveWagonOperationDTO?.requestType == "Retirada")
		{
			CabecalhoPedidoRetirada pedidoRetiradaResult = localizacaoVagaoRequestDTO?.pedidoRetirada;
			List<EntidadeVagao> entidadesVagao = _consignacaoRepository.GetEntidadeVagaoByPedidoRetiradaStamp(pedidoRetiradaResult?.CabecalhoPedidoRetiradaStamp);
			UOridest dadosEstacao = _consignacaoRepository.GetDadosEstacaoByCodigo(pedidoRetiradaResult?.Estacao);
			EntidadeVagao entidadeVagaoParaDadosCabecalho = entidadesVagao.FirstOrDefault();
			PatiosManobra dadosPatio2 = _consignacaoRepository.GetPatioManobraByCodigo(dadosEstacao?.Patio);
			LocaisFerroviarios localFerroviario2 = _consignacaoRepository.GetLocalFerroviario(pedidoRetiradaResult.Desvio);
			string destino2 = "";
			destino2 = localFerroviario2?.Design;
			localizacaoVeiculoManobra = new LocalizacaoVeiculoManobraDTO
			{
				codori = pedidoRetiradaResult.Desvio,
				origem = localFerroviario2?.Design,
				coddest = pedidoRetiradaResult?.Estacao,
				codpatio = dadosPatio2?.Codigo,
				estacao = dadosEstacao?.Cidade,
				codest = pedidoRetiradaResult?.Estacao,
				destino = "",
				patio = dadosPatio2?.Design
			};
		}
		return localizacaoVeiculoManobra;
	}

	public void CorrigirLinhasManobra(List<ApproveWagonOperationDTO> listaApprove)
	{
		foreach (ApproveWagonOperationDTO ApproveWagonOperationDTO in listaApprove)
		{
			List<Manobras> manobras = _consignacaoRepository.GetManobrasByIdAndTipo(ApproveWagonOperationDTO.requestNumber, ApproveWagonOperationDTO.requestType);
			foreach (Manobras dadosManobra in manobras)
			{
				string stampManobra = dadosManobra?.ManobraStamp;
				if (ApproveWagonOperationDTO.requestType.Trim() == "Fornecimento")
				{
					PlanoManobra cabecalhoPlanoManobra = _consignacaoRepository.GetPlanoMabobraByPedidoID(ApproveWagonOperationDTO.requestNumber);
					foreach (LinhasPlanoManobra linha in cabecalhoPlanoManobra?.linhasPlanoManobras)
					{
						LocalizacaoVeiculoManobraDTO localizacaoVeiculoManobra = GetLocalizacaoVeiculoManobra(new LocalizacaoVagaoRequestDTO
						{
							planoManobra = cabecalhoPlanoManobra,
							linhaPlanoManobra = linha,
							ApproveWagonOperationDTO = ApproveWagonOperationDTO,
							entidadeVagao = null,
							pedidoRetirada = null
						});
						EntidadeVagao dadosEntidade = _consignacaoRepository.GetEntidadeVagaoByStamp(linha.Stampevg);
						LinhasManobra dadosLinhaManobra = _consignacaoRepository.GetLinhaManobraByStampManobraAndVeiculo(stampManobra, linha.Novg);
						if (dadosLinhaManobra != null)
						{
							int actualizacaoResult = _SGOFCTX.LinhasManobra.Where((LinhasManobra manobra) => manobra.ManobraStamp == stampManobra && manobra.Novg == linha.Novg).Update((LinhasManobra up) => new LinhasManobra
							{
								Codori = localizacaoVeiculoManobra.codori,
								Origem = localizacaoVeiculoManobra.origem,
								Coddest = localizacaoVeiculoManobra.coddest,
								Destino = localizacaoVeiculoManobra.destino,
								Codest = localizacaoVeiculoManobra.codest,
								Estacao = localizacaoVeiculoManobra.estacao,
								Patio = localizacaoVeiculoManobra.patio,
								Codpatio = localizacaoVeiculoManobra.codpatio
							});
						}
					}
				}
				if (ApproveWagonOperationDTO.requestType.Trim() == "Retirada")
				{
					CabecalhoPedidoRetirada pedidoRetiradaResult = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(ApproveWagonOperationDTO.requestNumber);
					LocalizacaoVeiculoManobraDTO localizacaoVeiculoManobra2 = GetLocalizacaoVeiculoManobra(new LocalizacaoVagaoRequestDTO
					{
						planoManobra = null,
						linhaPlanoManobra = null,
						ApproveWagonOperationDTO = ApproveWagonOperationDTO,
						entidadeVagao = null,
						pedidoRetirada = pedidoRetiradaResult
					});
					_SGOFCTX.LinhasManobra.Where((LinhasManobra manobra) => manobra.ManobraStamp == stampManobra).Update((LinhasManobra up) => new LinhasManobra
					{
						Codori = localizacaoVeiculoManobra2.codori,
						Origem = localizacaoVeiculoManobra2.origem,
						Coddest = localizacaoVeiculoManobra2.coddest,
						Destino = localizacaoVeiculoManobra2.destino,
						Codest = localizacaoVeiculoManobra2.codest,
						Estacao = localizacaoVeiculoManobra2.estacao,
						Patio = localizacaoVeiculoManobra2.patio,
						Codpatio = localizacaoVeiculoManobra2.codpatio
					});
				}
			}
		}
	}

	public void ProcessarTopUp(string topUpStamp, TopUpDTO topUp, List<UTopuplin> linhasTopUp)
	{
		decimal? responseID = logHelper.generateResponseID();
		try
		{
			if (topUp.requestType?.Trim() == "Fornecimento")
			{
				foreach (UTopuplin linha in linhasTopUp)
				{
					PlanoManobra planoManobra = _consignacaoRepository.GetPlanoManobraByPedido(topUp.requestNumber);
					List<LinhasPlanoManobra> linhasPlanoManobra = _consignacaoRepository.GetLinhasPlanoManobrasByPlanoStamp(planoManobra.cabecalhoPlanoManobra.CabecalhoPlanoManobraStamp);
					LinhasPlanoManobra baseLinhaPlanoManobra = linhasPlanoManobra.FirstOrDefault();
					EntidadeVagao dadosEntidade = _consignacaoRepository.GetEntidadeVagaoByStamp(linha.Stampvag);
					Manobras manobra = _consignacaoRepository.GetManobraById(topUp.requestNumber, topUp.requestType);
					List<LinhasManobra> linhasManobra = _consignacaoRepository.GetLinhasManobrasByManobraStamp(manobra.ManobraStamp);
					LinhasManobra baseLinhaManobra = linhasManobra.FirstOrDefault();
					LinhasPlanoManobra linhaPlanoManobra = new LinhasPlanoManobra
					{
						CabecalhoPlanoManobraStamp = baseLinhaPlanoManobra?.CabecalhoPlanoManobraStamp,
						LinhasPlanoManobraStamp = 25.useThisSizeForStamp(),
						Carga = linha?.Catcarga,
						Novg = linha?.Vagno,
						Stampevg = linha.Stampvag,
						Consgno = linha.Consgno,
						Codorig = baseLinhaPlanoManobra?.Codorig,
						Origem = baseLinhaPlanoManobra?.Origem
					};
					List<KeyValuePair<string, object>> conditionsLinhaPlanoManobra = new List<KeyValuePair<string, object>>();
					conditionsLinhaPlanoManobra.Add(new KeyValuePair<string, object>("Novg", linha?.Vagno));
					conditionsLinhaPlanoManobra.Add(new KeyValuePair<string, object>("CabecalhoPlanoManobraStamp", baseLinhaPlanoManobra?.CabecalhoPlanoManobraStamp));
					List<string> keysToExcludeLinhaPlanoManobra = new List<string>();
					keysToExcludeLinhaPlanoManobra.Add("LinhasPlanoManobraStamp");
					_genericRepository.UpsertEntity(linhaPlanoManobra, keysToExcludeLinhaPlanoManobra, conditionsLinhaPlanoManobra, saveChanges: false);
					if (manobra != null)
					{
						LinhasManobra linhaManobra = new LinhasManobra
						{
							ManobraStamp = baseLinhaManobra.ManobraStamp,
							Nrbm = manobra.No.ToString(),
							LinhasManobraStamp = 25.useThisSizeForStamp(),
							Novg = linha.Vagno,
							Stampevg = linha?.Stampvag,
							Ousrdata = DateTime.Now.Date,
							Usrdata = DateTime.Now.Date,
							Ousrhora = DateTime.Now.ToString("HH:mm"),
							Usrhora = DateTime.Now.ToString("HH:mm"),
							Codori = baseLinhaManobra.Codori,
							Origem = baseLinhaManobra.Origem,
							Coddest = baseLinhaManobra.Coddest,
							Codpatio = baseLinhaManobra.Codpatio,
							Destino = baseLinhaManobra.Destino,
							Patio = baseLinhaManobra.Patio,
							Stampveic = dadosEntidade?.Stampveic
						};
						List<KeyValuePair<string, object>> conditionsLinhaManobra = new List<KeyValuePair<string, object>>();
						conditionsLinhaManobra.Add(new KeyValuePair<string, object>("Novg", linha?.Vagno));
						conditionsLinhaManobra.Add(new KeyValuePair<string, object>("ManobraStamp", baseLinhaManobra.ManobraStamp));
						List<string> keysToExcludeLinhaManobra = new List<string>();
						keysToExcludeLinhaManobra.Add("LinhasManobraStamp");
						_genericRepository.UpsertEntity(linhaManobra, keysToExcludeLinhaManobra, conditionsLinhaManobra, saveChanges: false);
					}
				}
				_genericRepository.SaveChanges();
			}
			else
			{
				if (!(topUp.requestType?.Trim() == "Retirada"))
				{
					return;
				}
				foreach (UTopuplin linha2 in linhasTopUp)
				{
					EntidadeVagao dadosEntidade2 = _consignacaoRepository.GetEntidadeVagaoByStamp(linha2.Stampvag);
					Manobras manobra2 = _consignacaoRepository.GetManobraById(topUp.requestNumber, topUp.requestType);
					List<LinhasManobra> linhasManobra2 = _consignacaoRepository.GetLinhasManobrasByManobraStamp(manobra2.ManobraStamp);
					LinhasManobra baseLinhaManobra2 = linhasManobra2.FirstOrDefault();
					CabecalhoPedidoRetirada pedidoRetiradaResult = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(topUp.requestNumber);
					dadosEntidade2.Stamppedret = pedidoRetiradaResult.CabecalhoPedidoRetiradaStamp;
					if (manobra2 != null)
					{
						LinhasManobra linhaManobra2 = new LinhasManobra
						{
							ManobraStamp = baseLinhaManobra2.ManobraStamp,
							Nrbm = manobra2.No.ToString(),
							LinhasManobraStamp = 25.useThisSizeForStamp(),
							Novg = linha2.Vagno,
							Stampevg = linha2?.Stampvag,
							Ousrdata = DateTime.Now.Date,
							Usrdata = DateTime.Now.Date,
							Ousrhora = DateTime.Now.ToString("HH:mm"),
							Usrhora = DateTime.Now.ToString("HH:mm"),
							Codori = baseLinhaManobra2.Codori,
							Origem = baseLinhaManobra2.Origem,
							Coddest = baseLinhaManobra2.Coddest,
							Codpatio = baseLinhaManobra2.Codpatio,
							Destino = baseLinhaManobra2.Destino,
							Patio = baseLinhaManobra2.Patio,
							Stampveic = dadosEntidade2?.Stampveic
						};
						List<KeyValuePair<string, object>> conditionsLinhaManobra2 = new List<KeyValuePair<string, object>>();
						conditionsLinhaManobra2.Add(new KeyValuePair<string, object>("Novg", linha2?.Vagno));
						conditionsLinhaManobra2.Add(new KeyValuePair<string, object>("ManobraStamp", baseLinhaManobra2.ManobraStamp));
						List<string> keysToExcludeLinhaManobra2 = new List<string>();
						keysToExcludeLinhaManobra2.Add("LinhasManobraStamp");
						_genericRepository.UpsertEntity(linhaManobra2, keysToExcludeLinhaManobra2, conditionsLinhaManobra2, saveChanges: false);
					}
				}
				_genericRepository.SaveChanges();
			}
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
			logHelper.generateLogJB(finalResponse, responseID.ToString(), "ConsignmentService.ProcessarTopUp");
		}
	}

	public ResponseDTO ProcessarPedidoFornecimento(WagonSupplyRequestDTO wagonSupplyRequestDTO, string source)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		string planoManobraStamp = 25.useThisSizeForStamp();
		List<WagonRequestResultDTO> suppliedWagonsResult = new List<WagonRequestResultDTO>();
		List<WagonRequestResultDTO> vagoesFornecidosResult = new List<WagonRequestResultDTO>();
		List<VeiculosPorRegularizar> vagoesPorRegularizar = new List<VeiculosPorRegularizar>();
		PlanoManobra planoManobraResult = _consignacaoRepository.GetPlanoMabobraByPedidoID(wagonSupplyRequestDTO.mpdcRequestForSupplyNumber);
		DateTime submitedAt = conversionExtension.StringToDateTime(planoManobraResult?.cabecalhoPlanoManobra?.Ousrdata.Value.ToString("yyyy-MM-dd") + " " + planoManobraResult?.cabecalhoPlanoManobra?.Ousrhora);
		string boletim = "";
		Manobras manobraData = _consignacaoRepository.GetManobra(wagonSupplyRequestDTO?.mpdcRequestForSupplyNumber?.Trim());
		if (manobraData != null)
		{
			boletim = manobraData?.No;
		}
		WagonSupplyRequestResponseDTO supplyResult = new WagonSupplyRequestResponseDTO
		{
			mpdcRequestForSupplyNumber = wagonSupplyRequestDTO?.mpdcRequestForSupplyNumber,
			SubmittedAt = submitedAt,
			bolletin = boletim
		};
		if (wagonSupplyRequestDTO.isTopUp)
		{
			bool processed = true;
			string errorMessage = "";
			string topUpstamp = 25.UseThisSizeForStamp();
			UTopup topUp = new UTopup
			{
				UTopupstamp = topUpstamp,
				Pedidoid = wagonSupplyRequestDTO.mpdcRequestForSupplyNumber,
				Data = DateTime.Now,
				Tipo = "Fornecimento",
				Aprovado = false
			};
			UTopup topupExistente = _consignacaoRepository.GetTopUpByPedidoAndTipo(wagonSupplyRequestDTO.mpdcRequestForSupplyNumber, "Fornecimento");
			if (topupExistente != null)
			{
				topUpstamp = topupExistente.UTopupstamp;
			}
			List<KeyValuePair<string, object>> conditionsTopUp = new List<KeyValuePair<string, object>>();
			conditionsTopUp.Add(new KeyValuePair<string, object>("Pedidoid", wagonSupplyRequestDTO.mpdcRequestForSupplyNumber));
			conditionsTopUp.Add(new KeyValuePair<string, object>("Tipo", "Fornecimento"));
			List<string> keysToExcludeTopUp = new List<string>();
			keysToExcludeTopUp.Add("UTopupstamp");
			_genericRepository.UpsertEntity(topUp, keysToExcludeTopUp, conditionsTopUp, saveChanges: false);
			foreach (WagonRequestDTO wagonData in wagonSupplyRequestDTO.wagons)
			{
				processed = true;
				errorMessage = "";
				VagaoVeiculoFerroviarioDTO entidadeVagaoResult = _consignacaoRepository.GetEntidadeVagao(wagonData.wagonNumber, wagonData.consignmentNumber);
				if (entidadeVagaoResult == null)
				{
					return new ResponseDTO(new ResponseCodesDTO("0404", "Wagon " + wagonData?.wagonNumber + " not found", responseID), null, null);
				}
				suppliedWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData.wagonNumber,
					consignmentNumber = wagonData.consignmentNumber,
					processed = true,
					errorMessage = ""
				});
				UTopuplin topUpLin = new UTopuplin
				{
					UTopuplinstamp = 25.UseThisSizeForStamp(),
					UTopupstamp = topUpstamp,
					Vagno = wagonData.wagonNumber,
					Carga = wagonData.commodity,
					Catcarga = wagonData.commodityCategory,
					Peso = wagonData.weight.Value,
					Consgno = wagonData.consignmentNumber,
					Local = entidadeVagaoResult?.entidadeVagao?.Local,
					Desiglocal = entidadeVagaoResult?.entidadeVagao?.Desiglocal,
					Estado = wagonData.wagonStatus
				};
				List<KeyValuePair<string, object>> conditionsTopUpLinha = new List<KeyValuePair<string, object>>();
				conditionsTopUpLinha.Add(new KeyValuePair<string, object>("Vagno", wagonData.wagonNumber));
				conditionsTopUpLinha.Add(new KeyValuePair<string, object>("UTopupstamp", topUpstamp));
				List<string> keysToExcludeTopUpLinha = new List<string>();
				keysToExcludeTopUpLinha.Add("UTopuplinstamp");
				_genericRepository.UpsertEntity(topUpLin, keysToExcludeTopUpLinha, conditionsTopUpLinha, saveChanges: false);
			}
			_genericRepository.SaveChanges();
			supplyResult.wagons = suppliedWagonsResult;
			return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), supplyResult, null);
		}
		if (planoManobraResult != null)
		{
			if (planoManobraResult != null && planoManobraResult.cabecalhoPlanoManobra?.Aprovado == true && wagonSupplyRequestDTO != null && wagonSupplyRequestDTO.requireApproval)
			{
				return new ResponseDTO(new ResponseCodesDTO("0400", "the request has already been approved", responseID), null, null);
			}
			planoManobraStamp = planoManobraResult?.cabecalhoPlanoManobra?.CabecalhoPlanoManobraStamp;
			List<VeiculosPorRegularizar> veiculosPorRegularizarExistentes = _consignacaoRepository.GetVeiculosPorRegularizarByPedidoStamp(planoManobraStamp);
			_genericRepository.BulkDelete(planoManobraResult?.linhasPlanoManobras);
			_genericRepository.BulkDelete(veiculosPorRegularizarExistentes);
		}
		List<ComboioRegisto> comboios = new List<ComboioRegisto>();
		LocationDTO dadosEstacao = locationHelper.HandleStationWithdrawalHelper(new LocationDTO
		{
			Code = wagonSupplyRequestDTO.stationCode,
			Designation = wagonSupplyRequestDTO.station
		}, source);
		CabecalhoPlanoManobra wagonRequestSupply = new CabecalhoPlanoManobra
		{
			CabecalhoPlanoManobraStamp = planoManobraStamp,
			Datasub = wagonSupplyRequestDTO?.firstSubmissionTentative.Date,
			Horasub = wagonSupplyRequestDTO?.firstSubmissionTentative.ToString("HH:mm:ss"),
			Estacao = wagonSupplyRequestDTO?.stationCode,
			Codest = wagonSupplyRequestDTO?.stationCode,
			Data = wagonSupplyRequestDTO.expectedSupplyDate,
			PedidoId = wagonSupplyRequestDTO.mpdcRequestForSupplyNumber,
			Tipo = "Fornecimento",
			Aprovado = ((wagonSupplyRequestDTO != null) ? new bool?(!wagonSupplyRequestDTO.requireApproval) : null),
			Dataaprovacao = ((wagonSupplyRequestDTO != null && !wagonSupplyRequestDTO.requireApproval) ? DateTime.Now : new DateTime(1900, 1, 1)),
			Horaaprovacao = ((wagonSupplyRequestDTO != null && !wagonSupplyRequestDTO.requireApproval) ? DateTime.Now.ToString("HH:mm") : "00:00"),
			Verificar = wagonSupplyRequestDTO?.requireApproval,
			Automatica = ((wagonSupplyRequestDTO != null) ? new bool?(!wagonSupplyRequestDTO.requireApproval) : null)
		};
		List<LinhasPlanoManobra> linhasPlanoManobras = new List<LinhasPlanoManobra>();
		foreach (WagonRequestDTO wagonData2 in wagonSupplyRequestDTO.wagons)
		{
			bool processed2 = true;
			string errorMessage2 = "";
			VagaoVeiculoFerroviarioDTO entidadeVagaoResult2 = _consignacaoRepository.GetEntidadeVagao(wagonData2.wagonNumber, wagonData2.consignmentNumber);
			if (entidadeVagaoResult2 == null)
			{
				processed2 = false;
				errorMessage2 = "Wagon " + wagonData2?.wagonNumber + " not found";
				suppliedWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData2.wagonNumber,
					consignmentNumber = wagonData2.consignmentNumber,
					processed = false,
					errorMessage = errorMessage2
				});
				vagoesPorRegularizar.Add(new VeiculosPorRegularizar
				{
					Novg = wagonData2.wagonNumber,
					VeiculosPorRegularizarstamp = 25.UseThisSizeForStamp(),
					Stamppedido = planoManobraStamp,
					Tipo = "Fornecimento",
					Descerr = "O vagão " + wagonData2?.wagonNumber + " não foi encontrado"
				});
			}
			if (entidadeVagaoResult2 != null && entidadeVagaoResult2.entidadeVagao?.Desiglocal.Contains("COMBOIO") == true)
			{
				ResponseDTO wagoOnTrain = new ResponseDTO(new ResponseCodesDTO("0405", "The Wagon " + wagonData2?.wagonNumber + " is still attached to the  train", responseID), null, null);
				processed2 = false;
				errorMessage2 = "The Wagon " + wagonData2?.wagonNumber + " is still attached to the  train";
				suppliedWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData2.wagonNumber,
					consignmentNumber = wagonData2.consignmentNumber,
					processed = false,
					errorMessage = errorMessage2
				});
				vagoesPorRegularizar.Add(new VeiculosPorRegularizar
				{
					Novg = wagonData2.wagonNumber,
					VeiculosPorRegularizarstamp = 25.UseThisSizeForStamp(),
					Stamppedido = planoManobraStamp,
					Tipo = "Fornecimento",
					Descerr = "O vagão " + wagonData2?.wagonNumber + " ainda está em comboio "
				});
			}
			if (processed2)
			{
				ComboioRegisto comboioVagao = _consignacaoRepository.GetComboioVagao(entidadeVagaoResult2?.entidadeVagao?.EntidadeVagaostamp);
				if (comboioVagao != null)
				{
					comboios.Add(comboioVagao);
				}
				LinhasPlanoManobra linhaPlanoManobra = new LinhasPlanoManobra
				{
					CabecalhoPlanoManobraStamp = planoManobraStamp,
					LinhasPlanoManobraStamp = 25.useThisSizeForStamp(),
					Carga = wagonData2?.commodityCategory,
					Novg = wagonData2?.wagonNumber,
					Stampevg = entidadeVagaoResult2?.entidadeVagao?.EntidadeVagaostamp,
					Consgno = wagonData2?.consignmentNumber,
					Codorig = entidadeVagaoResult2?.entidadeVagao.Codest,
					Origem = entidadeVagaoResult2?.entidadeVagao.Desiglocal
				};
				linhaPlanoManobra.AssignDefaultEntityValues();
				ComboioRegisto firstComboio = comboios.OrderByDescending((ComboioRegisto c) => c.Data).FirstOrDefault();
				DesengateComboio desengateComboio = _consignacaoRepository.GetDesengateComboioByComboioStamp(firstComboio?.ComboioRegistoStamp);
				string codprocedencia = "";
				string descprocedencia = "";
				if (desengateComboio != null)
				{
					codprocedencia = desengateComboio.Local;
					descprocedencia = desengateComboio.Desiglocal;
				}
				wagonRequestSupply.Estacao = codprocedencia;
				linhasPlanoManobras.Add(linhaPlanoManobra);
				suppliedWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData2.wagonNumber,
					consignmentNumber = wagonData2.consignmentNumber,
					processed = true,
					errorMessage = ""
				});
			}
		}
		supplyResult.wagons = suppliedWagonsResult;
		List<WagonRequestResultDTO> vagoesComErro = vagoesFornecidosResult;
		_genericRepository.BulkAdd(vagoesPorRegularizar);
		_genericRepository.UpsertEntity(wagonRequestSupply, new List<string> { "PedidoId", "CabecalhoPlanoManobraStamp" }, new List<KeyValuePair<string, object>>
		{
			new KeyValuePair<string, object>("PedidoId", wagonSupplyRequestDTO.mpdcRequestForSupplyNumber)
		}, saveChanges: false);
		_genericRepository.BulkAdd(linhasPlanoManobras);
		_genericRepository.SaveChanges();
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), supplyResult, null);
	}

	public Dossier ProcessarConsignacao(Dossier dossier, string admin)
	{
		try
		{
			Dossier consignacao = _consignacaoRepository.getConsignacao(dossier?.bo3?.UConsgno);
			if (consignacao == null)
			{
				dossier.novo = true;
				_consignacaoRepository.criarConsignacao(dossier);
				dossier.processamentoResponse = new ResponseCodesDTO("0000", "Success");
				return dossier;
			}
			dossier.novo = false;
			dossier.bo.Bostamp = consignacao?.bo3?.Bo3stamp;
			List<SGOFWS.Domains.Models.Bi2> vagoes = dossier.bi2;
			foreach (SGOFWS.Domains.Models.Bi2 vagao in vagoes)
			{
				vagao.Bostamp = consignacao?.bo3?.Bo3stamp;
				SGOFWS.Domains.Models.Bi2 vagaoAlterado = _consignacaoRepository.getVagaoActualizado(consignacao?.bo3.Bo3stamp, vagao.UVagno, vagao.UUltdtrep, vagao.UStatus);
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
					_consignacaoRepository.actualizarVagao(vagao);
				}
				_consignacaoRepository.inserirVagaoSeNaoExiste(vagao, consignacao?.bo3?.Bo3stamp, consignacao);
			}
			_consignacaoRepository.actualizarTotalVeiculos(dossier.bo3.UTotveicl, consignacao?.bo3.Bo3stamp);
			dossier.processamentoResponse = new ResponseCodesDTO("0000", "Success");
			return dossier;
		}
		catch (Exception ex)
		{
			return new Dossier
			{
				processamentoResponse = new ResponseCodesDTO("0007", ex?.Message + ex.InnerException?.Message + ex?.StackTrace)
			};
		}
	}

	public void SincronzarGuias()
	{
		decimal? requestID = logHelper.generateResponseID();
		try
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			DateTime dataExecucao = DateTime.Now;
			DateTime dataobra = DateTime.Parse("2024-04-22T06:21:30.769Z");
			List<decimal> ndosList = new List<decimal> { 140m, 141m, 142m, 143m, 262m, 261m };
			List<GuiaMercadoria> guias = _cfmMAinBDRepository.GetGuiasNaoSincronizadas(dataobra, ndosList);
			ConfiguracaoDossier configuracaoDossier = configuracaoHelper.getConfiguracaoDossier("CONSIGNACAO");
			string formattedNumber = 482.ToString("D4");
			foreach (GuiaMercadoria guia in guias)
			{
				List<UBovg> vagoesGuia = _cfmMAinBDRepository.GetVagoesGuiaByGuiaStamP(guia?.bo?.Bostamp);
				if (vagoesGuia.Count() > 0)
				{
					SGOFWS.Domains.Models.Cl dadosCliente = _consignacaoRepository.GetClienteByNome(guia?.bo?.Nome);
					Fluxo dadosFluxo = _consignacaoRepository.GetFluxoByCodigo(guia?.bo2?.UFluxo);
					string dossierStamp = 25.UseThisSizeForStamp();
					string consgno = guia?.bo?.Ndos.ToString() + "-" + ((int)(guia?.bo?.Obrano).Value).ToString("D4") + "-" + guia?.bo?.Boano.ToString();
					if ((dadosFluxo?.Trafego?.Trim() == "Nacional" && dadosFluxo?.Sentido?.Trim() == "D") || (dadosFluxo?.Trafego?.Trim() == "Internacional" && dadosFluxo?.Sentido?.Trim() == "A") || (dadosFluxo?.Trafego?.Trim() == "Nacional" && dadosFluxo?.Sentido?.Trim() == "A"))
					{
						Dossier dossier = new Dossier
						{
							bo = new SGOFWS.Domains.Models.Bo
							{
								Bostamp = dossierStamp
							},
							bo2 = new SGOFWS.Domains.Models.Bo2
							{
								Bo2stamp = dossierStamp
							},
							bo3 = new SGOFWS.Domains.Models.Bo3
							{
								Bo3stamp = dossierStamp,
								UFluxocom = guia?.bo2?.UFluxo,
								UFluxotec = guia?.bo2?.UFluxo,
								UConsgtip = "BULK",
								UConsgno = consgno,
								UStrackin = "LRE06",
								UFccodstd = dadosFluxo?.Coddeste,
								UFcstdest = dadosFluxo?.Destdesig,
								UCodstcag = dadosFluxo?.Codorige,
								UStcarrg = dadosFluxo?.Origdesig,
								UCoddesvg = dadosFluxo?.Origdesv,
								UExped = dadosFluxo?.Expd,
								UConsgtno = dadosFluxo?.Consig,
								UCodstdet = dadosFluxo?.Coddeste,
								UStdest = dadosFluxo?.Destdesig,
								UCoddesvt = dadosFluxo?.Destdesv,
								UTotveicl = vagoesGuia.Count()
							}
						};
						decimal ndos = configuracaoDossier.ndos;
						UBovg vagaoGuiaFirst = vagoesGuia.FirstOrDefault();
						EntidadeVagao entidadeVagaoFirst = _consignacaoRepository.GetEntidadeVagaoByStamp(vagaoGuiaFirst?.Stampevg);
						string nmdos = "Consignação";
						string clnome = dadosCliente?.Nome;
						decimal? no = dadosCliente?.No;
						decimal? obrano = _consignacaoRepository.getMaxObrano(ndos);
						string admin = entidadeVagaoFirst?.Admin;
						dossier.bo.Ndos = ndos;
						dossier.bo.Obrano = obrano;
						dossier.bo.Nmdos = nmdos;
						dossier.bo.Nome = clnome;
						dossier.bo.No = no;
						dossier.bo3.UAdmin = admin;
						List<SGOFWS.Domains.Models.Bi> bis = new List<SGOFWS.Domains.Models.Bi>();
						List<SGOFWS.Domains.Models.Bi2> bi2s = new List<SGOFWS.Domains.Models.Bi2>();
						foreach (UBovg vagao in vagoesGuia)
						{
							EntidadeVagao entidadeVagao = _consignacaoRepository.GetEntidadeVagaoByStamp(vagao?.Stampevg);
							UOridest dadosEstacao = _consignacaoRepository.GetDadosEstacaoByCodigo(entidadeVagao?.Estacao);
							SGOFWS.Domains.Models.Bi bi = new SGOFWS.Domains.Models.Bi
							{
								Bostamp = dossierStamp
							};
							SGOFWS.Domains.Models.Bi2 bi2 = new SGOFWS.Domains.Models.Bi2
							{
								UDatcarrg = "",
								UHorcarrg = "",
								UVagno = vagao.Novg,
								UAdmintr = admin,
								UVagtip = vagao.Tipo,
								UVagdesc = "",
								UVagcod = "",
								UDualcon = false,
								UContdcod = dadosFluxo?.Codcarga,
								UCntdesc = vagao?.Carga,
								UDsccmcag = vagao?.Carga,
								UPeso = vagao.Qtd,
								UEncerrno = vagao?.Encerno,
								UEncrradm = admin,
								UComboio = "",
								UProxst = "",
								UCodactst = conversionExtension.nullToString(entidadeVagao?.Codest),
								UStact = conversionExtension.nullToString(dadosEstacao?.Cidade),
								UFullempy = true,
								UUltdtrep = "",
								UStatus = "",
								UEta = new DateTime(1900, 1, 1).ToString(),
								UEtafrt = new DateTime(1900, 1, 1).ToString(),
								UTotcont = default(decimal),
								UUltmtmst = DateTime.Now.ToString(),
								Usrhora = DateTime.Now.ToString("HH:mm"),
								Usrdata = DateTime.Now,
								Ousrdata = DateTime.Now,
								Ousrhora = DateTime.Now.ToString("HH:mm")
							};
							bis.Add(bi);
							bi2s.Add(bi2);
						}
						dossier.bi2 = bi2s;
						dossier.bi = bis;
						ProcessarConsignacao(dossier, admin);
					}
				}
				int approveResult = _cFMMAINCONTEXT.Bo3.Where((SGOFWS.Domains.Models.CfmMain.Bo3 bo) => bo.Bo3stamp == guia.bo3.Bo3stamp).Update((SGOFWS.Domains.Models.CfmMain.Bo3 cabupdt) => new SGOFWS.Domains.Models.CfmMain.Bo3
				{
					UGsync = true
				});
			}
			watch.Stop();
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
			logHelper.generateLogJB(finalResponse, requestID.ToString(), "ConsignmentService.SincronzarGuias");
		}
	}

	public DateTime parseShuntingDate(string date)
	{
		try
		{
			return DateTime.Parse(date);
		}
		catch (Exception)
		{
			return new DateTime(1900, 1, 1);
		}
	}

	public ResponseDTO GetManobrasByRange(DateTime startDate, DateTime endDate)
	{
		decimal? responseID = logHelper.generateResponseID();
		List<ManobraELinha> manobras = _consignacaoRepository.GetManobrasByRange(startDate, endDate);
		List<ShuntingDTO> groupedManobras = (from g in (from m in manobras
				group m by new
				{
					m.manobra.ManobraStamp,
					m.manobra.Pedidoid,
					m.manobra.Dataini,
					m.manobra.No,
					m.manobra.Datafim,
					m.manobra.Tipo,
					m.manobra.Horaini,
					m.manobra.Horafim,
					m.manobra.Usrinis
				}).Select(@group =>
			{
				ShuntingDTO obj = new ShuntingDTO
				{
					wagonsRequestId = @group.Key.Pedidoid,
					requestType = @group.Key.Tipo
				};
				string usrinis = @group.Key.Usrinis;
				obj.operatorName = ((usrinis != null && usrinis.Length == 0) ? "ND" : _consignacaoRepository.GetUsByInicias(@group.Key.Usrinis));
				obj.shuntingId = @group.Key.No;
				obj.shuntingStartDate = @group.Key.Dataini.Value.Date.ToString("yyyy-MM-dd") + " " + @group.Key.Horaini;
				obj.shuntingEndDate = @group.Key.Datafim.Value.Date.ToString("yyyy-MM-dd") + " " + @group.Key.Horafim;
				obj.wagons = @group.Select((ManobraELinha gp) => gp.linhaManobra.Novg).ToList();
				return obj;
			})
			orderby decimal.Parse(g.wagonsRequestId) descending
			select g).ToList();
		groupedManobras.ConvertAll(delegate(ShuntingDTO grouped)
		{
			DateTime approvalDate = new DateTime(1900, 1, 1);
			string text = "00:00";
			string aprovedBy = "";
			if (grouped.requestType == "Fornecimento")
			{
				PlanoManobra planoMabobraByPedidoID = _consignacaoRepository.GetPlanoMabobraByPedidoID(grouped.wagonsRequestId);
				if (planoMabobraByPedidoID != null)
				{
					approvalDate = DateTime.Parse(planoMabobraByPedidoID.cabecalhoPlanoManobra.Dataaprovacao.ToString("yyyy-MM-dd") + " " + planoMabobraByPedidoID.cabecalhoPlanoManobra.Horaaprovacao);
					aprovedBy = _consignacaoRepository.GetUsByInicias(planoMabobraByPedidoID.cabecalhoPlanoManobra.Usrinis);
				}
			}
			if (grouped.requestType == "Retirada")
			{
				CabecalhoPedidoRetirada cabecalhoPedidoRetiradaByPedidoId = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(grouped.wagonsRequestId);
				if (cabecalhoPedidoRetiradaByPedidoId != null)
				{
					approvalDate = DateTime.Parse(cabecalhoPedidoRetiradaByPedidoId.Dataaprovacao.Value.ToString("yyyy-MM-dd") + " " + cabecalhoPedidoRetiradaByPedidoId.Horaaprovacao);
					aprovedBy = _consignacaoRepository.GetUsByInicias(cabecalhoPedidoRetiradaByPedidoId.Usrinis);
				}
			}
			grouped.requestType = ((grouped.requestType == "Fornecimento") ? "Supply" : "Withdrawal");
			grouped.approvalDate = approvalDate;
			grouped.aprovedBy = aprovedBy;
			return grouped;
		});
		List<ShuntingDTO> shuntings = new List<ShuntingDTO>();
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), groupedManobras, null);
	}

	public void FinalizarManobra(ShuntingDTO shuntingDTO, string entidade)
	{
		Manobras manobra = _consignacaoRepository.GetManobra(shuntingDTO?.wagonsRequestId);
		List<EntidadeVagao> entidades = _consignacaoRepository.GetLinhasManobraById(shuntingDTO?.wagonsRequestId);
		UOridest dadosEstacao = _consignacaoRepository.GetDadosEstacaoByCodigo(manobra?.Estacao);
		LocaisFerroviarios localFerroviario = _consignacaoRepository.GetLocalFerroviario(manobra?.Destino);
		foreach (EntidadeVagao linha in entidades)
		{
			LocalizacaoVagaoDTO localizacao = new LocalizacaoVagaoDTO
			{
				codest = manobra?.Estacao,
				estacao = dadosEstacao?.Cidade,
				vagao = linha.No,
				consgno = ((manobra?.Tipo?.Trim() == "Fornecimento") ? linha.Consgno : linha.Consorig),
				dataOperacao = DateTime.Parse(shuntingDTO.shuntingEndDate),
				proximaEstacao = localFerroviario?.Designestac,
				etaProximaEstacao = DateTime.Parse(shuntingDTO.shuntingEndDate).ToString(),
				estado = ((manobra?.Tipo?.Trim() == "Fornecimento") ? ("Fornecido à(ao) " + entidade) : ("Retirado do(a) " + entidade))
			};
			ActualizarLocalizacaoVagao(localizacao);
		}
		_SGOFCTX.SaveChanges();
	}

	public ResponseDTO ProcessarPedidoRetirada(WagonWithdrawalResquestDTO wagonWithdrawalRequestDTO, string source)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), null, null);
		string pedidoRetiradaStamp = 25.useThisSizeForStamp();
		CabecalhoPlanoManobra pedidoFornecimento = _consignacaoRepository.GetPlanoMabobraByPedidoIDAndTipo(wagonWithdrawalRequestDTO.requestNumber, "Fornecimento");
		string numeroSequencialPedidoRetirada = GerarNumeroPedidoRetirada(DateTime.Now);
		List<WagonRequestResultDTO> withdrawalWagonsResult = new List<WagonRequestResultDTO>();
		List<WagonRequestResultDTO> vagoesRetiradadosResult = new List<WagonRequestResultDTO>();
		List<VeiculosPorRegularizar> vagoesPorRegularizar = new List<VeiculosPorRegularizar>();
		CabecalhoPedidoRetirada pedidoRetiradaResult = _consignacaoRepository.GetCabecalhoPedidoRetiradaByPedidoId(wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber);
		DateTime submitedAt = conversionExtension.StringToDateTime(pedidoRetiradaResult?.Ousrdata.Value.ToString("yyyy-MM-dd") + " " + pedidoRetiradaResult?.Ousrhora);
		string boletim = "";
		Manobras manobraData = _consignacaoRepository.GetManobra(wagonWithdrawalRequestDTO?.mpdcRequestForWithdrawalNumber?.Trim());
		if (manobraData != null)
		{
			boletim = manobraData?.No;
		}
		WagonWithdrawalResponseDTO withdrawalResult = new WagonWithdrawalResponseDTO
		{
			mpdcRequestForWithdrawalNumber = wagonWithdrawalRequestDTO?.mpdcRequestForWithdrawalNumber,
			SubmittedAt = submitedAt,
			bolletin = boletim
		};
		if (pedidoFornecimento == null)
		{
			return new ResponseDTO(new ResponseCodesDTO("0404", "Request " + wagonWithdrawalRequestDTO.requestNumber + " not found", responseID), null, null);
		}
		if (wagonWithdrawalRequestDTO.isTopUp)
		{
			string topUpstamp = 25.UseThisSizeForStamp();
			UTopup topUp = new UTopup
			{
				UTopupstamp = topUpstamp,
				Pedidoid = wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber,
				Data = DateTime.Now,
				Tipo = "Retirada",
				Aprovado = false
			};
			UTopup topupExistente = _consignacaoRepository.GetTopUpByPedidoAndTipo(wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber, "Retirada");
			if (topupExistente != null)
			{
				topUpstamp = topupExistente.UTopupstamp;
			}
			List<KeyValuePair<string, object>> conditionsTopUp = new List<KeyValuePair<string, object>>();
			conditionsTopUp.Add(new KeyValuePair<string, object>("Pedidoid", wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber));
			conditionsTopUp.Add(new KeyValuePair<string, object>("Tipo", "Retirada"));
			List<string> keysToExcludeTopUp = new List<string>();
			keysToExcludeTopUp.Add("UTopupstamp");
			_genericRepository.UpsertEntity(topUp, keysToExcludeTopUp, conditionsTopUp, saveChanges: false);
			foreach (WagonRequestDTO wagonData in wagonWithdrawalRequestDTO.wagons)
			{
				VagaoVeiculoFerroviarioDTO entidadeVagaoResult = _consignacaoRepository.GetEntidadeVagao(wagonData.wagonNumber, wagonData.consignmentNumber);
				if (entidadeVagaoResult == null)
				{
					return new ResponseDTO(new ResponseCodesDTO("0404", "Wagon " + wagonData?.wagonNumber + " not found", responseID), null, null);
				}
				withdrawalWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData.wagonNumber,
					consignmentNumber = wagonData.consignmentNumber,
					processed = true,
					errorMessage = ""
				});
				UTopuplin topUpLin = new UTopuplin
				{
					UTopuplinstamp = 25.UseThisSizeForStamp(),
					UTopupstamp = topUpstamp,
					Vagno = wagonData.wagonNumber,
					Carga = wagonData.commodity,
					Catcarga = wagonData.commodityCategory,
					Peso = wagonData.weight.Value,
					Consgno = wagonData.consignmentNumber,
					Local = entidadeVagaoResult?.entidadeVagao?.Local,
					Desiglocal = entidadeVagaoResult?.entidadeVagao?.Desiglocal,
					Estado = wagonData.wagonStatus
				};
				List<KeyValuePair<string, object>> conditionsTopUpLinha = new List<KeyValuePair<string, object>>();
				conditionsTopUpLinha.Add(new KeyValuePair<string, object>("Vagno", wagonData.wagonNumber));
				conditionsTopUpLinha.Add(new KeyValuePair<string, object>("UTopupstamp", topUpstamp));
				List<string> keysToExcludeTopUpLinha = new List<string>();
				keysToExcludeTopUpLinha.Add("UTopuplinstamp");
				_genericRepository.UpsertEntity(topUpLin, keysToExcludeTopUpLinha, conditionsTopUpLinha, saveChanges: false);
			}
			_genericRepository.SaveChanges();
			withdrawalResult.wagons = withdrawalWagonsResult;
			return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), withdrawalResult, null);
		}
		if (pedidoRetiradaResult != null)
		{
			if (pedidoRetiradaResult != null && pedidoRetiradaResult.Aprovado == true && wagonWithdrawalRequestDTO.requireApproval)
			{
				return new ResponseDTO(new ResponseCodesDTO("0400", "the request has already been approved", responseID), null, null);
			}
			pedidoRetiradaStamp = pedidoRetiradaResult?.CabecalhoPedidoRetiradaStamp;
			numeroSequencialPedidoRetirada = pedidoRetiradaResult?.Nrped;
			List<VeiculosPorRegularizar> veiculosPorRegularizarExistentes = _consignacaoRepository.GetVeiculosPorRegularizarByPedidoStamp(pedidoRetiradaStamp);
			_genericRepository.BulkDelete(veiculosPorRegularizarExistentes);
		}
		LocationDTO dadosEstacao = locationHelper.HandleStationWithdrawalHelper(new LocationDTO
		{
			Code = wagonWithdrawalRequestDTO.stationCode,
			Designation = wagonWithdrawalRequestDTO.station
		}, source);
		List<WagonRequestDTO> wagons = wagonWithdrawalRequestDTO.wagons;
		if (wagons != null && wagons.Count() == 0)
		{
			return new ResponseDTO(new ResponseCodesDTO("0400", "Unable to submit request " + wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber + " with no wagons", responseID), null, null);
		}
		CabecalhoPedidoRetirada wagonRequestWithdrawal = new CabecalhoPedidoRetirada
		{
			Estacao = wagonWithdrawalRequestDTO?.stationCode,
			CabecalhoPedidoRetiradaStamp = pedidoRetiradaStamp,
			Data = wagonWithdrawalRequestDTO?.firstSubmissionTentative.Date,
			Pedidoid = wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber,
			Nrped = numeroSequencialPedidoRetirada,
			Pedidoforn = wagonWithdrawalRequestDTO.requestNumber,
			Hora = wagonWithdrawalRequestDTO?.firstSubmissionTentative.ToString("HH:mm"),
			Datasub = wagonWithdrawalRequestDTO?.firstSubmissionTentative,
			Horasub = wagonWithdrawalRequestDTO?.firstSubmissionTentative.ToString("HH:mm"),
			Aprovado = ((wagonWithdrawalRequestDTO != null) ? new bool?(!wagonWithdrawalRequestDTO.requireApproval) : null),
			Dataaprovacao = ((wagonWithdrawalRequestDTO != null && !wagonWithdrawalRequestDTO.requireApproval) ? DateTime.Now : new DateTime(1900, 1, 1)),
			Horaaprovacao = ((wagonWithdrawalRequestDTO != null && !wagonWithdrawalRequestDTO.requireApproval) ? DateTime.Now.ToString("HH:mm") : "00:00"),
			Verificar = wagonWithdrawalRequestDTO?.requireApproval,
			Automatica = ((wagonWithdrawalRequestDTO != null) ? new bool?(!wagonWithdrawalRequestDTO.requireApproval) : null)
		};
		_genericRepository.UpsertEntity(wagonRequestWithdrawal, new List<string> { "Pedidoid", "CabecalhoPedidoRetiradaStamp" }, new List<KeyValuePair<string, object>>
		{
			new KeyValuePair<string, object>("Pedidoid", wagonWithdrawalRequestDTO.mpdcRequestForWithdrawalNumber?.Trim())
		}, saveChanges: false);
		foreach (WagonRequestDTO wagonData2 in wagonWithdrawalRequestDTO.wagons)
		{
			bool processed = true;
			string errorMessage = "";
			VagaoVeiculoFerroviarioDTO entidadeVagaoResult2 = _consignacaoRepository.GetEntidadeVagao(wagonData2.wagonNumber, wagonData2.consignmentNumber);
			if (entidadeVagaoResult2 == null)
			{
				processed = false;
				errorMessage = "Wagon " + wagonData2?.wagonNumber + " not found";
				withdrawalWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData2.wagonNumber,
					consignmentNumber = wagonData2.consignmentNumber,
					processed = false,
					errorMessage = errorMessage
				});
				vagoesPorRegularizar.Add(new VeiculosPorRegularizar
				{
					Novg = wagonData2.wagonNumber,
					VeiculosPorRegularizarstamp = 25.UseThisSizeForStamp(),
					Stamppedido = pedidoRetiradaStamp,
					Tipo = "Retirada",
					Descerr = "O vagão " + wagonData2?.wagonNumber + " não foi encontrado"
				});
			}
			AdmnistracaoVizinha dadosAdmnistracao = _consignacaoRepository.GetAdmnistracaoVizinhaVeiculoVazioByno(wagonData2?.wagonNumber?.Trim());
			string fluxoVagao = "";
			string estadoVagao = "Vazio";
			string carga = "";
			if (dadosAdmnistracao == null)
			{
				processed = false;
				errorMessage = "Admnistration for wagon: " + wagonData2?.wagonNumber + " not found";
				withdrawalWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData2.wagonNumber,
					consignmentNumber = wagonData2.consignmentNumber,
					processed = false,
					errorMessage = errorMessage
				});
				vagoesPorRegularizar.Add(new VeiculosPorRegularizar
				{
					Novg = wagonData2.wagonNumber,
					VeiculosPorRegularizarstamp = 25.UseThisSizeForStamp(),
					Stamppedido = pedidoRetiradaStamp,
					Tipo = "Retirada",
					Descerr = "Adminstração para o vagão " + wagonData2?.wagonNumber + " não foi encontrada."
				});
			}
			if (processed)
			{
				fluxoVagao = dadosAdmnistracao?.Fluxov;
				Fluxo dadosFluxo = _consignacaoRepository.GetFluxoByCodigo(dadosAdmnistracao.Fluxov);
				carga = dadosFluxo?.Carga;
				VeiculoFerroviario dadosVeiculo = _consignacaoRepository.GetVeiculoFerroviarioByNo(wagonData2?.wagonNumber?.Trim());
				if (dadosVeiculo == null)
				{
					return new ResponseDTO(new ResponseCodesDTO("0404", "Vehicle " + wagonData2?.wagonNumber + " not found", responseID), null, null);
				}
				LocationDTO dadosLocal = locationHelper.HandleStationWithdrawalHelper(new LocationDTO
				{
					Code = wagonData2.location,
					Designation = wagonData2.locationDesign
				}, source);
				entidadeVagaoResult2.entidadeVagao.Fechado = true;
				EntidadeVagao entidadeVagao = new EntidadeVagao();
				entidadeVagao.Estado = estadoVagao;
				entidadeVagao.Fluxo = fluxoVagao;
				entidadeVagao.Admin = entidadeVagaoResult2.entidadeVagao.Admin;
				entidadeVagao.EntidadeVagaostamp = 25.UseThisSizeForStamp();
				entidadeVagao.Codest = entidadeVagaoResult2.entidadeVagao?.Codest;
				entidadeVagao.Local = entidadeVagaoResult2.entidadeVagao.Local;
				entidadeVagao.Desiglocal = entidadeVagaoResult2.entidadeVagao?.Desiglocal;
				entidadeVagao.Stamppedret = pedidoRetiradaStamp;
				entidadeVagao.Codesto = "";
				entidadeVagao.Cliente = dadosFluxo?.Designcli;
				entidadeVagao.Carga = carga;
				entidadeVagao.Cliente = "";
				entidadeVagao.Expedidor = "";
				entidadeVagao.Consignat = "";
				entidadeVagao.Consgno = "";
				entidadeVagao.Consorig = entidadeVagaoResult2?.entidadeVagao?.Consgno;
				entidadeVagao.Dataini = wagonWithdrawalRequestDTO.firstSubmissionTentative.Date;
				entidadeVagao.Horaini = wagonWithdrawalRequestDTO.firstSubmissionTentative.ToString("HH:mm");
				entidadeVagao.Qtdton = default(decimal);
				entidadeVagao.Verificar = true;
				entidadeVagao.Qtd = default(decimal);
				entidadeVagao.Qtdunid = default(decimal);
				entidadeVagao.Qtdm3 = default(decimal);
				entidadeVagao.Nrreceb = "";
				entidadeVagao.Datapret = DateTime.Now.Date;
				entidadeVagao.Horapret = DateTime.Now.ToString("HH:mm");
				entidadeVagao.Nrpedret = numeroSequencialPedidoRetirada;
				entidadeVagao.No = wagonData2?.wagonNumber;
				entidadeVagao.Stampveic = dadosVeiculo.VeiculoFerroviariostamp;
				dadosVeiculo.Codest = entidadeVagaoResult2.entidadeVagao?.Codest;
				dadosVeiculo.Local = entidadeVagaoResult2.entidadeVagao.Local;
				dadosVeiculo.Desiglocal = entidadeVagaoResult2.entidadeVagao?.Desiglocal;
				_genericRepository.UpsertEntity(entidadeVagao, new List<string> { "Stamppedret", "EntidadeVagaostamp" }, new List<KeyValuePair<string, object>>
				{
					new KeyValuePair<string, object>("Stamppedret", pedidoRetiradaStamp),
					new KeyValuePair<string, object>("No", entidadeVagao.No)
				}, saveChanges: false);
				withdrawalWagonsResult.Add(new WagonRequestResultDTO
				{
					wagonNumber = wagonData2.wagonNumber,
					consignmentNumber = wagonData2.consignmentNumber,
					processed = true,
					errorMessage = ""
				});
			}
		}
		_genericRepository.BulkAdd(vagoesPorRegularizar);
		_genericRepository.SaveChanges();
		withdrawalResult.wagons = withdrawalWagonsResult;
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), withdrawalResult, null);
	}

	public ResponseDTO GetConsignmentByNumber(string consignmentNumber)
	{
		decimal? responseID = logHelper.generateResponseID();
		Dossier dossier = _consignacaoRepository.getConsignacao(consignmentNumber);
		if (dossier == null)
		{
			return new ResponseDTO(new ResponseCodesDTO("00404", "Consignment Not Found", responseID), null, null);
		}
		MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<SGOFWS.Domains.Models.Bi2, WagonDTO>().ForMember((WagonDTO dest) => dest.WagonNumber, delegate(IMemberConfigurationExpression<SGOFWS.Domains.Models.Bi2, WagonDTO, string> opt)
			{
				opt.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UVagno);
			});
		});
		Mapper mapper = new Mapper(config);
		List<WagonDTO> wagons = mapper.Map<List<WagonDTO>>(dossier.bi2);
		ConsignmentDTO consignment = new ConsignmentDTO
		{
			ConsignmentNumber = dossier?.bo3.UConsgno,
			wagons = wagons
		};
		return new ResponseDTO(new ResponseCodesDTO("0000", "Success", responseID), consignment, consignment);
	}

	public void CriarEntidadeVagao(AddEntidadeVagaoDTO addEntidadeVagaoDTO)
	{
		Dossier dossier = addEntidadeVagaoDTO.dossier;
		List<SGOFWS.Domains.Models.Bi2> bi2 = dossier.bi2;
		string admnistracao = dossier.bo3.UAdmin;
		string fluxo = dossier.bo3.UFluxocom;
		string consignacao = dossier.bo3.UConsgno;
		MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<SGOFWS.Domains.Models.Bi2, EntidadeVagao>().ForPath((EntidadeVagao dest) => dest.EntidadeVagaostamp, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
			{
				act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => 25.UseThisSizeForStamp(false));
			}).ForPath((EntidadeVagao dest) => dest.No, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
			{
				act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UVagno);
			})
				.ForPath((EntidadeVagao dest) => dest.Consgno, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => consignacao);
				})
				.ForPath((EntidadeVagao dest) => dest.Admin, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => admnistracao);
				})
				.ForPath((EntidadeVagao dest) => dest.Estado, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => (src.UFullempy == (bool?)true) ? "Carregado" : "Vazio");
				})
				.ForPath((EntidadeVagao dest) => dest.Carga, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UDsccmcag);
				})
				.ForPath((EntidadeVagao dest) => dest.Qtdton, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, decimal?> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UPeso);
				})
				.ForPath((EntidadeVagao dest) => dest.Qtdm3, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, decimal?> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UVagvol);
				})
				.ForPath((EntidadeVagao dest) => dest.Ousrdata, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, DateTime?> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => DateTime.Now.Date);
				})
				.ForPath((EntidadeVagao dest) => dest.Ousrdata, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, DateTime?> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => DateTime.Now.Date);
				})
				.ForPath((EntidadeVagao dest) => dest.Usrhora, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((EntidadeVagao dest) => dest.Ousrhora, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => DateTime.Now.ToString("HH:mm"));
				})
				.ForPath((EntidadeVagao dest) => dest.Qtdm3, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, decimal?> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UVagvol);
				})
				.ForPath((EntidadeVagao dest) => dest.Encerado, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => src.UEncerrno);
				})
				.ForPath((EntidadeVagao dest) => dest.Enceradmin, delegate(IPathConfigurationExpression<SGOFWS.Domains.Models.Bi2, EntidadeVagao, string> act)
				{
					act.MapFrom((SGOFWS.Domains.Models.Bi2 src) => admnistracao);
				});
		});
		Mapper mapper = new Mapper(config);
		List<EntidadeVagao> entidadeVagaoList = mapper.Map<List<EntidadeVagao>>(bi2);
		foreach (EntidadeVagao entidade in entidadeVagaoList)
		{
			List<KeyValuePair<string, object>> conditionsLinhaPlanoManobra = new List<KeyValuePair<string, object>>();
			conditionsLinhaPlanoManobra.Add(new KeyValuePair<string, object>("No", entidade.No));
			conditionsLinhaPlanoManobra.Add(new KeyValuePair<string, object>("Consgno", entidade.Consgno));
			List<string> keysToExcludeLinhaPlanoManobra = new List<string>();
			keysToExcludeLinhaPlanoManobra.Add("EntidadeVagaostamp");
			_genericRepository.UpsertEntity(entidade, keysToExcludeLinhaPlanoManobra, conditionsLinhaPlanoManobra, saveChanges: false);
		}
		_genericRepository.SaveChanges();
	}

	public string GerarNumeroSequencialManobra(DateTime dataini)
	{
		string snR = "";
		int MaxAno = 0;
		decimal myNr = default(decimal);
		DateTime? maxDataIni = _SGOFCTX.Manobras.Max((Manobras u) => u.Dataini);
		if (maxDataIni.HasValue)
		{
			MaxAno = maxDataIni.Value.Year;
		}
		if (DateTime.Now.Year == MaxAno && dataini.Year == MaxAno)
		{
			DateTime dateParameter = new DateTime(2024, 3, 1);
			string maxNoQuery = "SELECT MAX(no) AS no FROM U_FER019 WHERE YEAR(U_FER019.dataini) = YEAR({1})";
			string maxNo = (from x in _SGOFCTX.Manobras.FromSqlRaw(maxNoQuery, dataini, dataini).AsNoTracking()
				select x.No).FirstOrDefault();
			long currentYear = DateTime.Now.Year;
			long mnr = long.Parse(maxNo);
			if (mnr != 0L)
			{
				myNr = mnr - long.Parse((currentYear * 10000000).ToString());
			}
			if (myNr == 0m)
			{
				myNr = 1m;
			}
			else
			{
				myNr += 1m;
			}
			string myNrString = myNr.ToString();
			snR = DateTime.Now.Year + myNrString.PadLeft(7, '0');
		}
		return snR;
	}

	public void ActualizarLocalizacaoVagao(LocalizacaoVagaoDTO vagao)
	{
		IQueryable<SGOFWS.Domains.Models.Bi2> dossierResult = from bo in _SGOFCTX.Bo
			join bo2 in _SGOFCTX.Bo2 on bo.Bostamp equals bo2.Bo2stamp
			join bo3 in _SGOFCTX.Bo3 on bo.Bostamp equals bo3.Bo3stamp
			join bi2 in _SGOFCTX.Bi2 on bo.Bostamp equals bi2.Bostamp
			where bo3.UConsgno == vagao.consgno && bi2.UVagno == vagao.vagao
			select bi2;
		SGOFWS.Domains.Models.Bi2 dadosVagao = dossierResult.FirstOrDefault();
		if (dadosVagao != null)
		{
			dadosVagao.UCodactst = ((vagao?.codest == null) ? "" : vagao?.codest);
			dadosVagao.UStact = ((vagao?.estacao == null) ? "" : vagao?.estacao);
			dadosVagao.UProxst = ((vagao?.proximaEstacao == null) ? "" : vagao?.proximaEstacao);
			dadosVagao.UDtop = vagao?.dataOperacao;
			dadosVagao.UEta = vagao?.etaProximaEstacao;
			dadosVagao.UStatus = ((vagao?.estado?.Trim() == "A caminho da") ? ("Partiu da " + vagao?.estacao) : vagao?.estado?.Trim());
		}
		_SGOFCTX.SaveChanges();
	}

	public void ActualizarLocalizacaoVagoes(List<LocalizacaoVagaoDTO> vagoes)
	{
		List<string> vagaoActualizado = new List<string>();
		foreach (LocalizacaoVagaoDTO vagao in vagoes)
		{
			List<string> vagaoFoiActualizado = vagaoActualizado.Where((string vac) => vac == vagao?.vagao).ToList();
			if (vagaoFoiActualizado.Any())
			{
				continue;
			}
			List<LocalizacaoVagaoDTO> estacoesVagao = vagoes.Where((LocalizacaoVagaoDTO v) => v.vagao == vagao.vagao && v.consgno == vagao.consgno).ToList();
			var maxOrderStation = (from v in estacoesVagao
				where v.vagao == vagao.vagao && v.consgno == vagao.consgno
				group v by v.codest into g
				select new
				{
					Codest = g.Key,
					MaxOrder = g.Max((LocalizacaoVagaoDTO v) => v.ordem)
				} into x
				orderby x.MaxOrder descending
				select x).FirstOrDefault();
			List<LocalizacaoVagaoDTO> operacoesEstacao = estacoesVagao.Where((LocalizacaoVagaoDTO ev) => ev.codest == maxOrderStation?.Codest).ToList();
			LocalizacaoVagaoDTO dadosPartida = operacoesEstacao.Where((LocalizacaoVagaoDTO op) => op.tipoOp?.Trim() == "Departure").FirstOrDefault();
			LocalizacaoVagaoDTO dadosChegada = operacoesEstacao.Where((LocalizacaoVagaoDTO op) => op.tipoOp?.Trim() == "Arrival").FirstOrDefault();
			LocalizacaoVagaoDTO estacaoAcualVagao = new LocalizacaoVagaoDTO();
			if (dadosChegada != null)
			{
				estacaoAcualVagao = dadosChegada;
			}
			if (dadosPartida != null)
			{
				estacaoAcualVagao = dadosPartida;
			}
			IQueryable<SGOFWS.Domains.Models.Bi2> dossierResult = from bo in _SGOFCTX.Bo
				join bo2 in _SGOFCTX.Bo2 on bo.Bostamp equals bo2.Bo2stamp
				join bo3 in _SGOFCTX.Bo3 on bo.Bostamp equals bo3.Bo3stamp
				join bi2 in _SGOFCTX.Bi2 on bo.Bostamp equals bi2.Bostamp
				where bo3.UConsgno == vagao.consgno && bi2.UVagno == vagao.vagao
				select bi2;
			SGOFWS.Domains.Models.Bi2 dadosVagao = dossierResult.FirstOrDefault();
			if (dadosVagao != null)
			{
				dadosVagao.UCodactst = estacaoAcualVagao?.codest;
				dadosVagao.UStact = estacaoAcualVagao?.estacao;
				dadosVagao.UProxst = estacaoAcualVagao?.proximaEstacao;
				dadosVagao.UDtop = estacaoAcualVagao?.dataOperacao;
				dadosVagao.UComboio = estacaoAcualVagao?.comboio;
				dadosVagao.UEta = estacaoAcualVagao?.etaProximaEstacao;
				dadosVagao.UStatus = ((estacaoAcualVagao?.estado?.Trim() == "A caminho da ") ? ("Partiu da " + estacaoAcualVagao?.estacao) : estacaoAcualVagao?.estado?.Trim());
			}
			vagaoActualizado.Add(vagao?.vagao);
		}
		_SGOFCTX.SaveChanges();
	}

	public void ActualizarTrajecto(UTrajcons trajcons)
	{
		_genericRepository.UpsertEntity(trajcons, new List<string> { "UTrajconsstamp" }, new List<KeyValuePair<string, object>>
		{
			new KeyValuePair<string, object>("Consgno", trajcons.Consgno),
			new KeyValuePair<string, object>("Vagno", trajcons.Vagno),
			new KeyValuePair<string, object>("Tipoop", trajcons.Tipoop),
			new KeyValuePair<string, object>("Estacao", trajcons.Estacao)
		}, saveChanges: false);
	}

	public List<UStqueue> CreateStQueue(ComboioNotificacao dadosComboio, bool partida, List<UTrajrot> trajecto, List<UEntrot> destNot)
	{
		string estacao = "";
		DateTime operationDate = DateTime.Now;
		List<UStqueue> stationQueues = new List<UStqueue>();
		if (partida)
		{
			estacao = dadosComboio?.comboio?.Codorig;
			ComboioRegisto comboio = dadosComboio.comboio;
			string datac = ((comboio == null || !comboio.Datap.HasValue) ? "1900-01-01" : dadosComboio?.comboio?.Datap.Value.Date.ToString("yyyy-MM-dd"));
			operationDate = conversionExtension.ParseToDate(datac + " " + dadosComboio?.comboio?.Horap);
		}
		if (!partida)
		{
			estacao = dadosComboio?.comboio?.Coddest;
			ComboioRegisto comboio2 = dadosComboio.comboio;
			string datac2 = ((comboio2 == null || !comboio2.Datac.HasValue) ? "1900-01-01" : dadosComboio?.comboio?.Datac.Value.Date.ToString("yyyy-MM-dd"));
			operationDate = conversionExtension.ParseToDate(datac2 + " " + dadosComboio?.comboio?.Hora);
		}
		UTrajrot trajectoData = trajecto.Where((UTrajrot trj) => trj.Codigo.Trim() == estacao?.Trim()).FirstOrDefault();
		if (trajectoData != null)
		{
			string estacaoStamp = trajectoData?.UTrajrotstamp;
			UTrajrot nextStationNotificacao = trajecto.Where((UTrajrot trj) => trj.Codigo.Trim() == trajectoData?.Codprxst.Trim()).FirstOrDefault();
			UTrajrot bossaDataNot = trajecto.Where((UTrajrot trj) => trj != null && trj.Bossa && trj?.Codigo.Trim() == dadosComboio?.comboio?.Coddest?.Trim()).FirstOrDefault();
			decimal? ordem = trajectoData?.Ordem;
			string etaNextStation = "1900-01-01 00:00";
			string etaToDestination = "1900-01-01 00:00";
			Tempos destinationStation = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == dadosComboio.comboio?.Coddest.Trim()).FirstOrDefault();
			Tempos proximaEstacao = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == nextStationNotificacao?.Codigo.Trim()).FirstOrDefault();
			Tempos bossa = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == bossaDataNot?.Codigo.Trim()).FirstOrDefault();
			if (proximaEstacao != null)
			{
				etaNextStation = proximaEstacao.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + proximaEstacao.Hrprevc;
			}
			if (destinationStation != null)
			{
				etaToDestination = destinationStation.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + destinationStation.Hrprevc;
			}
			string station = trajectoData?.Estacao;
			string stationCode = trajectoData?.Codigo;
			string nexStation = trajectoData?.Proximaestacao;
			string nextStationCode = trajectoData?.Codprxst;
			string trainOrentation = dadosComboio?.comboio?.Sentido;
			MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<LocalizacaoVagao, UpdateStationWagonDTO>();
			});
			Mapper mapper = new Mapper(config);
			List<LocalizacaoVagao> localizacaoVagoes = dadosComboio.vagoes;
			List<UpdateStationWagonDTO> updateRequestWagons = mapper.Map<List<UpdateStationWagonDTO>>(localizacaoVagoes);
			if (trajectoData != null)
			{
				foreach (UEntrot dest in destNot)
				{
					stationQueues.Add(new UStqueue
					{
						UStqueuestamp = 25.UseThisSizeForStamp(),
						Station = station,
						Stationcode = stationCode,
						Destination = dadosComboio.comboio.Destino,
						Coddest = dadosComboio.comboio.Coddest,
						Ord = ordem.Value,
						Etanextst = conversionExtension.ParseToDate(etaNextStation),
						Operationdate = operationDate,
						Nextstation = nexStation,
						Nextstcode = nextStationCode,
						Etadestination = conversionExtension.ParseToDate(etaToDestination),
						Trainnumber = dadosComboio?.comboio.Numero,
						Trainreference = dadosComboio?.comboio.Ref,
						Tempostamp = "",
						Trainorientation = ((trainOrentation == "D") ? "Descending" : "Ascending"),
						Operationtype = (partida ? "Departure" : "Arrival"),
						Wagonsdata = JsonConvert.SerializeObject(updateRequestWagons, Formatting.Indented),
						Entity = dest?.Entidade
					});
				}
			}
		}
		return stationQueues;
	}

	public void ActualizacaoEstacoes()
	{
		decimal? requestID = logHelper.generateResponseID();
		try
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			DateTime dataExecucao = DateTime.Now;
			List<UStqueue> stationQueueData = new List<UStqueue>();
			List<ComboioRegisto> comboios = _consignacaoRepository.GetListComboiosPorNotificar();
			foreach (ComboioRegisto comboio in comboios)
			{
				RotaTrack rota = _consignacaoRepository.GetRotaTrack(comboio?.Codorig?.Trim(), comboio?.Coddest?.Trim());
				if (rota == null)
				{
					continue;
				}
				ComboioNotificacao dadosComboio = _consignacaoRepository.GetComboioNotificacao(comboio.ComboioRegistoStamp);
				List<UTrajrot> trajecto = rota.trajecto;
				List<UEntrot> entidadesNotificacao = rota.entidades;
				dadosComboio.comboio = comboio;
				ComboioRegisto comboioRegisto = comboio;
				if (comboioRegisto != null && comboioRegisto.Partiu == true && !comboio.Partidanotif)
				{
					List<UStqueue> estacQueue = CreateStQueue(dadosComboio, partida: true, trajecto, entidadesNotificacao);
					stationQueueData = stationQueueData.Concat(estacQueue).ToList();
					comboio.Partidanotif = true;
				}
				ComboioRegisto comboioRegisto2 = comboio;
				if (comboioRegisto2 != null && comboioRegisto2.Chegou == true && !comboio.Chegnotif)
				{
					List<UStqueue> estacQueue2 = CreateStQueue(dadosComboio, partida: false, trajecto, entidadesNotificacao);
					stationQueueData = stationQueueData.Concat(estacQueue2).ToList();
					comboio.Chegnotif = true;
				}
				foreach (Tempos tempo in dadosComboio.tempos)
				{
					UTrajrot estacaoNotificacao = trajecto.Where((UTrajrot trj) => trj.Codigo.Trim() == tempo.Codest.Trim()).FirstOrDefault();
					if (estacaoNotificacao == null)
					{
						continue;
					}
					foreach (UEntrot dest in entidadesNotificacao)
					{
						UTrajrot nextStationNotificacao = new UTrajrot();
						nextStationNotificacao = trajecto.Where((UTrajrot trj) => trj?.Codigo.Trim() == estacaoNotificacao?.Codprxst.Trim()).FirstOrDefault();
						UTrajrot bossaDataNot = trajecto.Where((UTrajrot trj) => trj != null && trj.Bossa && trj?.Codigo?.Trim() == comboio?.Coddest?.Trim()).FirstOrDefault();
						decimal? ordem = estacaoNotificacao?.Ordem;
						string etaNextStation = "1900-01-01 00:00";
						string etaToBossa = "1900-01-01 00:00";
						string etaToDestination = "1900-01-01 00:00";
						Tempos proximaEstacao = new Tempos();
						proximaEstacao = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == nextStationNotificacao?.Codigo.Trim()).FirstOrDefault();
						Tempos bossa = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == bossaDataNot?.Codigo.Trim()).FirstOrDefault();
						Tempos destinationStation = dadosComboio.tempos.Where((Tempos temp) => temp.Codest.Trim() == dadosComboio.comboio?.Coddest.Trim()).FirstOrDefault();
						if (proximaEstacao != null)
						{
							etaNextStation = proximaEstacao.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + proximaEstacao.Hrprevc;
						}
						if (bossa != null)
						{
							etaToBossa = bossa.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + bossa.Hrprevc;
						}
						if (destinationStation != null)
						{
							etaToDestination = destinationStation.Dtprevc.Date.ToString("yyyy-MM-dd") + " " + destinationStation.Hrprevc;
						}
						string station = estacaoNotificacao?.Estacao;
						string stationCode = estacaoNotificacao?.Codigo;
						string nexStation = estacaoNotificacao?.Proximaestacao;
						string trainOrentation = comboio?.Sentido;
						MapperConfiguration config = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
						{
							cfg.CreateMap<LocalizacaoVagao, UpdateStationWagonDTO>();
						});
						Mapper mapper = new Mapper(config);
						List<LocalizacaoVagao> localizacaoVagoes = dadosComboio.vagoes;
						List<UpdateStationWagonDTO> updateRequestWagons = mapper.Map<List<UpdateStationWagonDTO>>(localizacaoVagoes);
						if (tempo.Notifchegada)
						{
							comboio.Notificar = "";
							UStqueue stationQueue = new UStqueue
							{
								UStqueuestamp = 25.UseThisSizeForStamp(),
								Station = station,
								Stationcode = stationCode,
								Destination = dadosComboio.comboio.Destino,
								Coddest = dadosComboio.comboio.Coddest,
								Ord = ordem.Value,
								Etanextst = conversionExtension.ParseToDate(etaNextStation),
								Operationdate = conversionExtension.ParseToDate(tempo.Datac.Date.ToString("yyyy-MM-dd") + " " + tempo.Horac),
								Nextstation = nextStationNotificacao?.Estacao,
								Nextstcode = nextStationNotificacao?.Codigo,
								Etabossa = conversionExtension.ParseToDate(etaToBossa),
								Etadestination = DateTime.Parse(etaToDestination),
								Trainnumber = comboio.Numero,
								Trainreference = comboio.Ref,
								Tempostamp = tempo.TemposStamp,
								Trainorientation = ((trainOrentation == "D") ? "Descending" : "Ascending"),
								Operationtype = "Arrival",
								Wagonsdata = JsonConvert.SerializeObject(updateRequestWagons, Formatting.Indented),
								Entity = dest?.Entidade
							};
							_SGOFCTX.Add(stationQueue);
							_SGOFCTX.SaveChanges();
						}
						if (tempo.Notifpartida)
						{
							comboio.Notificar = "";
							UStqueue stationQueue2 = new UStqueue
							{
								UStqueuestamp = 25.UseThisSizeForStamp(),
								Station = station,
								Stationcode = stationCode,
								Destination = dadosComboio.comboio.Destino,
								Coddest = dadosComboio.comboio.Coddest,
								Ord = ordem.Value,
								Tempostamp = tempo.TemposStamp,
								Etanextst = conversionExtension.ParseToDate(etaNextStation),
								Operationdate = conversionExtension.ParseToDate(tempo.Datap.Date.ToString("yyyy-MM-dd") + " " + tempo.Horap),
								Nextstation = nextStationNotificacao?.Estacao,
								Nextstcode = nextStationNotificacao?.Codigo,
								Etabossa = conversionExtension.ParseToDate(etaToBossa),
								Etadestination = DateTime.Parse(etaToDestination),
								Trainnumber = comboio.Numero,
								Trainreference = comboio.Ref,
								Trainorientation = ((trainOrentation == "D") ? "Descending" : "Ascending"),
								Operationtype = "Departure",
								Wagonsdata = JsonConvert.SerializeObject(updateRequestWagons, Formatting.Indented),
								Entity = dest?.Entidade
							};
							_SGOFCTX.Add(stationQueue2);
							_SGOFCTX.SaveChanges();
						}
					}
					if (tempo.Notifchegada)
					{
						tempo.Notifchegada = false;
					}
					if (tempo.Notifpartida)
					{
						tempo.Notifpartida = false;
					}
				}
				_genericRepository.BulkAdd(stationQueueData);
				comboio.Notificar = "";
				_genericRepository.SaveChanges();
			}
			watch.Stop();
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
			logHelper.generateLogJB(finalResponse, requestID.ToString(), "MPDCService.UpdateStation");
		}
	}

	public void ActualizacaoEstacaoCFM()
	{
		decimal? requestID = logHelper.generateResponseID();
		try
		{
			List<UStqueue> stQueue = _consignacaoRepository.GetStQueueByEntity("CFM");
			List<UStqueue> orderedStations = stQueue.OrderBy((UStqueue s) => s.Ord).ToList();
			List<UTrajcons> trajecto = new List<UTrajcons>();
			List<LocalizacaoVagaoDTO> vagoesPorActualizar = new List<LocalizacaoVagaoDTO>();
			foreach (UStqueue updateStationRequestBody in orderedStations)
			{
				if (updateStationRequestBody.Operationdate.Year == 1900)
				{
					continue;
				}
				List<UpdateStationWagonDTO> wagons = JsonConvert.DeserializeObject<List<UpdateStationWagonDTO>>(updateStationRequestBody.Wagonsdata);
				foreach (UpdateStationWagonDTO vagao in wagons)
				{
					string estado = ((updateStationRequestBody?.Operationtype == "Arrival") ? ("Chegou à  " + updateStationRequestBody?.Station) : ("A caminho da " + updateStationRequestBody?.Nextstation));
					vagoesPorActualizar.Add(new LocalizacaoVagaoDTO
					{
						codest = updateStationRequestBody?.Stationcode,
						estacao = updateStationRequestBody.Station,
						estado = estado,
						comboio = updateStationRequestBody?.Trainreference,
						consgno = vagao.consignmentNumber,
						tipoOp = updateStationRequestBody.Operationtype,
						ordem = updateStationRequestBody.Ord,
						vagao = vagao.wagonNumber,
						etaProximaEstacao = updateStationRequestBody.Etanextst.ToString(),
						proximaEstacao = updateStationRequestBody.Nextstation,
						dataOperacao = updateStationRequestBody.Operationdate
					});
					ActualizarTrajecto(new UTrajcons
					{
						Vagno = vagao.wagonNumber,
						Consgno = vagao.consignmentNumber,
						Codest = updateStationRequestBody?.Stationcode,
						Estacao = updateStationRequestBody.Station,
						Codprxmst = "",
						Refcomboio = updateStationRequestBody.Trainreference,
						Etaproximast = updateStationRequestBody.Etanextst,
						Proximast = updateStationRequestBody.Nextstation,
						Dataop = updateStationRequestBody?.Operationdate,
						Tipoop = updateStationRequestBody.Operationtype,
						Sentido = updateStationRequestBody.Trainorientation,
						Tempostamp = updateStationRequestBody.Tempostamp
					});
				}
			}
			ActualizarLocalizacaoVagoes(vagoesPorActualizar);
			_SGOFCTX.RemoveRange(stQueue);
			_SGOFCTX.SaveChanges();
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
			logHelper.generateLogJB(finalResponse, requestID.ToString(), "ConsignmentService.UpdateStationCFM");
		}
	}

	public string GerarNumeroPedidoRetirada(DateTime data)
	{
		string snR = "";
		int MaxAno = 0;
		decimal myNr = default(decimal);
		DateTime dateParameter = new DateTime(2024, 3, 1);
		string maxNoQuery = "SELECT MAX(nrped) AS nrped FROM U_FER012 WHERE YEAR(data) = YEAR({1})";
		string maxNo = (from x in _SGOFCTX.CabecalhoPedidoRetirada.FromSqlRaw(maxNoQuery, data, data).AsNoTracking()
			select x.Nrped).FirstOrDefault();
		long currentYear = DateTime.Now.Year;
		long mnr = long.Parse(maxNo);
		if (mnr != 0L)
		{
			myNr = mnr - long.Parse((currentYear * 10000000).ToString());
		}
		if (myNr == 0m)
		{
			myNr = 1m;
		}
		else
		{
			myNr += 1m;
		}
		string myNrString = myNr.ToString();
		return DateTime.Now.Year + myNrString.PadLeft(7, '0');
	}
}
