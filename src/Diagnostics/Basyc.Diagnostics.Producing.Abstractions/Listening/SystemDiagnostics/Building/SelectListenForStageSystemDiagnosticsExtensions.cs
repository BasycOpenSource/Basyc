using Basyc.Diagnostics.Producing.Shared.Listening;
using Basyc.Diagnostics.Producing.Shared.Listening.Building;
using Basyc.Diagnostics.Producing.Shared.Listening.SystemDiagnostics;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public static class SelectListenForStageSystemDiagnosticsExtensions
{
    public static SelectListenForStage AnyActvity(this SelectListenForStage parent)
    {
        parent.Services.AddSingleton<IDiagnosticListener, SystemDiagnosticsListener>();
        return parent;
    }

    public static SelectListenForStage Actvity(this SelectListenForStage parent, Func<Activity, bool> filter)
    {
        parent.Services.Configure<SystemDiagnosticsListenerOptions>(x => { x.Filter = filter; });
        parent.Services.AddSingleton<IDiagnosticListener, SystemDiagnosticsListener>();
        return parent;
    }
}
