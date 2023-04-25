using Basyc.MessageBus.Manager.Application;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedParameterTypeNameFormatter : ITypedParameterNameFormatter
{
    public string GetCustomTypeName(Type type) => GetFriendlyName(type);

    private static string GetFriendlyName(Type type)
    {
        if (type == typeof(int))
        {
            return "int";
        }

        if (type == typeof(short))
        {
            return "short";
        }

        if (type == typeof(byte))
        {
            return "byte";
        }

        if (type == typeof(bool))
        {
            return "bool";
        }

        if (type == typeof(long))
        {
            return "long";
        }

        if (type == typeof(float))
        {
            return "float";
        }

        if (type == typeof(double))
        {
            return "double";
        }

        if (type == typeof(decimal))
        {
            return "decimal";
        }

        if (type == typeof(string))
        {
            return "string";
        }

        if (type.IsGenericType)
        {
            return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            elementType.ThrowIfNull();
            return GetFriendlyName(elementType) + "[]";
        }

        return type.Name;
    }
}
