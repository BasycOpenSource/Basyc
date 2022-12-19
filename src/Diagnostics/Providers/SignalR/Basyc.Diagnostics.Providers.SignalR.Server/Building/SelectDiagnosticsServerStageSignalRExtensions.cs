using Basyc.Diagnostics.Server.Abstractions;
using Basyc.Diagnostics.Server.Abstractions.Building;
using Basyc.Diagnostics.SignalR.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SelectDiagnosticsServerStageSignalRExtensions
{
	/// <summary>
	/// Call <see cref="WepApplicationDiagnosticsSignalRExtensions.MapBasycSignalRDiagnosticsServer(WebApplication, string)"/> to start 
	/// </summary>
	/// <param name="parent"></param>
	/// <returns></returns>
	public static void SelectSignalRPusher(this SelectDiagnosticsServerStage parent)
	{
		parent.services.AddSignalR();
		parent.services.TryAddSingleton<InMemoryServerDiagnosticReceiver>();
		parent.services.AddSingleton<IServerDiagnosticReceiver, InMemoryServerDiagnosticReceiver>(x => x.GetRequiredService<InMemoryServerDiagnosticReceiver>());
		parent.services.AddSingleton<IServerDiagnosticPusher, SignalRServerDiagnosticPusher>();
	}
}
