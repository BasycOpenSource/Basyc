using Basyc.MessageBus.Client.NetMQ;
using Basyc.MessageBus.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Basyc.MessageBus.InMemory.Tests;

public class NetMqMessageBusClientTests
{
    //private readonly ITypedMessageBusClient client;

    public NetMqMessageBusClientTests()
    {
        var mock = new Mock<ILogger<NetMqByteMessageBusClient>>();
        var logger = mock.Object;
        //client = new NetMQByteMessageBusClient(,logger);
    }

    [Fact]
    public async Task Work()
    {
        //var res = await client.RequestAsync<TestRequest, Customer>();
    }
}

public record TestRequest : IMessage<Customer>;

public record Customer;
