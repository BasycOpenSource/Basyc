using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Client.Diagnostics;

public class HandlerLoggerScopeProvider
{
	private readonly AsyncLocal<HandlerScopeState?> handlerScopeState = new();
	private readonly AsyncLocal<Stack<ScopeWrapper>> scopes = new();

	public IDisposable BeginHandlerScope(HandlerScopeState state, IDisposable normalLoggerScope)
	{
		if (handlerScopeState.Value is not null)
		{
			throw new InvalidOperationException($"Can't call {nameof(BeginHandlerScope)} twice in same async context.");
		}

		handlerScopeState.Value = state;
		scopes.Value = new Stack<ScopeWrapper>();
		scopes.Value.Push(new ScopeWrapper(state, normalLoggerScope));
		return new HandlerScopeEnder(scopes.Value, handlerScopeState);
	}

	public bool TryGetHandlerScope([NotNullWhen(true)] out HandlerScopeState? handlerScopeState)
	{
		handlerScopeState = this.handlerScopeState.Value;
		return this.handlerScopeState.Value != null;
	}

	private class HandlerScopeEnder : IDisposable
	{
		private readonly AsyncLocal<HandlerScopeState?> handlerScopeState;
		private readonly Stack<ScopeWrapper> scopes;
		private bool isDisposed;

		public HandlerScopeEnder(Stack<ScopeWrapper> scopes, AsyncLocal<HandlerScopeState?> handlerScopeState)
		{
			this.scopes = scopes;
			this.handlerScopeState = handlerScopeState;
		}

		public void Dispose()
		{
			if (isDisposed is true)
			{
				throw new InvalidOperationException("Can't dispose twice");
			}

			var scopeWrapper = scopes.Pop();
			scopeWrapper.NormalLoggerScope.Dispose();
			handlerScopeState.Value = null;
			isDisposed = true;
		}
	}

	private record ScopeWrapper(HandlerScopeState State, IDisposable NormalLoggerScope);
}
