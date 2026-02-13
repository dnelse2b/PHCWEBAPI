using System;
using System.Reflection;
using SGOFWS.DTOs;

namespace SGOFWS.Helper;

public class ObjectHelper
{
	public object ConvertToType(Type targetType, object value)
	{
		if (targetType == null)
		{
			throw new ArgumentNullException("targetType");
		}
		if (value == null || targetType.IsInstanceOfType(value))
		{
			return value;
		}
		try
		{
			return Convert.ChangeType(value, targetType);
		}
		catch (InvalidCastException)
		{
			return null;
		}
		catch (FormatException)
		{
			return null;
		}
		catch (OverflowException)
		{
			return null;
		}
	}

	private PropertyInfo FindModelProperty(Type objType, string modelName)
	{
		PropertyInfo[] properties = objType.GetProperties();
		foreach (PropertyInfo property in properties)
		{
			if (property.PropertyType.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase))
			{
				return property;
			}
		}
		return null;
	}

	public PropertyResult ScanObjectValue(object obj, string modelName, string propertyName)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		Type objType = obj.GetType();
		PropertyInfo modelProperty = FindModelProperty(objType, modelName);
		if (modelProperty != null)
		{
			object modelInstance = modelProperty.GetValue(obj);
			if (modelInstance != null)
			{
				PropertyInfo targetProperty = modelInstance.GetType().GetProperty(propertyName);
				if (targetProperty != null)
				{
					return new PropertyResult
					{
						PropertyValue = targetProperty.GetValue(modelInstance),
						PropertyType = targetProperty.PropertyType
					};
				}
			}
		}
		return null;
	}
}
