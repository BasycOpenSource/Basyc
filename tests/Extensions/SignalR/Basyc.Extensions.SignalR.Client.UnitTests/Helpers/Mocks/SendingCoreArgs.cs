namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

#pragma warning disable CA1819 // Properties should not return arrays
public record SendingCoreArgs(string MethodName, object?[] Args, CancellationToken CancellationToken);
