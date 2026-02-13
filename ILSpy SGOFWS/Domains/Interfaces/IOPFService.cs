using SGOFWS.DTOs;

namespace SGOFWS.Domains.Interfaces;

public interface IOPFService
{
	bool ProcessarRevisoes(RevisaoMaterialMapeadaDTO revisaoMaterialMapeada);

	bool ProcessarComposicoes(ComposicaoMapeadaDTO composicaoMapeada);
}
