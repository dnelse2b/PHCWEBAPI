using System;
using System.Collections.Generic;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Persistence.APIs.MPDC.DTOS;

namespace SGOFWS.Domains.Interfaces;

public interface IConsignmentService
{
	void ProcessarTopUp(string topUpStamp, TopUpDTO topUp, List<UTopuplin> linhasTopUp);

	Dossier ProcessarConsignacao(Dossier dossier, string admin);

	void CriarEntidadeVagao(AddEntidadeVagaoDTO addEntidadeVagaoDTO);

	string GerarNumeroSequencialManobra(DateTime dataini);

	ResponseDTO GetRequestByID(string requestType, string requestID);

	ResponseDTO GetManobrasByRange(DateTime startDate, DateTime endDate);

	ResponseDTO GetConsignmentByNumber(string consignmentNumber);

	string GerarNumeroPedidoRetirada(DateTime data);

	void ActualizarLocalizacaoVagao(LocalizacaoVagaoDTO vagao);

	void FinalizarManobra(ShuntingDTO shuntingDTO, string entidade);

	void ActualizarTrajecto(UTrajcons trajcons);

	void SincronzarGuias();

	ResponseDTO GetStationsByRange(DateTime startDate, DateTime endDate, string entity);

	ResponseDTO ProcessarPedidoRetirada(WagonWithdrawalResquestDTO wagonWithdrawalRequestDTO, string source);

	ResponseDTO ProcessarPedidoFornecimento(WagonSupplyRequestDTO wagonSupplyRequestDTO, string source);
}
