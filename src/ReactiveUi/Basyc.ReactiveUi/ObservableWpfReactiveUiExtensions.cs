using DynamicData;
using System.ComponentModel;

namespace Basyc.ReactiveUi;
internal static class ObservableWpfReactiveUiExtensions
{
	public static IObservable<IChangeSet<TObject>> SetAutoRefresh<TObject>(this IObservable<IChangeSet<TObject>> source, bool enabled)
		where TObject : INotifyPropertyChanged => enabled ? source.AutoRefresh() : source;
}
