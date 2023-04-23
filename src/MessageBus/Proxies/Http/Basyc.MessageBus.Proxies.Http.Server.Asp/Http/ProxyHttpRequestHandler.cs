using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Shared.Http;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;

namespace Basyc.MessageBus.HttpProxy.Server.Asp.Http;

public class ProxyHttpRequestHandler
{
    private static readonly string proxyRequestSimpleDatatype = TypedToSimpleConverter.ConvertTypeToSimple<RequestHttpDto>();
    private static readonly string proxyResponseSimpleDataType = TypedToSimpleConverter.ConvertTypeToSimple<ResponseHttpDto>();
    private readonly IByteMessageBusClient messageBus;
    private readonly IObjectToByteSerailizer serializer;

    public ProxyHttpRequestHandler(IByteMessageBusClient messageBus, IObjectToByteSerailizer serializer)
    {
        this.messageBus = messageBus;
        this.serializer = serializer;
    }

    public async Task Handle(HttpContext context)
    {
        var httpBodyMemoryStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(httpBodyMemoryStream);
        var proxyRequestBytes = httpBodyMemoryStream.ToArray();
        var proxyRequest = (RequestHttpDto)serializer.Deserialize(proxyRequestBytes, proxyRequestSimpleDatatype)!;

        if (proxyRequest.HasResponse)
        {
            var busTask = messageBus.RequestAsync(proxyRequest.MessageType, proxyRequest.MessageBytes!);
            var busTaskValue = await busTask.Task;
            await busTaskValue.Match(
                async byteResponse =>
                {
                    var proxyResponse = new ResponseHttpDto(busTask.TraceId, byteResponse.ResponseBytes, byteResponse.ResposneType);
                    var proxyResponseBytes = serializer.Serialize(proxyResponse, proxyResponseSimpleDataType);
                    await context.Response.BodyWriter.WriteAsync(proxyResponseBytes);
                },
                busRequestError => throw new Exception(busRequestError.Message));
        }
        else
        {
            var busTask = messageBus.SendAsync(proxyRequest.MessageType, proxyRequest.MessageBytes!);
            var sendResult = await busTask.Task;
            await sendResult.Match(
                async success =>
                {
                    var proxyResponse = new ResponseHttpDto(busTask.TraceId);
                    var proxyResponseBytes = serializer.Serialize(proxyResponse, proxyResponseSimpleDataType);
                    await context.Response.BodyWriter.WriteAsync(proxyResponseBytes);
                },
                busRequestError => throw new Exception(busRequestError.Message));
        }
    }
}
