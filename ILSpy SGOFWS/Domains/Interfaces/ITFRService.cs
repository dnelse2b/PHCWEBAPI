using System.Collections.Generic;
using System.Threading.Tasks;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;

namespace SGOFWS.Domains.Interfaces;

public interface ITFRService
{
	ResponseDTO BulkConsignmentHandler(string admin, List<object> data, bool recovery);

	Task TFRConsignmentsHandler();

	Dossier processarConsignacao(Dossier dossier, string admin, List<Dossier> novasConsignacoes, List<VagaoDTO> vagoesActualizados, bool recovery);

	void deleteTest();

	void getMaxObrano();
}
