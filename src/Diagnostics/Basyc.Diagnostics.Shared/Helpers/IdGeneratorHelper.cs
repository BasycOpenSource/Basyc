using System.Diagnostics;

namespace Basyc.Diagnostics.Shared.Helpers;

public static class IdGeneratorHelper
{
    public static string GenerateNewSpanId() => ActivitySpanId.CreateRandom().ToString();

    public static string GenerateNewTraceId() => ActivityTraceId.CreateRandom().ToString();
}
