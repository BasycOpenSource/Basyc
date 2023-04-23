﻿using System.Runtime.CompilerServices;

namespace System;

public static class ObjectExtensions
{
    /// <summary>
    ///     Checks if value is null.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Value<T>(this T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
#pragma warning disable CA2201 // Do not raise reserved exception types
            throw new NullReferenceException($"{paramName} is not expected to be null here");
#pragma warning restore CA2201 // Do not raise reserved exception types

        return value;
    }
}
