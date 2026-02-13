using SGOFWS.DTOs;

namespace SGOFWS.Domains.Interfaces;

public interface ISyncService
{
	ResponseDTO SyncData(SyncDTO syncDTO);

	void HandleSync();
}
