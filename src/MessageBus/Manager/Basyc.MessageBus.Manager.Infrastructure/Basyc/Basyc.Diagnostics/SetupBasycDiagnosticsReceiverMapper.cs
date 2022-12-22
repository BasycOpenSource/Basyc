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
	/// No mapping will be applied. Use only when receivers produces log entries with same id as local SessionId
	/// </summary>
	/// <returns></returns>
	public SetupRequesterStage NoMapper()
	{
		services.AddSingleton<IBasycDiagnosticsReceiverTraceIdMapper, NullBasycDiagnosticsReceiverTraceIdMapper>();
		return new(services);
	}

	/// <summary>
	/// Translates Ids received from daignostic receiver to local SessionId
	/// </summary>
	/// <typeparam name="TMapper"></typeparam>
	/// <returns></returns>
	public SetupRequesterStage UseMapper<TMapper>() where TMapper : class, IBasycDiagnosticsReceiverTraceIdMapper
	{
		services.RemoveAll<IBasycDiagnosticsReceiverTraceIdMapper>();
		services.AddSingleton<TMapper>();
		services.AddSingleton<IBasycDiagnosticsReceiverTraceIdMapper, TMapper>(x => x.GetRequiredService<TMapper>());
		return new(services);
	}
}