using System;
using System.Collections.Generic;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;
using SGOFWS.Persistence.APIs.MPDC.DTOS;

namespace SGOFWS.Domains.Interfaces;

public interface IConsignacaoRepository
{
	Dossier getConsignacao(string consigno);

	List<ComboioRegisto> GetComboiosByRangeDate(DateTime startDate, DateTime dataFim);

	List<LinhasPlanoManobra> GetLinhasPlanoManobrasByPlanoStamp(string stamp);

	List<LinhasManobra> GetLinhasManobrasByPedidoId(string pedidoId);

	List<VeiculosPorRegularizar> GetVeiculosPorRegularizarByPedidoStamp(string stampPedido);

	List<EntidadeVagao> GetEntidadesByStampRetirada(string stamp);

	List<LinhasManobra> GetLinhasManobrasByManobraStamp(string stamp);

	List<Manobras> GetManobrasByIdAndTipo(string id, string tipo);

	Bo3 getConsignacaoSemDossier(string consigno);

	string GetUsByInicias(string inicias);

	LinhasManobra GetLinhaManobraByStampManobraAndVeiculo(string stampManobra, string novg);

	List<Dossier> GetConsignacoesPorNotificarIncluirOperacao(string operacao, string operacaoToInclude, string entidade, string sentido, DateTime data);

	UTopup GetUTopupByTipo(string tipo, string numero);

	List<ManobraELinha> GetManobrasByRange(DateTime dataInicio, DateTime dataFim);

	Cl GetClienteByNome(string nome);

	EntidadeVagao GetEntidadeVagaoByStamp(string stamp);

	List<EstacaoNotificacaoDestinatario> GetEstacoesNotificacao();

	List<UTopuplin> GetLinhasTopUp(string stamp);

	int ApproveTopUp(string stamp);

	UTopup GetTopUpByPedidoAndTipo(string pedido, string tipo);

	Dossier GetConsignacaoCabecalho(string consigno);

	Bo3 GetBo3ByStamp(string bo3stamp);

	Fluxo GetFluxoByCodigo(string codigo);

	List<UStqueue> GetStQueueByEntity(string entity);

	Bi2 GetVagaoByConsgnoAndVagno(string vagno, string consgno);

	CabecalhoPlanoManobra GetPlanoManobraByPedidoId(string pedidoID);

	CabecalhoPedidoRetirada GetCabecalhoPedidoRetiradaByPedidoId(string pedidoID);

	DesengateComboio GetDesengateComboioByComboioStamp(string comboioStamp);

	PatiosManobra GetPatioManobraByCodigo(string codigo);

	Manobras GetManobra(string id);

	List<LinhasManobra> GetLinhasManobras(string manobrastamp);

	List<EntidadeVagao> GetLinhasManobraById(string pedidoID);

	LinhaVeiculoFerroviario GetLinhaVeiculoByStampVag(string stampVag);

	ComboioRegisto GetComboioVagao(string entidadeVagaoStamp);

	LocaisFerroviarios GetLocalFerroviario(string codigo);

	VeiculoFerroviario GetVeiculoFerroviarioByNo(string no);

	int AprovarFornecimentoVagao(string pedidoID);

	int AprovarRetiradaVagao(string pedidoID);

	PlanoManobra GetPlanoManobraByPedido(string pedidoID);

	AdmnistracaoVizinha GetAdmnistracaoVizinhaVeiculoVazioByno(string no);

	int RemoveLinhasManobra(string manobrastamp);

	List<EntidadeVagao> GetEntidadeVagaoByPedidoRetiradaStamp(string pedidoRetiradaStamp);

	decimal? getMaxClNo();

	decimal? getMaxAgente();

	Manobras GetManobraById(string id, string tipo);

	int FinalizarManobra(ShuntingDTO shuntingDTO);

	int IniciarManobra(ShuntingDTO shuntingDTO);

	string getVagaoID(string consignacaoID, string vagno);

	Bi2 getVagao(string consignacaoID, string vagno);

	VagaoVeiculoFerroviarioDTO GetEntidadeVagao(string vagno, string consgno);

	PlanoManobra GetPlanoMabobraByPedidoID(string pedidoID);

	CabecalhoPlanoManobra GetPlanoMabobraByPedidoIDAndTipo(string pedidoID, string tipo);

	void deleteTest();

	void testCons();

	void criarConsignacao(Dossier dossier);

	List<ComboioRegisto> GetListComboiosPorNotificar();

	ComboioNotificacao GetComboioNotificacao(string stampComboio);

	List<Dossier> GetConsignacoesPorNotificar(string operacao, string entidade, string sentido, DateTime data);

	List<MineDepartureNotificationMPDCDTO> GetConsignacoesPartidaMinaMPDC();

	LinhasPlanoManobra GetVagaoFornecimentoByVagnoAndPedido(string consgno, string pedidoID, string vagno);

	string getEstacaoByCodigo(string codigo);

	UOridest GetDadosEstacaoByCodigo(string codigo);

	AgenteTransitario getAgenteByNuit(string nuit);

	Cl getClienteByNuit(string nuit);

	void overrideVagoes(Dossier dossier);

	decimal? getMaxObrano(decimal? ndos);

	bool? consignacaoExiste(string consignacao);

	void actualizarHistoricoConsignacao(string obs, string consignacaostamp);

	void inserirVagaoSeNaoExiste(Bi2 vagao, string consignacaostamp, Dossier consignacao);

	void actualizarTotalVeiculos(decimal? totalVeiculos, string consignacaostamp);

	Bi2 getVagaoActualizado(string consignacaostamp, string vagno, string uUltdtrep, string status);

	List<Comboio> GetDadosNotificacaoEstacao(string operacao, string entidade, string sentido);

	List<Bi2> getListaVagoesConsignacao(string consignacaostamp);

	RotaTrack GetRotaTrack(string origem, string destino);

	UNotific GetNotificacaoByTabstampAndEntidade(string tabstamp, string entidade);

	void saveChanges();

	void actualizarVagao(Bi2 vagao);
}
