using DynamicData;
using DynamicData.Alias;
using System.ComponentModel;

namespace Basyc.ReactiveUi;
internal static class ObservableWpfReactiveUiExtensions
{
    //public static IObservable<IChangeSet<TObject>> SetAutoRefresh<TObject>(this IObservable<IChangeSet<TObject>> source, bool enabled)
    //   where TObject : INotifyPropertyChanged
    //{
    //  return enabled ? source.AutoRefresh() : source;
    //}
    public static IObservable<IChangeSet<TObject>> SetAutoRefresh<TObject>(this IObservable<IChangeSet<TObject>> source, bool enabled)
    {
        if (typeof(TObject).IsAssignableTo(typeof(INotifyPropertyChanged)) is false)
        {
            return source;
        }

        var sourceCasted = source.Select(x => (INotifyPropertyChanged)x!);

        return enabled ? source.Select(x => (INotifyPropertyChanged)x!).AutoRefresh().Select(x => (TObject)x) : source;
    }
}
