using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System;

public static class TypeExtensions
{
	public static object CanBeNull(this Type type)
	{
		TypeInfo typeInfo = type.GetTypeInfo();
		if (typeInfo.IsValueType && !type.IsNullable())
		{
			return type == typeof(string);
		}

		return true;
	}

	public static bool IsNullable(this Type type)
	{
		TypeInfo typeInfo = type.GetTypeInfo();
		if (typeInfo.IsGenericType)
		{
			return typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		return false;
	}

	public static object GetDefaultValue(this Type type)
	{
		if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
		{
			return Activator.CreateInstance(type);
		}

		return null;
	}

	public static object Cast(this Type type, object data)
	{
		var DataParam = Expression.Parameter(typeof(object), "data");
		var Body = Expression.Block(Expression.Convert(Expression.Convert(DataParam, data.GetType()), type));

		var Run = Expression.Lambda(Body, DataParam).Compile();
		var ret = Run.DynamicInvoke(data);
		return ret;
	}

	/// <summary>
	/// Return all methods matching constraints (even iherited)
	/// </summary>
	/// <returns></returns>
	public static MethodInfo[] GetMethodsRecursive(this Type type, BindingFlags bindingFlags)
	{
		List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods(bindingFlags));
		foreach (Type parentInterfaceType in type.GetInterfaces())
		{
			foreach (MethodInfo method in parentInterfaceType.GetMethods(bindingFlags))
				if (!methods.Contains(method))
					methods.Add(method);
		}

		return methods.ToArray();
	}
}
