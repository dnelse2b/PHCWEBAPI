using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SGOFWS.Extensions;

public static class ModelBuilderExtension
{
	public static DateTime? ToDateTime(this string dateString, int format)
	{
		throw new NotSupportedException();
	}

	public static ModelBuilder AddSqlConvertFunction(this ModelBuilder modelBuilder)
	{
		modelBuilder.HasDbFunction(() => ((string)null).ToDateTime(0)).HasTranslation((IReadOnlyList<SqlExpression> args) => new SqlFunctionExpression("CONVERT", args.Prepend(new SqlFragmentExpression("date")), nullable: true, new bool[3] { false, true, false }, typeof(DateTime?), null));
		return modelBuilder;
	}

	public static string HandleNull(this string value)
	{
		if (value != null)
		{
			return value;
		}
		return "";
	}
}
