using SGOFWS.DTOs;

namespace SGOFWS.Domains.Interfaces;

public interface ITrackService
{
	ResponseDTO UpdateTrainStation(TrainUpdateStationDTO trainUpdateStation);
}
