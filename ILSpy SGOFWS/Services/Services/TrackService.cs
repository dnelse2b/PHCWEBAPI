using System;
using System.Collections.Generic;
using SGOFWS.Domains.Interface;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Extensions;
using SGOFWS.Helper;

namespace SGOFWS.Services;

public class TrackService : ITrackService
{
	private readonly LogHelper logHelper = new LogHelper();

	private readonly IOPFService _OPFService;

	private readonly IOPFRepository _OPFRepository;

	private readonly IGenericRepository _genericRepository;

	private readonly IConsignacaoRepository _consignacaoRepository;

	private readonly ConversionExtension conversionExtension = new ConversionExtension();

	public TrackService(IOPFService oPFService, IOPFRepository oPFRepository, IGenericRepository genericRepository, IConsignacaoRepository consignacaoRepository)
	{
		_OPFService = oPFService;
		_OPFRepository = oPFRepository;
		_consignacaoRepository = consignacaoRepository;
		_genericRepository = genericRepository;
	}

	public ResponseDTO UpdateTrainStation(TrainUpdateStationDTO trainUpdateStation)
	{
		decimal? responseID = logHelper.generateResponseID();
		ResponseDTO response = new ResponseDTO(new ResponseCodesDTO("0000", "Dados actualizados com sucesso", responseID), null, null);
		try
		{
			ComboioRegisto comboioRegisto = _OPFRepository.GetComboioRegistoByRef(trainUpdateStation.train);
			if (comboioRegisto == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0404", "Comboio não encontrado", responseID), null, null);
			}
			UOridest dadosEstacao = _consignacaoRepository.GetDadosEstacaoByCodigo(trainUpdateStation?.station);
			if (dadosEstacao == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0404", "Estação não encontrada", responseID), null, null);
			}
			DateTime date = DateTime.Parse(trainUpdateStation.date);
			if (date.Year == 1900)
			{
				return new ResponseDTO(new ResponseCodesDTO("0400", "Data inválida", responseID), null, null);
			}
			if (comboioRegisto?.Cauda?.Trim() != trainUpdateStation?.tail)
			{
				response = new ResponseDTO(new ResponseCodesDTO("0000", "Dados actualizados com sucesso. Tenha atenção que a cauda é diferente da anterior.", responseID), null, null);
			}
			string telemeter = trainUpdateStation.telemeter;
			if (telemeter != null && telemeter.Length > 0)
			{
				Telemetro telemetroData = _OPFRepository.GetTelemetroByNo(trainUpdateStation?.telemeter?.Trim());
				if (telemetroData == null)
				{
					return new ResponseDTO(new ResponseCodesDTO("0404", "Telemetro não encontrado", responseID), null, null);
				}
				telemetroData.Antest = telemetroData.Ultest;
				telemetroData.Coantest = telemetroData.Coultest;
				telemetroData.Ultest = dadosEstacao?.Cidade;
				telemetroData.Coultest = trainUpdateStation.station;
			}
			comboioRegisto.Notificar = "NOTIFICAR";
			comboioRegisto.Telemetro = trainUpdateStation.telemeter;
			comboioRegisto.Caudaant = comboioRegisto.Cauda;
			comboioRegisto.Cauda = trainUpdateStation.tail;
			if (comboioRegisto.Coddest?.Trim() == trainUpdateStation.station?.Trim() && trainUpdateStation.operation == "Arrival")
			{
				comboioRegisto.Chegnotif = false;
				comboioRegisto.Datac = date.Date;
				comboioRegisto.Hora = date.ToString("HH:mm");
				comboioRegisto.Chegou = true;
			}
			if (comboioRegisto.Codorig?.Trim() == trainUpdateStation.station?.Trim() && trainUpdateStation.operation == "Departure")
			{
				comboioRegisto.Partidanotif = false;
				comboioRegisto.Datap = date.Date;
				comboioRegisto.Horap = date.ToString("HH:mm");
				comboioRegisto.Partiu = true;
			}
			List<Tempos> tempos = _OPFRepository.GetTemposByComboioStamp(comboioRegisto.ComboioRegistoStamp);
			Tempos estacaoTempo = tempos.Find((Tempos x) => x.Codest?.Trim() == trainUpdateStation.station?.Trim());
			if (estacaoTempo == null)
			{
				return new ResponseDTO(new ResponseCodesDTO("0404", "Station not found fer030", responseID), null, null);
			}
			if (trainUpdateStation.operation == "Arrival")
			{
				estacaoTempo.Datac = date.Date;
				estacaoTempo.Horac = date.ToString("HH:mm");
				estacaoTempo.Notifchegada = true;
			}
			if (trainUpdateStation.operation == "Departure")
			{
				estacaoTempo.Datap = date.Date;
				estacaoTempo.Horap = date.ToString("HH:mm");
				estacaoTempo.Notifpartida = true;
			}
			_genericRepository.SaveChanges();
			return response;
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
			logHelper.generateLogJB(finalResponse, responseID.ToString(), "TrackService.UpdateTrainStation");
			return finalResponse;
		}
	}
}
