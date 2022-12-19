using Basyc.MessageBus.HttpProxy.Client.Http;
using Basyc.Serialization.Abstraction;
using Basyc.Serialization.ProtobufNet;
using Microsoft.Extensions.Options;
using Moq;

namespace Basyc.MessageBus.HttpProxy.Client.Tests
{
    public class HttpProxyClientMessageBusManagerTests
    {
        private readonly HttpProxyObjectMessageBusClient manager;
        private readonly Mock<HttpMessageHandler> httpHandlerMock;

        public HttpProxyClientMessageBusManagerTests()
        {
            httpHandlerMock = new Mock<HttpMessageHandler>();
            var serilizer = new ObjectFromTypedByteSerializer(new ProtobufByteSerializer());
            var options = Options.Create(new HttpProxyObjectMessageBusClientOptions() { ProxyHostUri = new Uri("https://localhost:6969/") });
            manager = new HttpProxyObjectMessageBusClient(options, serilizer);
        }

        //[Fact]
        //public async Task Should_Send_On_Valid()
        //{
        //    var responseContent =

        //    httpHandlerMock.SetupAnyRequest()
        //        .ReturnsResponse(System.Net.HttpStatusCode.OK, new StringContent("TestCon"));

        //    await manager.SendAsync(typeof(object), new object(), default);
        //}

        //[Fact]
        //public async Task Should_Throw_On_Bad_Status_Code()
        //{
        //    var serverErrorMessage = "CustomServerMessage";
        //    httpHandlerMock.SetupAnyRequest()
        //        .ReturnsResponse(System.Net.HttpStatusCode.BadRequest, x => x.ReasonPhrase = serverErrorMessage);

        //    Func<Task> f = async () =>
        //    {
        //        await manager.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple(typeof(object)), new object(), default).Task;
        //    };
        //    await f.Should().ThrowAsync<Exception>().WithMessage($"*{serverErrorMessage}*");
        //}

        //[Fact]
        //public async Task Should_Throw_On_Inner_Exception()
        //{
        //    var exceptionMessage = "ERROR";
        //    httpHandlerMock.SetupAnyRequest().Throws(new Exception(exceptionMessage));
        //    Func<Task> f = async () =>
        //    {
        //        await manager.SendAsync(TypedToSimpleConverter.ConvertTypeToSimple(typeof(object)), new object(), default).Task;
        //    };
        //    await f.Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
        //}
    }
}