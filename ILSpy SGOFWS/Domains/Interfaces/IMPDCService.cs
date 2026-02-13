using System.Threading.Tasks;
using SGOFWS.DTOs;
using SGOFWS.Persistence.APIs.MPDC.DTOS;

namespace SGOFWS.Domains.Interfaces;

public interface IMPDCService
{
	Task NotifyMPDCMineDeparture();

	Task UpdateStation();

	Task<ResponseDTO> ApproveWagonOperation(ApproveWagonOperationDTO ApproveWagonOperationDTO);

	Task<ResponseDTO> StartShuntingAsync(ShuntingDTO shunting);

	Task<ResponseDTO> SubmitShunting(ShuntingDTO shunting);

	ResponseDTO UpdateConsignment(UpdateConsignmentDataMPDCDTO updateConsignment);

	Task<ResponseDTO> TopUpRequest(TopUpDTO topUp);

	ResponseDTO RequestForWagonSupply(WagonSupplyRequestDTO wagonSupplyRequestDTO);

	Task<string> HandleApproveWagonOperation(ApproveWagonOperationDTO ApproveWagonOperationDTO);

	ResponseDTO RequestForWagonWithdrawal(WagonWithdrawalResquestDTO wagonWithdrawalRequestDTO);

	ResponseDTO WagonOffload(WagonOffloadDTO wagonOffloadDTO);

	Task<ResponseDTO> CloseShuntingAsync(ShuntingDTO shunting);
}
