using System.Linq.Expressions;
using System.Reflection;
using Throw;

namespace System;

public static class TypeExtensions
{
    public static object CanBeNull(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsValueType && !type.IsNullable())
        {
            return type == typeof(string);
        }

        return true;
    }

    public static bool IsNullable(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType)
        {
            return typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        return false;
    }

    public static object? GetDefaultValue(this Type type)
    {
        if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }

    public static object Cast(this Type type, object data)
    {
        var dataParam = Expression.Parameter(typeof(object), "data");
        var body = Expression.Block(Expression.Convert(Expression.Convert(dataParam, data.GetType()), type));

        var run = Expression.Lambda(body, dataParam).Compile();
        var ret = run.DynamicInvoke(data);
        ret.ThrowIfNull();
        return ret;
    }

    /// <summary>
    ///     Return all methods matching constraints (even iherited).
    /// </summary>
    public static MethodInfo[] GetMethodsRecursive(this Type type, BindingFlags bindingFlags)
    {
        var methods = new List<MethodInfo>(type.GetMethods(bindingFlags));
        foreach (var parentInterfaceType in type.GetInterfaces())
        {
            foreach (var method in parentInterfaceType.GetMethods(bindingFlags))
            {
                if (!methods.Contains(method))
                {
                    methods.Add(method);
                }
            }
        }

        return methods.ToArray();
    }
}
