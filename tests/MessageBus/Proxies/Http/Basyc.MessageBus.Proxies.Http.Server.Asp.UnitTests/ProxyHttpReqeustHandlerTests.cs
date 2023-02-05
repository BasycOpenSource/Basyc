using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Server.Asp.Http;
using Basyc.MessageBus.HttpProxy.Shared.Http;
using Basyc.MessageBus.Shared;
using Basyc.Serailization.SystemTextJson;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text.Json;
using Xunit;

namespace Basyc.MessageBus.HttpProxy.Server.Asp.Tests;

public class ProxyHttpReqeustHandlerTests
{
	private readonly ProxyHttpRequestHandler handler;
	private readonly Mock<HttpContext> httpContextMock;
	private readonly Mock<IByteMessageBusClient> messageBusMock;
	private readonly IObjectToByteSerailizer serializer;

	public ProxyHttpReqeustHandlerTests()
	{
		messageBusMock = new Mock<IByteMessageBusClient>();
		serializer = new ObjectFromTypedByteSerializer(new JsonByteSerializer());
		handler = new ProxyHttpRequestHandler(messageBusMock.Object, serializer);
		httpContextMock = new Mock<HttpContext>();
	}

	[Fact]
	public async Task Throws_When_MessageBus_Throws()
	{
		var dummyRequestType = TypedToSimpleConverter.ConvertTypeToSimple<DummyRequest>();
		var ser = serializer.Serialize(new DummyRequest(), dummyRequestType);
		var proxyRequest = new RequestHttpDto(dummyRequestType, false);
		var proxyBytes = JsonSerializer.SerializeToUtf8Bytes(proxyRequest);
		var proxyMemory = new MemoryStream(proxyBytes);

		httpContextMock.SetupGet(x => x.Request.Body).Returns(proxyMemory);

		var busErrorMessage = "BUS_ERROR_MESSAGE";
		messageBusMock
			.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<byte[]>(), default, default))
			.Throws(new Exception(busErrorMessage));

		Func<Task> taskWrapper = async () =>
		{
			await handler.Handle(httpContextMock.Object);
		};

		await taskWrapper.Should().ThrowAsync<Exception>().WithMessage($"*{busErrorMessage}*");
	}
}

public record DummyRequest : IMessage;
