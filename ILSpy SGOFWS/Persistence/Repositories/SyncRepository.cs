using System.Collections.Generic;
using System.Linq;
using SGOFWS.Domains.Interfaces;
using SGOFWS.Domains.Models;
using SGOFWS.Persistence.Contexts;

namespace SGOFWS.Persistence.Repositories;

public class SyncRepository : ISyncRepository
{
	private readonly SGOFCTX _SGOFCTX;

	public SyncRepository(SGOFCTX sGOFCTX)
	{
		_SGOFCTX = sGOFCTX;
	}

	public List<USync> GetSync()
	{
		return _SGOFCTX.USync.ToList();
	}
}
