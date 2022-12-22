namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

public record SendingCoreArgs(string MethodName, object?[] Args, CancellationToken CancellationToken);