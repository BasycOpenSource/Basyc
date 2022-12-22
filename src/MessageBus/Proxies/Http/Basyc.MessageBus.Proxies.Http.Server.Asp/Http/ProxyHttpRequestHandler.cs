using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Shared.Http;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;

namespace Basyc.MessageBus.HttpProxy.Server.Asp.Http;

public class ProxyHttpRequestHandler
{
	private readonly IByteMessageBusClient messageBus;
	private readonly IObjectToByteSerailizer serializer;
	private static readonly string proxyRequestSimpleDatatype = TypedToSimpleConverter.ConvertTypeToSimple<RequestHttpDTO>();
	private static readonly string proxyResponseSimpleDataType = TypedToSimpleConverter.ConvertTypeToSimple<ResponseHttpDTO>();

	public ProxyHttpRequestHandler(IByteMessageBusClient messageBus, IObjectToByteSerailizer serializer)
	{
		this.messageBus = messageBus;
		this.serializer = serializer;
	}

	public async Task Handle(HttpContext context)
	{
		MemoryStream httpBodyMemoryStream = new MemoryStream();
		await context.Request.Body.CopyToAsync(httpBodyMemoryStream);
		var proxyRequestBytes = httpBodyMemoryStream.ToArray();
		RequestHttpDTO proxyRequest = (RequestHttpDTO)serializer.Deserialize(proxyRequestBytes, proxyRequestSimpleDatatype);

		if (proxyRequest.HasResponse)
		{
			var busTask = messageBus.RequestAsync(proxyRequest.MessageType, proxyRequest.MessageBytes);
			var busTaskValue = await busTask.Task;
			await busTaskValue.Match(
				async byteResponse =>
				{
					var proxyResponse = new ResponseHttpDTO(busTask.TraceId, byteResponse.ResponseBytes, byteResponse.ResposneType);
					var proxyResponseBytes = serializer.Serialize(proxyResponse, proxyResponseSimpleDataType);
					await context.Response.BodyWriter.WriteAsync(proxyResponseBytes);
				},
				busRequestError => throw new Exception(busRequestError.Message));
		}
		else
		{
			var busTask = messageBus.SendAsync(proxyRequest.MessageType, proxyRequest.MessageBytes);
			var sendResult = await busTask.Task;
			await sendResult.Match(
			async success =>
			{
				var proxyResponse = new ResponseHttpDTO(busTask.TraceId);
				var proxyResponseBytes = serializer.Serialize(proxyResponse, proxyResponseSimpleDataType);
				await context.Response.BodyWriter.WriteAsync(proxyResponseBytes);
			},
			busRequestError => throw new Exception(busRequestError.Message));

		}
	}
}