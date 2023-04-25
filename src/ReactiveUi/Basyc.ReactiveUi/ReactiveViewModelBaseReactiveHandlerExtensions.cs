using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Basyc.ReactiveUi;
#pragma warning disable SA1612
/// <summary>
/// Class containing extension methods for <see cref="BasycReactiveViewModelBase"/> for creating reactive handlers.
/// </summary>
public static class ReactiveViewModelBaseReactiveHandlerExtensions
{
    /// <summary>
    /// Calls <paramref name="handler"/> when view-model's collection property change or when item's property change.
    /// <br/>
    /// <br/>
    /// Everytime owner class of <paramref name="sourceProperty"/> produces proper <see cref="INotifyPropertyChanged"/> event
    /// the <paramref name="handler"/> will be called.
    /// <paramref name="sourceProperty"/> can be even deeply nested property of private field (x=>x.Property1.Property1) but all path segments (Property1, Property2) must be observable aka
    /// must produce <see cref="INotifyPropertyChanged"/> events.
    /// </summary>
    /// <param name="sourceProperty">Property to listen to.</param>
    public static ReactiveSubscription ReactiveHandlerForCollectionChanged<TViewModel, TSourceItem>(this TViewModel viewModel,
        Expression<Func<TViewModel, ReadOnlyObservableCollection<TSourceItem>>> sourceProperty,
        Action<TSourceItem> handler,
        bool listenForItemPropertiesChanges = true)
        where TViewModel : class, IBasycReactiveViewModel
        where TSourceItem : INotifyPropertyChanged
    {
        IDisposable? nestedSubscription = null;
        return viewModel.WhenAnyValue(sourceProperty)
            .Subscribe(x =>
            {
                nestedSubscription = x.ToObservableChangeSet()
                    .SetAutoRefresh(listenForItemPropertiesChanges)
                    .Subscribe(x =>
                    {
                        if (x.Last().Reason is ListChangeReason.AddRange)
                        {
                            foreach (var rangeItem in x.Last().Range)
                                handler(rangeItem);
                        }
                        else
                        {
                            handler(x.Last().Item.Current);
                        }
                    })
                    .DisposeWithViewModel(viewModel, nestedSubscription);
            })
            .DisposeWithViewModel(viewModel)
            .ToReactiveSubscription();
    }

    /// <inheritdoc cref="ReactiveHandlerForCollectionChanged{TViewModel,TSourceItem}" />
    /// <summary>
    /// Calls <paramref name="handler"/> when view-model's property collection item change.
    /// <br/>
    /// <br/>
    /// Everytime owner class of <paramref name="sourceProperty"/> produces proper <see cref="INotifyPropertyChanged"/> event
    /// the <paramref name="handler"/> will be called.
    /// <paramref name="sourceProperty"/> can be even deeply nested property of private field (x=>x.Property1.Property1) but all path segments (Property1, Property2) must be observable aka
    /// must produce <see cref="INotifyPropertyChanged"/> events.
    /// </summary>
    public static ReactiveSubscription ReactiveHandlerForItemModified<TViewModel, TSourceItem>(this TViewModel viewModel,
        Expression<Func<TViewModel, ReadOnlyObservableCollection<TSourceItem>>> sourceProperty,
        Action<TSourceItem> handler,
        bool listenForItemPropertiesChanges = true)
        where TViewModel : class, IBasycReactiveViewModel
        where TSourceItem : INotifyPropertyChanged
    {
        IDisposable? nestedSubscription = null;
        return viewModel.WhenAnyValue(sourceProperty)
            .Subscribe(x =>
            {
                nestedSubscription = x.ToObservableChangeSet()
                    .SetAutoRefresh(listenForItemPropertiesChanges)
                    .Where(x => x.HasCountChanged() is false)
                    .Subscribe(x =>
                    {
                        handler(x.Last().Item.Current);
                    })
                    .DisposeWithViewModel(viewModel, nestedSubscription);
            })
            .DisposeWithViewModel(viewModel)
            .ToReactiveSubscription();
    }

    /////<inheritdoc cref="ReactiveHandlerForCollectionChanged{TViewModel,TSourceItem}" />
    ///// <summary>
    ///// Calls <see cref="handler"/> when view-model's property change (not listening for nested properties )
    ///// <br/>
    ///// <br/>
    ///// Everytime owner class of <see cref="sourceProperty"/> produces proper <see cref="INotifyPropertyChanged"/> event
    ///// the <see cref="handler"/> will be called.
    ///// <see cref="sourceProperty"/> can be even deeply nested property of private field (x=>x.Property1.Property1) but all path segments (Property1, Property2) must be observable aka
    ///// must produce <see cref="INotifyPropertyChanged"/> events
    ///// </summary>
    //public static ReactiveSubscription ReactiveHandler<TViewModel, TObservedProperty>(this TViewModel viewModel,
    //  Expression<Func<TViewModel, IObservable<TObservedProperty>>> sourceProperty,
    //  Action<TObservedProperty> handler)
    //  where TViewModel : BasycReactiveViewModelBase
    //{
    //  return viewModel.WhenAnyObservable(sourceProperty!)
    //  .Subscribe(handler)
    //  .DisposeWithViewModel(viewModel)
    //  .ToReactiveSubscription();
    //}

    /// <inheritdoc cref="ReactiveHandlerForCollectionChanged{TViewModel,TSourceItem}" />
    /// <summary>
    /// Calls <paramref name="handler"/> when view-model's property change (not listening for nested properties )
    /// <br/>
    /// <br/>
    /// Everytime owner class of <paramref name="sourceProperty"/> produces proper <see cref="INotifyPropertyChanged"/> event
    /// the <paramref name="handler"/> will be called.
    /// <paramref name="sourceProperty"/> can be even deeply nested property of private field (x=>x.Property1.Property1) but all path segments (Property1, Property2) must be observable aka
    /// must produce <see cref="INotifyPropertyChanged"/> events.
    /// </summary>
    public static ReactiveSubscription ReactiveHandler<TViewModel, TObservedProperty>(this TViewModel viewModel,
        Expression<Func<TViewModel, TObservedProperty>> sourceProperty,
        Action<TObservedProperty> handler)
        where TViewModel : class, IBasycReactiveViewModel => viewModel.WhenAnyValue(sourceProperty!)
        .Subscribe(handler)
        .DisposeWithViewModel(viewModel)
        .ToReactiveSubscription();
}

#pragma warning restore SA1612
