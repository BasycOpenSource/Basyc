using System.Runtime.CompilerServices;

namespace System;

public static class ObjectExtensions
{
    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, [CallerArgumentExpression("value")] string? paramName = null)
#pragma warning disable CA2201
        => value is null ? throw new NullReferenceException($"{paramName} is not expected to be null here") : value;
#pragma warning restore CA2201

    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, [CallerArgumentExpression("value")] string? paramName = null)
#pragma warning disable CA2201
        where T : struct => value is null ? throw new NullReferenceException($"{paramName} is not expected to be null here") : value.Value;
#pragma warning restore CA2201
}
