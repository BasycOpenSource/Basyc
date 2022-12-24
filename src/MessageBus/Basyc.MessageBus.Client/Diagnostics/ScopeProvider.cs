using System;
using System.Collections.Generic;
using System.Threading;

namespace Basyc.MessageBus.Client.Diagnostics;

public class HandlerLoggerScopeProvider
{
	private readonly AsyncLocal<Stack<ScopeWrapper>> scopes = new AsyncLocal<Stack<ScopeWrapper>>();
	private readonly AsyncLocal<HandlerScopeState> handlerScopeState = new AsyncLocal<HandlerScopeState>();

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

	public bool TryGetHandlerScope(out HandlerScopeState handlerScopeState)
	{
		handlerScopeState = this.handlerScopeState.Value;
		return this.handlerScopeState.Value != null;
	}

	private class HandlerScopeEnder : IDisposable
	{
		private bool isDisposed = false;
		private readonly Stack<ScopeWrapper> scopes;
		private readonly AsyncLocal<HandlerScopeState> handlerScopeState;

		public HandlerScopeEnder(Stack<ScopeWrapper> scopes, AsyncLocal<HandlerScopeState> handlerScopeState)
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
