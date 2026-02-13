using System;
using SGOFWS.DTOs;

namespace SGOFWS.Helper;

public class LocationHelper
{
	public LocationDTO HandleStationWithdrawalHelper(LocationDTO locationDTO, string source)
	{
		if (locationDTO != null && locationDTO.Code != null && locationDTO?.Code?.Trim() != "")
		{
			return locationDTO;
		}
		if (source == "MPDC")
		{
			return new LocationDTO
			{
				Code = "ACE01",
				Designation = "Estacao Central de Maputo"
			};
		}
		throw new Exception("SOURCE_NOT_MAPPED");
	}

	public string GetTrackInByAdmin(string admin, string linha)
	{
		if (!(admin == "TFR"))
		{
			if (admin == "CFM")
			{
				return "LRE06";
			}
			return "LRE07";
		}
		return "LRE07";
	}

	public TrackConsignmentDTO GetTrackConsignment(string admin, string siding, string linha)
	{
		TrackConsignmentDTO trackConsignment = new TrackConsignmentDTO
		{
			trackIn = GetTrackInByAdmin(admin, linha)
		};
		switch (siding)
		{
		case "MAPUTO":
			trackConsignment.destination = "Estação Central(Maputo)";
			trackConsignment.destinationCode = "ACP01";
			break;
		case "TCM-COAL (SIDING)":
		case "ENT DA MATOLA":
		case "MATOLA":
		case "MPM-TCMCOAL":
			trackConsignment.destination = "Estação do Entreposto da Matola";
			trackConsignment.destinationCode = "ACE04";
			break;
		}
		return trackConsignment;
	}

	public LocationDTO HandleYardSupplyHelper(LocationDTO locationDTO, string source)
	{
		if (locationDTO != null && locationDTO.Code != null && locationDTO?.Code?.Trim() != "")
		{
			return locationDTO;
		}
		if (source == "MPDC")
		{
			return new LocationDTO
			{
				Code = "MPT",
				Designation = "Estacao Central de Maputo"
			};
		}
		throw new Exception("SOURCE_NOT_MAPPED");
	}

	public LocationDTO HandleWagonLocationWithdrawalHelper(LocationDTO locationDTO, string source)
	{
		if (locationDTO != null && locationDTO.Code != null && locationDTO?.Code?.Trim() != "")
		{
			return locationDTO;
		}
		if (source == "MPDC")
		{
			return new LocationDTO
			{
				Code = "ACE01",
				Designation = "Estacao Central de Maputo"
			};
		}
		throw new Exception("SOURCE_NOT_MAPPED");
	}
}
