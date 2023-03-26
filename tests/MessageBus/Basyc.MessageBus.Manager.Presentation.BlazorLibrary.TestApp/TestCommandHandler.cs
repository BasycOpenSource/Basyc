using Basyc.MessageBus.Client.RequestResponse;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.TestApp;

public class TestCommandHandler : IMessageHandler<TestCommand>
{
	public Task Handle(TestCommand message, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}
}
