using System.Collections.Generic;
using SGOFWS.Domains.Models;

namespace SGOFWS.Domains.Interfaces;

public interface IOPFRepository
{
	RevisaoMaterial GetRevisaoByStamp(string revistaoStamp);

	Recebimento GetRecebimentoByRevisaoStamp(string revistaoStamp);

	Composicao GetComposicaoByStamp(string composicaoStamp);

	ComboioRegisto GetComboioRegistoByRef(string combref);

	Telemetro GetTelemetroByNo(string no);

	List<Tempos> GetTemposByComboioStamp(string comboioStamp);

	Bi2 GetBi2Bystamp(string bi2stamp);
}
