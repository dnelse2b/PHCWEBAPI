using System.Collections.Generic;
using SGOFWS.Domains.Models;

namespace SGOFWS.Domains.Interfaces;

public interface ISyncRepository
{
	List<USync> GetSync();
}
