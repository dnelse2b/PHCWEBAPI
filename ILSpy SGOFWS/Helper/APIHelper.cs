using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SGOFWS.DTOs;

namespace SGOFWS.Helper;

public class APIHelper
{
	public API getApiEntity(string entity, string operationCode)
	{
		IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
		IConfigurationRoot config = configuration.Build();
		API[] configuracoes = config.GetSection("APIS").Get<API[]>();
		API apiEntityData = configuracoes.Where((API apiEntity) => apiEntity.entity == entity).FirstOrDefault();
		if (apiEntityData != null)
		{
			IEnumerable<Endpoint> endpointData = apiEntityData.endpoints.Where((Endpoint endpoint) => endpoint.operationCode == operationCode);
			if (endpointData != null)
			{
				apiEntityData.status = "1";
				return apiEntityData;
			}
			return new API
			{
				status = "0",
				message = "Não foi encontrado o endpoint com o código indicado para a respectiva entidade."
			};
		}
		return new API
		{
			status = "0",
			message = "Os dados da API da entidade indicada não foram encontrados"
		};
	}
}
