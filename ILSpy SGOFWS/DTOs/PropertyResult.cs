using System;
using Newtonsoft.Json;

namespace SGOFWS.DTOs;

public class PropertyResult
{
	public object PropertyValue { get; set; }

	public Type PropertyType { get; set; }

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this);
	}
}
