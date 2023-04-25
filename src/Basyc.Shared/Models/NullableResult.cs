namespace Basyc.Shared.Models;

public struct NullableResult<T>
{
    public NullableResult(T value, bool wasFound, bool checkValueType = true)
    {
        Value = value;
        WasFound = wasFound;
        DefaultValue = checkValueType ? (T)GetDefaultValue(value!.GetType()!)! : default;
    }

    public NullableResult(T value, bool wasFound, T defaultValue)
    {
        Value = value;
        WasFound = wasFound;
        DefaultValue = defaultValue;
    }

    public T Value { get; set; }

    public bool WasFound { get; init; }

    public readonly T? DefaultValue { get; init; }

    private static object? GetDefaultValue(Type type)
    {
        if (type.IsValueType)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }

            return Activator.CreateInstance(type);
        }

        return null;
    }
}
