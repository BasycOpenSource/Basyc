namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

#pragma warning disable CA1819 // Properties should not return arrays
public record HubProtocolMockMessage(string Target, object?[] Arguments);
