using System;
using System.Text;

namespace SGOFWS.Extensions;

public static class KeysExtension
{
	public static string UseThisSizeForStamp(this int size, bool lowerCase = false)
	{
		StringBuilder builder = new StringBuilder(size);
		char offset = (lowerCase ? 'a' : 'A');
		for (int i = 0; i < size; i++)
		{
			char @char = (char)new Random().Next(offset, offset + 26);
			builder.Append(@char);
		}
		if (!lowerCase)
		{
			return builder.ToString();
		}
		return builder.ToString().ToLower();
	}

	public static object GetValObjDy(this object obj, string propertyName)
	{
		return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
	}

	public static string generateRequestId()
	{
		string uuidAsString = Guid.NewGuid().ToString();
		return "OPMT" + uuidAsString.Substring(0, 21);
	}
}
