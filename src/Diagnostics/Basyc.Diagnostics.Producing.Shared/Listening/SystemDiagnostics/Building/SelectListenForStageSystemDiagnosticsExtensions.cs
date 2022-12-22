using Basyc.Diagnostics.Producing.Shared.Listening;
using Basyc.Diagnostics.Producing.Shared.Listening.Building;
using Basyc.Diagnostics.Producing.Shared.Listening.SystemDiagnostics;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public static class SelectListenForStageSystemDiagnosticsExtensions
{
    public static SelectListenForStage AnyActvity(this SelectListenForStage parent)
    {
        parent.services.AddSingleton<IDiagnosticListener, SystemDiagnosticsListener>();
        return parent;
    }

    public static SelectListenForStage Actvity(this SelectListenForStage parent, Func<Activity, bool> filter)
    {
        parent.services.Configure<SystemDiagnosticsListenerOptions>(x => { x.Filter = filter; });
        parent.services.AddSingleton<IDiagnosticListener, SystemDiagnosticsListener>();
        return parent;
    }
}
