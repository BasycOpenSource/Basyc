using Basyc.Diagnostics.Producing.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Server;

namespace Microsoft.AspNetCore.Builder
{
    public static class WepApplicationDiagnosticsSignalRExtensions
    {
        public static void MapBasycSignalRDiagnosticsServer(this WebApplication webApplication, string producersHubPattern = SignalRConstants.ProducersHubPattern, string receiversHubPattern = SignalRConstants.ReceiversHubPattern)
        {
            webApplication.MapHub<LoggingProducersHub>(producersHubPattern);
            webApplication.MapHub<LoggingReceiversHub>(receiversHubPattern);
        }

    }
}