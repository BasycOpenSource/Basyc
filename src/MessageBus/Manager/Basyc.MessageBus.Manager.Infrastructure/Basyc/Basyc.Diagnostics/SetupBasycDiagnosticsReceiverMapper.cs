using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.Diagnostics;

public class SetupBasycDiagnosticsReceiverMapper : BuilderStageBase
{
    public SetupBasycDiagnosticsReceiverMapper(IServiceCollection services) : base(services)
    {
    }

    /// <summary>
    ///     No mapping will be applied. Use only when receivers produces log entries with same id as local SessionId.
    /// </summary>
    public SetupRequesterStage NoTraceIdMapper()
    {
        Services.AddSingleton<IBasycDiagnosticsReceiverTraceIdMapper, NullBasycDiagnosticsReceiverTraceIdMapper>();
        return new SetupRequesterStage(Services);
    }

    /// <summary>
    ///     Translates Ids received from daignostic receiver to local SessionId.
    /// </summary>
    public SetupRequesterStage UseTraceIdMapper<TMapper>() where TMapper : class, IBasycDiagnosticsReceiverTraceIdMapper
    {
        Services.RemoveAll<IBasycDiagnosticsReceiverTraceIdMapper>();
        Services.AddSingleton<TMapper>();
        Services.AddSingleton<IBasycDiagnosticsReceiverTraceIdMapper, TMapper>(x => x.GetRequiredService<TMapper>());
        return new SetupRequesterStage(Services);
    }
}
