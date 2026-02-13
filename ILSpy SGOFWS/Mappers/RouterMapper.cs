using System;
using Newtonsoft.Json;
using SGOFWS.Domains.Models;
using SGOFWS.DTOs;

namespace SGOFWS.Mappers;

public class RouterMapper
{
	public readonly DossierTFRMapper dossierTFRMapper = new DossierTFRMapper();

	public readonly MPDCMapper mPDCMapper = new MPDCMapper();

	public Dossier mapData(string admin, object data)
	{
		if (!(admin == "TFR"))
		{
			if (admin == "MPDC")
			{
				UpdateConsignmentDataMPDCDTO consignacaoMPDC = JsonConvert.DeserializeObject<UpdateConsignmentDataMPDCDTO>(data.ToString());
				return mPDCMapper.mapDossier(consignacaoMPDC);
			}
			throw new Exception("Adminstração não encontrada");
		}
		ConsignacaoTFRDTO consignacao = JsonConvert.DeserializeObject<ConsignacaoTFRDTO>(data.ToString());
		return dossierTFRMapper.mapDossier(consignacao);
	}
}
