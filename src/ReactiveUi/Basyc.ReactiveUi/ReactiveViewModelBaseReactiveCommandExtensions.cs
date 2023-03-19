using ReactiveUI;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Basyc.ReactiveUi;
/// <summary>
/// Class containing extension methods for <see cref="BasycReactiveViewModelBase"/> for creating reactive commands
/// </summary>
public static class ReactiveViewModelBaseReactiveCommandExtensions
{
	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     <br/> Usage: 
	///     <code>
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand(() => Console.WriteLine())
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	public static ICommand ReactiveCommand<TSource>(this BasycReactiveViewModelBase viewModel, Action execute, IObservable<TSource>? canExecuteChanged = null,
		Func<bool>? canExecute = null)
	{
		canExecute ??= () => true;
		canExecuteChanged ??= Observable.Repeat<TSource>(default(TSource)!, 1); //Invoke can execute changed once
		var rxCanExecute = canExecuteChanged.Select(x => canExecute());
		return ReactiveUI.ReactiveCommand.Create(execute, rxCanExecute)
			.DisposeWithViewModel(viewModel);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     <br/> Usage: 
	///     <code>
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand(() => Console.WriteLine())
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	public static ICommand ReactiveCommand(this BasycReactiveViewModelBase viewModel, Action execute)
	{
		return ReactiveCommand<bool>(viewModel, execute, default, default);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     <br/> Usage: 
	///     <code>
	///     public IObservable&lt;bool&gt; Observable { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand(() => Console.WriteLine(), Observable)
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	/// <param name="canExecuteChanged">IObservable that will update <see cref="ICommand"/>'s <see cref="ICommand.CanExecuteChanged"/> </param>
	public static ICommand ReactiveCommand(this BasycReactiveViewModelBase viewModel, Action execute, IObservable<bool> canExecuteChanged)
	{
		return ReactiveUI.ReactiveCommand
		.Create(execute, canExecuteChanged)
		.DisposeWithViewModel(viewModel);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     (CanExecute from reactive property + converter)
	///     <br/> Usage: 
	///     <code>
	///     [Reactive] public string ReactiveProperty { get; init }
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand(() => Console.WriteLine(), x => x.ReactiveProperty, x => x == "canExecute")
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	/// <param name="sourceProperty">Selector of property which will update <see cref="ICommand"/>'s <see cref="ICommand.CanExecuteChanged"/> .</param>
	/// <param name="converter">Converter that will convert <see cref="sourceProperty"/> into <see cref="ICommand.CanExecuteChanged"/> .</param>
	public static ICommand ReactiveCommand<TViewModel, TVmProperty>(this TViewModel viewModel, Action execute, Expression<Func<TViewModel, TVmProperty>> sourceProperty,
		Func<TVmProperty, bool> converter)
		where TViewModel : BasycReactiveViewModelBase
	{
		var rxCanExecute = viewModel.WhenAnyValue(sourceProperty).Select(converter);
		return ReactiveUI.ReactiveCommand.Create(execute, rxCanExecute).DisposeWithViewModel(viewModel);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     (CanExecute from reactive property)
	///     <br/> Usage: 
	///     <code>
	///     [Reactive] public bool CanExecute { get; init }
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand(() => Console.WriteLine(), x => x.CanExecute)
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	/// <param name="sourceProperty">Selector of property which will update <see cref="ICommand"/>'s <see cref="ICommand.CanExecuteChanged"/> .</param>
	public static ICommand ReactiveCommand<TViewModel>(this TViewModel viewModel, Action execute, Expression<Func<TViewModel, bool>> sourceProperty)
		where TViewModel : BasycReactiveViewModelBase
	{
		var rxCanExecute = viewModel.WhenAnyValue(sourceProperty);
		return ReactiveUI.ReactiveCommand.Create(execute, rxCanExecute).DisposeWithViewModel(viewModel);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     (CanExecute from IObservable&lt;TSource> + canExecute delegate)
	///     <br/> Usage: 
	///     <code>
	///     [Reactive] public int Age { get; init; }
	///     public IObservable&lt;TSource> Observable { get; init }
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         Observable = ...
	///         ReactiveCommand = this.ReactiveCommand&lt;string>(x => Console.WriteLine(x), Observable, () => Age > 18)
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	/// <param name="canExecuteChanged">IObservable that will together with <see cref="canExecuteChanged"/> update <see cref="ICommand"/>'s <see cref="ICommand.CanExecuteChanged"/> </param>
	/// <param name="canExecute">delegating used to set <see cref="ICommand"/>'s <see cref="ICommand.CanExecuteChanged"/>. Called with every change from <see cref="canExecuteChanged"/> </param>
	public static ICommand ReactiveCommand<TCommandParameter, TSource>(this BasycReactiveViewModelBase viewModel, Action<TCommandParameter> execute,
		IObservable<TSource>? canExecuteChanged = null,
		Func<bool>? canExecute = null)
	{
		canExecute ??= () => true;
		canExecuteChanged ??= Observable.Repeat<TSource>(default(TSource)!, 1); //Invoke can execute changed once
		var rxCanExecute = canExecuteChanged.Select(x => canExecute());
		return ReactiveUI.ReactiveCommand.Create(execute, rxCanExecute).DisposeWithViewModel(viewModel);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     <br/> Usage: 
	///     <code>
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand&lt;string>(x => Console.WriteLine(x)e)
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	public static ICommand ReactiveCommand<TCommandParameter>(this BasycReactiveViewModelBase viewModel, Action<TCommandParameter> execute)
	{
		return ReactiveCommand<TCommandParameter, bool>(viewModel, execute, default, default);
	}

	/// <summary>
	///     <include file="docs.xml" path='Docs/ReactiveCommand/SummaryStart' />
	///     (CanExecute from reactive property)
	///     <br/> Usage: 
	///     <code>
	///     [Reactive] public bool CanExecute { get; init }
	///     public ICommand ReactiveCommand { get; init }
	///     public ViewModel()
	///     {
	///         ReactiveCommand = this.ReactiveCommand&lt;ViewModel,string>(x => Console.WriteLine(x), x => x.CanExecute)
	///     }
	///     </code>
	/// </summary>
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Remarks' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Params' />
	/// <include file="docs.xml" path='Docs/ReactiveCommand/Return' />
	/// <param name="sourceProperty">Selector of property which will update <see cref="ICommand"/>'s <see cref="ICommand.CanExecuteChanged"/> .</param>
	public static ICommand ReactiveCommand<TViewModel, TCommandParameter>(this TViewModel viewModel, Action<TCommandParameter> execute,
		Expression<Func<TViewModel, bool>> sourceProperty)
		where TViewModel : BasycReactiveViewModelBase
	{
		var rxCanExecute = viewModel.WhenAnyValue(sourceProperty);
		return ReactiveUI.ReactiveCommand.Create(execute, rxCanExecute).DisposeWithViewModel(viewModel);
	}
}
