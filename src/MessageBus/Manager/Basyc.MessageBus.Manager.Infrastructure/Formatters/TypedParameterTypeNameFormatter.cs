using Basyc.MessageBus.Manager.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedParameterTypeNameFormatter : ITypedParameterNameFormatter
{
    public string GetCustomTypeName(Type type)
    {
        return GetFriendlyName(type);
    }

    private static string GetFriendlyName(Type type)
    {
        if (type == typeof(int))
            return "int";
        if (type == typeof(short))
            return "short";
        if (type == typeof(byte))
            return "byte";
        if (type == typeof(bool))
            return "bool";
        if (type == typeof(long))
            return "long";
        if (type == typeof(float))
            return "float";
        if (type == typeof(double))
            return "double";
        if (type == typeof(decimal))
            return "decimal";
        if (type == typeof(string))
            return "string";
        if (type.IsGenericType)
            return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(x => GetFriendlyName(x)).ToArray()) + ">";
        if (type.IsArray)
            return GetFriendlyName(type.GetElementType()) + "[]";

        return type.Name;
    }
}
