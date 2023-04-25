namespace Basyc.Serializaton.Abstraction;

public static class TypedToSimpleConverter
{
    public static string ConvertTypeToSimple(Type type) => type.AssemblyQualifiedName!;

    public static string ConvertTypeToSimple<TType>() => ConvertTypeToSimple(typeof(TType));

    public static Type ConvertSimpleToType(string requestType)
    {
        var type = Type.GetType(requestType);
        if (type == null)
            throw new InvalidOperationException($"Failed to convert request simple type '{requestType}' to runtime type");

        return type;
    }
}
