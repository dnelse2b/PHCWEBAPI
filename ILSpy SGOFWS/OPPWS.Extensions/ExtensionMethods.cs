using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace OPPWS.Extensions;

public static class ExtensionMethods
{
	public static T DeepCopy<T>(this T self)
	{
		string serialized = JsonConvert.SerializeObject(self);
		return JsonConvert.DeserializeObject<T>(serialized);
	}

	public static List<string> GetAssignedProperties(this object obj)
	{
		List<string> assignedProperties = new List<string>();
		PropertyInfo[] properties = obj.GetType().GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo property in array)
		{
			if (property.PropertyType == typeof(string))
			{
				if (property.GetValue(obj) is string)
				{
					assignedProperties.Add(property.Name);
				}
			}
			else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
			{
				object value2 = property.GetValue(obj);
				if (value2 != null)
				{
					assignedProperties.Add(property.Name);
				}
			}
		}
		return assignedProperties;
	}

	public static void AssignDefaultValues(this object obj)
	{
		PropertyInfo[] properties = obj.GetType().GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo property in array)
		{
			if (property.PropertyType == typeof(string))
			{
				string value = property.GetValue(obj) as string;
				if (value == null)
				{
					property.SetValue(obj, string.Empty);
				}
			}
			else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
			{
				object value2 = property.GetValue(obj);
				DateTime minValue = new DateTime(1753, 1, 1);
				DateTime maxValue = new DateTime(9999, 12, 31);
				DateTime defaultValue = new DateTime(1900, 1, 1);
				if (value2 == null || (DateTime)value2 < minValue || (DateTime)value2 > maxValue)
				{
					property.SetValue(obj, defaultValue);
				}
			}
			else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
			{
				object value3 = property.GetValue(obj);
				if (value3 == null)
				{
					Type nonNullableType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
					object defaultValue2 = Activator.CreateInstance(nonNullableType);
					property.SetValue(obj, defaultValue2);
				}
			}
		}
	}

	public static void AssignDefaultEntityValues<T>(this T obj) where T : class
	{
		PropertyInfo[] properties = obj.GetType().GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo property in array)
		{
			if (property.PropertyType == typeof(string))
			{
				string value = property.GetValue(obj) as string;
				if (value == null)
				{
					property.SetValue(obj, string.Empty);
				}
			}
			else if (property.PropertyType == typeof(DateTime) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime))
			{
				object value2 = property.GetValue(obj);
				DateTime minValue = new DateTime(1753, 1, 1);
				DateTime maxValue = new DateTime(9999, 12, 31);
				DateTime defaultValue = new DateTime(1900, 1, 1);
				if (value2 == null || (DateTime)value2 < minValue || (DateTime)value2 > maxValue)
				{
					property.SetValue(obj, defaultValue);
				}
			}
			else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
			{
				object value3 = property.GetValue(obj);
				if (value3 == null)
				{
					Type nonNullableType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
					object defaultValue2 = Activator.CreateInstance(nonNullableType);
					property.SetValue(obj, defaultValue2);
				}
			}
		}
	}
}
