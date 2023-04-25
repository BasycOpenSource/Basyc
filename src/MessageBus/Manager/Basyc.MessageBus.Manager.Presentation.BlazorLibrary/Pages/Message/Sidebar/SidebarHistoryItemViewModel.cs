using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;
using Microsoft.Extensions.Logging;
using ReactiveUI.Fody.Helpers;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message.Sidebar;
public class SidebarHistoryItemViewModel : BasycReactiveViewModelBase
{
    public SidebarHistoryItemViewModel()
    {
        State = this.ReactiveProperty(
            x => x.State,
            x => x.MessageRequest!.State);

        Duration = this.ReactiveProperty(
            x => x.Duration,
            x => x.MessageRequest!.Duration);

        LogsInformationCount = this.ReactiveAggregatorProperty(
            x => x.LogsInformationCount,
            x => x.MessageRequest!.Diagnostics.LogEntries,
            x => x.Count(x => x.LogLevel is LogLevel.Information or LogLevel.Debug or LogLevel.Trace));

        LogsWarningCount = this.ReactiveAggregatorProperty(
            x => x.LogsWarningCount,
            x => x.MessageRequest!.Diagnostics.LogEntries,
            x => x.Count(x => x.LogLevel is LogLevel.Warning));

        LogsErrorCount = this.ReactiveAggregatorProperty(
            x => x.LogsErrorCount,
            x => x.MessageRequest!.Diagnostics.LogEntries,
            x => x.Count(x => x.LogLevel is LogLevel.Error or LogLevel.Critical));

        IsSelected = this.ReactiveProperty(
            x => x.IsSelected,
            x => x.NavigationService!.CurrentQueryParams,
            x => x.MessageRequest,
            x => x.Item1 is MessageRequest messageRequest && messageRequest == x.Item2);
    }

    [Reactive] public MessageRequest? MessageRequest { get; set; }

    [Reactive] public RequestResultState State { get; init; }

    [Reactive] public TimeSpan Duration { get; init; }

    [Reactive] public int LogsInformationCount { get; init; }

    [Reactive] public int LogsWarningCount { get; init; }

    [Reactive] public int LogsErrorCount { get; init; }

    [Reactive] public NavigationService? NavigationService { get; set; }

    [Reactive] public bool IsSelected { get; init; }
}
