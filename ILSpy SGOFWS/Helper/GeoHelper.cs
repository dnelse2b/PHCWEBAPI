namespace SGOFWS.Helper;

public class GeoHelper
{
	public string getAdminByCountryCode(string countryCode)
	{
		return countryCode switch
		{
			"ZW" => "NRZ", 
			"MZ" => "CFM", 
			"ZA" => "TFR", 
			_ => "NEX", 
		};
	}
}
