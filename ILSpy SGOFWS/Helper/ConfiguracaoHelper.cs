using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SGOFWS.DTOs;

namespace SGOFWS.Helper;

public class ConfiguracaoHelper
{
	public decimal? getNdos(string codigo)
	{
		IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
		IConfigurationRoot config = configuration.Build();
		ConfiguracaoDossier[] configuracoes = config.GetSection("ConfiguracoesDossier").Get<ConfiguracaoDossier[]>();
		ConfiguracaoDossier configuracaoData = configuracoes.Where((ConfiguracaoDossier configuracao) => configuracao.codigo == codigo).FirstOrDefault();
		if (configuracaoData != null)
		{
			return configuracaoData.ndos;
		}
		throw new Exception("Configuração do Dossier não encontrada");
	}

	public ConfiguracaoDossier getConfiguracaoDossier(string codigo)
	{
		IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
		IConfigurationRoot config = configuration.Build();
		ConfiguracaoDossier[] configuracoes = config.GetSection("ConfiguracoesDossier").Get<ConfiguracaoDossier[]>();
		ConfiguracaoDossier configuracaoData = configuracoes.Where((ConfiguracaoDossier configuracao) => configuracao.codigo == codigo).FirstOrDefault();
		if (configuracaoData != null)
		{
			return configuracaoData;
		}
		throw new Exception("Configuração do Dossier não encontrada");
	}
}
