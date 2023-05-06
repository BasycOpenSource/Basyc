using System.Runtime.CompilerServices;

namespace System;

public static class ObjectExtensions
{
    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, string? reason = null, [CallerArgumentExpression(nameof(value))] string? paramName = null)
#pragma warning disable CA2201
        => value is null ? throw new NullReferenceException($"{paramName} is not expected to be null here. {reason}") : value;
#pragma warning restore CA2201

    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, string? reason = null, [CallerArgumentExpression(nameof(value))] string? paramName = null)
#pragma warning disable CA2201
        where T : struct => value ?? throw new NullReferenceException($"{paramName} is not expected to be null here. {reason}");
#pragma warning restore CA2201
}
