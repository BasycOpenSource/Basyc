using System.Runtime.CompilerServices;

namespace System;

public static class ObjectExtensions
{
    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="paramName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        return value is null ? throw new NullReferenceException($"{paramName} is not expected to be null here") : value;
    }

    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="paramName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, [CallerArgumentExpression("value")] string? paramName = null)
        where T : struct
    {
        return value is null ? throw new NullReferenceException($"{paramName} is not expected to be null here") : value.Value;
    }
}
