using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.Diagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Building.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupDiagnosticsStageBasycDiagnosticsExtensions
{
    public static SetupDiagnosticsStage AddBasycDiagnostics(this SetupDiagnosticsStage parent)
    {
        parent.Services.AddSingleton<IRequestDiagnosticsSource, BasycDiagnosticsReceiversRequestDiagnosticsSource>();
        return parent;
    }
}
