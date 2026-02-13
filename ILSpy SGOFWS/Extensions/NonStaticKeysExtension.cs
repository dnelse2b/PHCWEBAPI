using System;
using System.Text;

namespace SGOFWS.Extensions;

public static class NonStaticKeysExtension
{
	public static string useThisSizeForStamp(this int size, bool lowerCase = false)
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
}
