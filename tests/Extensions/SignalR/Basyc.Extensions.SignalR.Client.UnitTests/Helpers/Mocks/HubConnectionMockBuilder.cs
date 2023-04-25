using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Pipelines;
using System.Net;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

public class HubConnectionMockBuilder : IHubConnectionBuilder
{
    public HubConnectionMockBuilder()
    {
    }

    public IServiceCollection Services => throw new NotImplementedException();

    public static HubConnectionMock Create()
    {
        var pipe = new Pipe();
        var connectionFactory = new ConnectionFactoryMock(pipe);
        var hubProtocol = new HubProtocolMock();
        var endpoint = new IPEndPoint(0, 0);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(x => x.AddDebug());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var retryPolicyMock = new Mock<IRetryPolicy>();
        var hubConnectionMock = new HubConnectionMock(pipe, connectionFactory, hubProtocol, endpoint, serviceProvider, loggerFactory, retryPolicyMock.Object);
        hubConnectionMock.ServerTimeout = TimeSpan.FromSeconds(1000);
        hubConnectionMock.HandshakeTimeout = TimeSpan.FromSeconds(1000);
        hubConnectionMock.KeepAliveInterval = TimeSpan.FromSeconds(1000);
        hubConnectionMock.Closed += HubConnection_Closed;
        hubConnectionMock.Reconnecting += HubConnection_Reconnecting;
        hubConnectionMock.Reconnected += HubConnection_Reconnected;

        return hubConnectionMock;
    }

    public HubConnection Build() => Create();

    private static Task HubConnection_Reconnected(string? arg) => Task.CompletedTask;

    private static Task HubConnection_Reconnecting(Exception? arg) => Task.CompletedTask;

    private static Task HubConnection_Closed(Exception? arg) => Task.CompletedTask;
}

#pragma warning disable SA1402
#pragma warning disable SA1204
public static class Extensions
#pragma warning restore SA1204
#pragma warning restore SA1402
{
    public static HubConnectionMock BuildAsMock(this HubConnectionMockBuilder builder) => HubConnectionMockBuilder.Create();
}
