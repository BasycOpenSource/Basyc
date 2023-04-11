using Basyc.Diagnostics.Shared.Logging;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.ReactiveUi;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.LogWindow;
public class LogWindowViewModel : BasycReactiveViewModelBase
{
	[Reactive] public RequestDiagnostic? RequestDiagnostic { get; set; }
	[Reactive] public int LogsCount { get; private set; }
	[Reactive] public ReadOnlyObservableCollection<LogEntry> Logs { get; set; } = null!;

	public LogWindowViewModel()
	{
		Logs = this.ReactiveCollectionProperty(
			x => x.Logs,
			x => x.RequestDiagnostic!.LogEntries,
			x => x);

		LogsCount = this.ReactiveProperty(
			x => x.LogsCount,
			x => x.RequestDiagnostic!.LogEntries.Count);
	}
}
