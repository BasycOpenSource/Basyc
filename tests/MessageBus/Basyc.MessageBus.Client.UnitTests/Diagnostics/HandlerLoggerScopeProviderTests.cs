using Basyc.MessageBus.Client.Diagnostics;
using Throw;

namespace Basyc.MessageBus.Client.Tests.Diagnostics;

public class HandlerLoggerScopeProviderTests
{
	[Fact]
	public void Should_Throw_When_CallingTwiceBeginHandlerScope()
	{
		var provider = new HandlerLoggerScopeProvider();
		_ = provider.BeginHandlerScope(new HandlerScopeState(1), new DummyScope("1"));
		var action = () => provider.BeginHandlerScope(new HandlerScopeState(2), new DummyScope("2"));
		action.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void Should_Throw_WhenDisposingTwiceBeginHandlerScope()
	{
		var provider = new HandlerLoggerScopeProvider();
		var handlerScope = provider.BeginHandlerScope(new HandlerScopeState(1), new DummyScope("1"));
		handlerScope.Dispose();
		var action = () => handlerScope.Dispose();
		action.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void Test()
	{
		var provider = new HandlerLoggerScopeProvider();
		var sessionId1 = 1;
		var handlerScope = provider.BeginHandlerScope(new HandlerScopeState(sessionId1), new DummyScope("1"));
		var hasHandlerScope = provider.TryGetHandlerScope(out var handlerScopeState);
		hasHandlerScope.Should().BeTrue();
		handlerScopeState.ThrowIfNull();
		handlerScopeState.SessionId.Should().Be(sessionId1);

		handlerScope.Dispose();

		hasHandlerScope = provider.TryGetHandlerScope(out handlerScopeState);
		hasHandlerScope.Should().BeFalse();
		handlerScopeState.Should().BeNull();
	}

	[Fact]
	public async Task Threads_ShouldNot_ShareStates()
	{
		var provider = new HandlerLoggerScopeProvider();
		await Task.Run(() =>
		{
			var sessionId1 = 1;
			var handlerScope = provider.BeginHandlerScope(new HandlerScopeState(sessionId1), new DummyScope("1"));
			var hasHandlerScope = provider.TryGetHandlerScope(out var handlerScopeState);
			hasHandlerScope.Should().BeTrue();
			handlerScopeState.ThrowIfNull();
			handlerScopeState.SessionId.Should().Be(sessionId1);
		});

		await Task.Run(() =>
		{
			var hasHandlerScope2 = provider.TryGetHandlerScope(out var handlerScopeState2);
			hasHandlerScope2.Should().BeFalse();
			handlerScopeState2.Should().BeNull();

			var sessionId2 = 2;
			var handlerScope2 = provider.BeginHandlerScope(new HandlerScopeState(sessionId2), new DummyScope("2"));
			hasHandlerScope2 = provider.TryGetHandlerScope(out handlerScopeState2);
			hasHandlerScope2.Should().BeTrue();
			handlerScopeState2.ThrowIfNull();
			handlerScopeState2.SessionId.Should().Be(sessionId2);
		});
	}

	private class DummyScope : IDisposable
	{
		public DummyScope(string state)
		{
			State = state;
		}

		public string State { get; }

		public void Dispose()
		{
		}
	}
}
