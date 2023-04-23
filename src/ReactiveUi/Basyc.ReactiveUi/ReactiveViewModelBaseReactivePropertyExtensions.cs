using Basyc.Extensions.System.Linq.Expressions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Basyc.ReactiveUi;
/// <summary>
/// Class containing extension methods for <see cref="ReactiveViewModelBase"/> for creating reactive properties
/// </summary>
public static class ReactiveViewModelBaseReactivePropertyExtensions
{
    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     <br/> Usage:
    ///     <code>
    ///         [Reactive] public int VmSourceProperty { get; init }
    ///         [Reactive] public string VmTargetProperty { get; init }
    ///         public ViewModel()
    ///         {
    ///             VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty)
    ///         }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    public static TProperty ReactiveProperty<TViewModel, TProperty>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TProperty>> targetProperty,
        Expression<Func<TViewModel, TProperty>> sourceProperty,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where TViewModel : BasycReactiveViewModelBase
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyGetter = targetProperty.Compile();
        viewModel.WhenAnyValue(sourceProperty)
            .BindTo(viewModel, targetProperty!)
            .DisposeWithViewModel(viewModel);
        return targetPropertyGetter.Invoke(viewModel);
    }

    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     (property to property)
    ///     with conversion function.
    ///     <br/> Usage:
    ///     <code>
    ///         [Reactive] public int VmSourceProperty { get; init }
    ///         [Reactive] public string VmTargetProperty { get; init }
    ///         public ViewModel()
    ///         {
    ///             VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty, x=>x.VmSourceProperty, x => x.ToString())
    ///         }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    /// <param name="converter">Function transforming source property to target property.</param>
    public static TTargetProperty ReactiveProperty<TViewModel, TSourceProperty, TTargetProperty>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TTargetProperty>> targetProperty,
        Expression<Func<TViewModel, TSourceProperty>> sourceProperty,
        Func<TSourceProperty, TTargetProperty> converter,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where TViewModel : BasycReactiveViewModelBase
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyGetter = targetProperty.Compile();

        viewModel.WhenAnyValue(sourceProperty)
            .DisposeOldPropertyValue(() => targetPropertyGetter.Invoke(viewModel))
            .Select(converter)
            .BindTo(viewModel, targetProperty!)
            .DisposeWithViewModel(viewModel);
        return targetPropertyGetter.Invoke(viewModel);
    }

    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     (property to property)
    ///     with conversion function.
    ///     <br/> Usage:
    ///     <code>
    ///         [Reactive] public int VmSourceProperty { get; init }
    ///         [Reactive] public string VmTargetProperty { get; init }
    ///         public ViewModel()
    ///         {
    ///             VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty, x=>x.VmSourceProperty, x => x.ToString())
    ///         }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    /// <param name="converter">Function transforming source property to target property.</param>
    public static TTargetProperty ReactiveProperty<TViewModel, TSourceProperty, TSourceProperty2, TTargetProperty>(
    this TViewModel viewModel,
    Expression<Func<TViewModel, TTargetProperty>> targetProperty,
    Expression<Func<TViewModel, TSourceProperty>> sourceProperty,
    Expression<Func<TViewModel, TSourceProperty2>> sourceProperty2,
    Func<(TSourceProperty, TSourceProperty2), TTargetProperty> converter,
    [CallerFilePath] string sourceFilePath = "",
    [CallerLineNumber] int sourceLineNumber = 0
)
    where TViewModel : BasycReactiveViewModelBase
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyGetter = targetProperty.Compile();
        viewModel.WhenAnyValue(sourceProperty, sourceProperty2)
                .DisposeOldPropertyValue(() => targetPropertyGetter.Invoke(viewModel))
                .Select(converter)
                .BindTo(viewModel, targetProperty!)
                .DisposeWithViewModel(viewModel);

        return targetPropertyGetter.Invoke(viewModel);
    }

    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     (collection to collection)
    ///      with conversion function.
    ///     <br/> Usage: 
    ///     <code>
    ///     [Reactive] public ReadOnlyObservableCollection&lt;int&gt; VmSourceProperty { get; init }
    ///     [Reactive] public ReadOnlyObservableCollection&lt;string&gt; VmTargetProperty { get; init }
    ///     public ViewModel()
    ///     {
    ///          VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty, x=>x.VmSourceProperty, x => x.ToString())
    ///     }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    /// <param name="converter">Function transforming source collection item to target collection item.</param>
    public static ReadOnlyObservableCollection<TTargetItem> ReactiveCollectionProperty<TViewModel, TSourceItem, TTargetItem>(this TViewModel viewModel,
        Expression<Func<TViewModel, ReadOnlyObservableCollection<TTargetItem>>> targetProperty,
        Expression<Func<TViewModel, ReadOnlyObservableCollection<TSourceItem>>> sourceProperty,
        Func<TSourceItem, TTargetItem> converter,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where TViewModel : BasycReactiveViewModelBase
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyName = targetProperty.GetMemberName();
        var backingField = viewModel!.GetType().GetReactivePropertyBackingField(targetPropertyName);

        IDisposable? nestedSubscription = null;
        IDisposable? nestedSubscription2 = null;
        viewModel.WhenAnyValue(sourceProperty)
            .Subscribe(newSourcePropertyValue =>
            {
                nestedSubscription = newSourcePropertyValue.ToObservableChangeSet()
                    .Transform(converter)
                    .Bind(out var vesselsViewModels)
                    .Subscribe()
                    .DisposeWithViewModel(viewModel, nestedSubscription);

                nestedSubscription2 = vesselsViewModels.ToObservableChangeSet()
                    .OnItemRemoved(removedItem =>
                    {
                        if (removedItem is IDisposable removedItemAsDisposable)
                            removedItemAsDisposable.Dispose();
                    })
                    .Subscribe()
                    .DisposeWithViewModel(viewModel, nestedSubscription2);

                backingField.SetValue(viewModel, vesselsViewModels);
                viewModel.RaisePropertyChanged(targetPropertyName);
            })
            .DisposeWithViewModel(viewModel);

        return targetProperty.Compile().Invoke(viewModel);
    }

    //same as ReadOnlyObservableCollection and ReadOnlyObservableCollection
    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     (collection to collection)
    ///      with conversion function.
    ///     <br/> Usage: 
    ///     <code>
    ///     [Reactive] public ReadOnlyObservableCollection&lt;int&gt; VmSourceProperty { get; init }
    ///     [Reactive] public ReadOnlyObservableCollection&lt;string&gt; VmTargetProperty { get; init }
    ///     public ViewModel()
    ///     {
    ///          VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty, x=>x.VmSourceProperty, x => x.ToString())
    ///     }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    /// <param name="converter">Function transforming source collection item to target collection item.</param>
    public static ReadOnlyObservableCollection<TTargetItem> ReactiveCollectionProperty<TViewModel, TSourceItem, TTargetItem>(this TViewModel viewModel,
        Expression<Func<TViewModel, ReadOnlyObservableCollection<TTargetItem>>> targetProperty,
        Expression<Func<TViewModel, ObservableCollection<TSourceItem>>> sourceProperty,
        Func<TSourceItem, TTargetItem> converter,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where TViewModel : BasycReactiveViewModelBase
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyName = targetProperty.GetMemberName();
        var backingField = viewModel!.GetType().GetReactivePropertyBackingField(targetPropertyName);

        IDisposable? nestedSubscription = null;
        IDisposable? nestedSubscription2 = null;
        viewModel.WhenAnyValue(sourceProperty)
            .Subscribe(newSourcePropertyValue =>
            {
                nestedSubscription = newSourcePropertyValue.ToObservableChangeSet()
                    .Transform(converter)
                    .Bind(out var vesselsViewModels)
                    .Subscribe()
                    .DisposeWithViewModel(viewModel, nestedSubscription);

                nestedSubscription2 = vesselsViewModels.ToObservableChangeSet()
                    .OnItemRemoved(removedItem =>
                    {
                        if (removedItem is IDisposable removedItemAsDisposable)
                            removedItemAsDisposable.Dispose();
                    })
                    .Subscribe()
                    .DisposeWithViewModel(viewModel, nestedSubscription2);

                backingField.SetValue(viewModel, vesselsViewModels);
                viewModel.RaisePropertyChanged(targetPropertyName);
            })
            .DisposeWithViewModel(viewModel);

        var initValue = targetProperty.Compile().Invoke(viewModel);
        return initValue;
    }

    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     (collection to property via aggregator)
    ///     <br/> Usage: 
    ///     <code>
    ///     [Reactive] public int VmSourceProperty { get; init }
    ///     [Reactive] public ReadOnlyObservableCollection&lt;int&gt; VmTargetProperty { get; init }
    ///     public ViewModel()
    ///     {
    ///          VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty, x=>x.VmSourceProperty, x => x.Count)
    ///     }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    /// <param name="aggregator">Function transforming whole source collection to target property.</param>
    public static TTargetProperty ReactiveAggregatorProperty<TViewModel, TSourceItem, TTargetProperty>(this TViewModel viewModel,
        Expression<Func<TViewModel, TTargetProperty>> targetProperty,
        Expression<Func<TViewModel, ReadOnlyObservableCollection<TSourceItem>>> sourceProperty,
        Func<IReadOnlyCollection<TSourceItem>, TTargetProperty> aggregator,
        bool listenForItemPropertiesChanges = true,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where TViewModel : BasycReactiveViewModelBase
        //where TSourceItem : INotifyPropertyChanged
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyGetter = targetProperty.Compile();
        var targetPropertyName = targetProperty.GetMemberName();
        var backingField = viewModel.GetType().GetReactivePropertyBackingField(targetPropertyName);
        IDisposable? nestedSubscription = null;
        viewModel.WhenAnyValue(sourceProperty)
            .Subscribe(newSourcePropertyValue =>
            {
                nestedSubscription = newSourcePropertyValue.ToObservableChangeSet()
                    .SetAutoRefresh(listenForItemPropertiesChanges)
                    .ToCollection()
                    .DisposeOldPropertyValue(() => targetPropertyGetter.Invoke(viewModel))
                    .Select(aggregator)
                    .BindTo(viewModel, targetProperty!)
                    .DisposeWithViewModel(viewModel, nestedSubscription);

                DisposeOldPropertyValue(() => targetPropertyGetter.Invoke(viewModel));
                var newTargetPropertyValue = aggregator.Invoke(newSourcePropertyValue);
                backingField.SetValue(viewModel, newTargetPropertyValue);
                viewModel.RaisePropertyChanged(targetPropertyName);
            })
            .DisposeWithViewModel(viewModel);

        ReadOnlyObservableCollection<TSourceItem> initialSourcePropertyValue;
        try
        {
            var sourcePropertyGetter = sourceProperty.Compile();
            initialSourcePropertyValue = sourcePropertyGetter.Invoke(viewModel);
        }
        catch (NullReferenceException)
        {
            //Means that one of the elements in expression path is null. This situation is okay for initial value.
            return default!;
        }

        var initialTargetPropertyValue = aggregator(initialSourcePropertyValue);
        return initialTargetPropertyValue;
    }

    //Same as for ReadonlyObservableCollection
    /// <summary>
    ///     <include file="docs.xml" path='Docs/ReactiveProperty/SummaryStart' />
    ///     (collection to property via aggregator)
    ///     <br/> Usage: 
    ///     <code>
    ///     [Reactive] public int VmSourceProperty { get; init }
    ///     [Reactive] public ObservableCollection&lt;int&gt; VmTargetProperty { get; init }
    ///     public ViewModel()
    ///     {
    ///          VmTargetProperty = this.ReactiveProperty(x=>x.VmTargetProperty, x=>x.VmSourceProperty, x => x.Count)
    ///     }
    ///     </code>
    /// </summary>
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Remarks' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Params' />
    /// <include file="docs.xml" path='Docs/ReactiveProperty/Return' />
    /// <param name="aggregator">Function transforming whole source collection to target property.</param>
    public static TTargetProperty ReactiveAggregatorProperty<TViewModel, TSourceItem, TTargetProperty>(this TViewModel viewModel,
        Expression<Func<TViewModel, TTargetProperty>> targetProperty,
        Expression<Func<TViewModel, ObservableCollection<TSourceItem>>> sourceProperty,
        Func<IReadOnlyCollection<TSourceItem>, TTargetProperty> aggregator,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0
    )
        where TViewModel : BasycReactiveViewModelBase
        where TSourceItem : INotifyPropertyChanged
    {
        AssertPropertyMatchesExpressionInDebug(sourceFilePath, sourceLineNumber, targetProperty);
        var targetPropertyName = targetProperty.GetMemberName();
        var backingField = viewModel.GetType().GetReactivePropertyBackingField(targetPropertyName);
        IDisposable? nestedSubscription = null;
        viewModel.WhenAnyValue(sourceProperty)
            .Subscribe(newSourcePropertyValue =>
            {
                nestedSubscription = newSourcePropertyValue.ToObservableChangeSet()
                    .AutoRefresh()
                    .ToCollection()
                    .Select(aggregator)
                    .BindTo(viewModel, targetProperty!) //TODO dispose old value 
                    .DisposeWithViewModel(viewModel, nestedSubscription);

                var newTargetPropertyValue = aggregator.Invoke(newSourcePropertyValue);
                backingField.SetValue(viewModel, newTargetPropertyValue);
                viewModel.RaisePropertyChanged(targetPropertyName);
            })
            .DisposeWithViewModel(viewModel);

        ObservableCollection<TSourceItem> initialSourcePropertyValue;
        try
        {
            var sourcePropertyGetter = sourceProperty.Compile();
            initialSourcePropertyValue = sourcePropertyGetter.Invoke(viewModel);
        }
        catch (NullReferenceException)
        {
            //Means that one of the elements in expression path is null. This situation is okay for initial value.
            return default!;
        }

        var initialTargetPropertyValue = aggregator(initialSourcePropertyValue);
        return initialTargetPropertyValue;
    }

    private static void AssertPropertyMatchesExpressionInDebug<TSender, TProperty>(string sourceFilePath, int sourceLineNumber,
        Expression<Func<TSender, TProperty>> targetPropertyExpression)
    {

#if DEBUG
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")))
        //	return;

        if (RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
            return;

        var targetPropertyName = targetPropertyExpression.GetMemberName();
        var line = File.ReadLines(sourceFilePath).Skip(sourceLineNumber - 1).Take(1).First();
        string viewModelPropertyName;
        var splitLine = line.Split('=', 2);
        if (splitLine.Length == 2)
        {
            viewModelPropertyName = splitLine.First().Trim();
        }
        else
        {
            line = File.ReadLines(sourceFilePath).Skip(sourceLineNumber - 2).Take(1).First();
            splitLine = line.Split('=', 2);
            if (splitLine.Length != 2)
                throw new InvalidOperationException("Cant check");
            viewModelPropertyName = splitLine.First().Trim();
        }

        if (viewModelPropertyName != targetPropertyName)
            throw new InvalidOperationException(
                $"You must assign to the same property as specified with targetProperty expression. ViewModel property: '{viewModelPropertyName}', target property: '{targetPropertyName}'");
#endif
    }

    private static IObservable<TSource> DisposeOldPropertyValue<TSource, TProperty>(this IObservable<TSource> source, Func<TProperty> propertyGetter) => source.Select(
        x =>
        {
            DisposeOldPropertyValue(propertyGetter);
            return x;
        });

    private static void DisposeOldPropertyValue<TProperty>(Func<TProperty> propertyGetter)
    {
        var oldValue = propertyGetter.Invoke();
        if (oldValue is not null and IDisposable oldValueAsDisposable)
            oldValueAsDisposable.Dispose();
    }
}
