using System.Collections.Generic;
using Newtonsoft.Json;
using SGOFWS.DTOs;

namespace SGOFWS.Domains.Models;

public class Comboio
{
	public ComboioRegisto dadosComboio { get; set; }

	public List<LocalizacaoVagao> veiculos { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
