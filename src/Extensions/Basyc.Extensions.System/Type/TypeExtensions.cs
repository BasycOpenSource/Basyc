using System.Reflection;

namespace System;

public static class TypeExtensions
{
    public static FieldInfo GetBackingField(this Type type, string propertyName)
    {
        var field = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        return field ?? throw new InvalidOperationException("Could not find backing field.");
    }

    public static FieldInfo? GetFieldEvenNested(this Type type, string fieldName, BindingFlags flags) => type.GetField(fieldName, flags) ?? type.BaseType.NullOr(baseType => GetFieldEvenNested(baseType, fieldName, flags));

    private static TConverted? NullOr<T, TConverted>(this T? obj, Func<T, TConverted?> converter)
        where T : class
        where TConverted : class => obj is null ? null : converter(obj);
}
