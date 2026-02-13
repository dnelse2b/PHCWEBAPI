using SGOFWS.Domains.Models;
using SGOFWS.DTOs;

namespace SGOFWS.Persistence.APIs.MPDC.DTOS;

public class LocalizacaoVagaoRequestDTO
{
	public PlanoManobra planoManobra { get; set; }

	public CabecalhoPedidoRetirada pedidoRetirada { get; set; }

	public EntidadeVagao entidadeVagao { get; set; }

	public LinhasPlanoManobra linhaPlanoManobra { get; set; }

	public ApproveWagonOperationDTO ApproveWagonOperationDTO { get; set; }
}
