using System.Collections.Generic;
using SGOFWS.Domains.Models;

namespace SGOFWS.DTOs;

public class ComboioNotificacao
{
	public ComboioRegisto comboio { get; set; }

	public List<Tempos> tempos { get; set; }

	public List<LocalizacaoVagao> vagoes { get; set; }
}
