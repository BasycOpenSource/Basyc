using Basyc.Diagnostics.Producing.Shared.Listening;
using Basyc.Diagnostics.Producing.Shared.Listening.Building;
using Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SelectListenForStageMicrosoftLoggingExtensions
    {
        public static SelectListenForStage AnyLog(this SelectListenForStage parent)
        {
            parent.services.AddSingleton<MicrosoftLoggingDiagnosticListener>();
            parent.services.AddSingleton<IDiagnosticListener, MicrosoftLoggingDiagnosticListener>(x => x.GetRequiredService<MicrosoftLoggingDiagnosticListener>());
            parent.services.AddLogging(x => x.AddBasycExporterLog());
            return parent;
        }
    }
}