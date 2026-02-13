using System;
using System.Data.SqlTypes;
using System.Globalization;

namespace SGOFWS.Extensions;

public class ConversionExtension
{
	public decimal? toDecimal(string value)
	{
		try
		{
			return Convert.ToDecimal(value);
		}
		catch (Exception)
		{
			return default(decimal);
		}
	}

	public bool? toBool(string value)
	{
		try
		{
			return (value == "1" || int.Parse(value) == 1) ? true : false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool isOdd(string value)
	{
		decimal? num = toDecimal(value) % (decimal?)2;
		return !((num.GetValueOrDefault() == default(decimal)) & num.HasValue);
	}

	public DateTime ParseToDate(object value)
	{
		if (DateTime.TryParse(value?.ToString(), out var parsedDate) && parsedDate >= SqlDateTime.MinValue.Value && parsedDate <= SqlDateTime.MaxValue.Value)
		{
			return parsedDate;
		}
		return DateTime.Now;
	}

	public DateTime StringToDateTime(string value)
	{
		try
		{
			return DateTime.Parse(value);
		}
		catch (Exception)
		{
			return new DateTime(1900, 1, 1);
		}
	}

	public string nullToString(object value)
	{
		if (value != null)
		{
			return value.ToString().Trim();
		}
		return "";
	}

	public DateTime toDateTime(string value, string format)
	{
		try
		{
			return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
		}
		catch (Exception)
		{
			return default(DateTime);
		}
	}

	public decimal toTons(string value)
	{
		try
		{
			return Math.Round(decimal.Divide(decimal.Parse(value, CultureInfo.InvariantCulture), 1000m), 3);
		}
		catch (Exception)
		{
			return 0m;
		}
	}
}
