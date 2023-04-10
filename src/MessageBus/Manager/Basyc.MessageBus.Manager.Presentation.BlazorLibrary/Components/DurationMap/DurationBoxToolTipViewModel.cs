using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.ReactiveUi;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap;
public class DurationBoxToolTipViewModel : BasycReactiveViewModelBase
{
	[Reactive] public ActivityContext? ActivityContext { get; set; }

	[Reactive] public int LogsInformationCount { get; init; }
	[Reactive] public int LogsWarningCount { get; init; }
	[Reactive] public int LogsErrorCount { get; init; }

	public DurationBoxToolTipViewModel()
	{

		//LogsInformationCount = this.ReactiveAggregatorProperty(
		//	x => x.LogsInformationCount,
		//	x => x.ActivityContext.Value().Logs,
		//	x => x.Count(x => x.LogLevel is LogLevel.Information or LogLevel.Debug or LogLevel.Trace));

		//LogsWarningCount = this.ReactiveAggregatorProperty(
		//	x => x.LogsWarningCount,
		//	x => x.MessageRequest!.Diagnostics.LogEntries,
		//	x => x.Count(x => x.LogLevel is LogLevel.Warning));

		//LogsErrorCount = this.ReactiveAggregatorProperty(
		//	x => x.LogsErrorCount,
		//	x => x.MessageRequest!.Diagnostics.LogEntries,
		//	x => x.Count(x => x.LogLevel is LogLevel.Error or LogLevel.Critical));
	}
}
