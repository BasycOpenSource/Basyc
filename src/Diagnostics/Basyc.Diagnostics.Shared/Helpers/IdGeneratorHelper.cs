using System.Diagnostics;

namespace Basyc.Diagnostics.Shared.Helpers
{
    public static class IdGeneratorHelper
    {
        public static string GenerateNewSpanId()
        {
            return ActivitySpanId.CreateRandom().ToString();
        }

        public static string GenerateNewTraceId()
        {
            return ActivityTraceId.CreateRandom().ToString();
        }
    }
}