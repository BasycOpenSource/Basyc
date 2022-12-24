using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Shared.Models;

public struct NullableResult<T>
{
	public T Value { get; set; }
	public bool WasFound { get; init; }
	public readonly T DefaultValue { get; init; }

	public NullableResult(T value, bool wasFound, bool checkValueType = true)
	{
		Value = value;
		WasFound = wasFound;
		DefaultValue = checkValueType ? (T)GetDefaultValue(value.GetType()) : default;
	}

	public NullableResult(T value, bool wasFound, T defaultValue)
	{
		Value = value;
		WasFound = wasFound;
		DefaultValue = defaultValue;
	}

	private static object GetDefaultValue(Type type)
	{
		if (type.IsValueType)
		{
			if (type == typeof(string))
			{
				return string.Empty;
			}
			else
			{
				return Activator.CreateInstance(type);
			}
		}
		else
		{
			return null;
		}
	}
}
