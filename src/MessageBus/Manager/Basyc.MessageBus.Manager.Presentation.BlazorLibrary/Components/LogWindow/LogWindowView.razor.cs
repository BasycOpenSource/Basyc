using Basyc.Diagnostics.Shared.Logging;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.LogWindow;
public partial class LogWindowView
{
	[Reactive] public int LogsCount { get; set; }
	[Reactive] public ReadOnlyObservableCollection<LogEntry> Logs { get; set; } = null!;
}
