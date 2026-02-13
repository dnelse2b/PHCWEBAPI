using System.Collections.Generic;
using SGOFWS.Domains.Models;
using SGOFWS.Persistence.APIs.MPDC.DTOS;

namespace SGOFWS.Domains.Interfaces;

public interface IMPDCHelper
{
	List<UpdateStationRequest> MapStations(List<UStqueue> stations);
}
