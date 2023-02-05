using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Basyc.MessageBus.HttpProxy.Server.Asp.SignalR;

public partial class ProxyClientHub : Hub<IClientMethodsServerCanCall>, IMethodsClientCanCall
{
	private readonly ILogger<ProxyClientHub> logger;
	private readonly IByteMessageBusClient messageBus;

	public ProxyClientHub(IByteMessageBusClient messageBus, ILogger<ProxyClientHub> logger)
	{
		this.messageBus = messageBus;
		this.logger = logger;
	}

	public async Task Request(RequestSignalRDto proxyRequest)
	{
		var reqeustActivity = DiagnosticHelper.Start($"{nameof(ProxyClientHub)}.{nameof(Request)}", proxyRequest.RequestContext.TraceId,
			proxyRequest.RequestContext.ParentSpanId);
		LogRequestReceived(proxyRequest.MessageType);
		if (proxyRequest.HasResponse)
		{
			var busTask = messageBus.RequestAsync(proxyRequest.MessageType, proxyRequest.MessageBytes!, proxyRequest.RequestContext);
			await Clients.Caller.ReceiveRequestResultMetadata(new RequestMetadataSignalRDto(busTask.TraceId));
			var busTaskValue = await busTask.Task;
			LogBusResponseReceived(proxyRequest.MessageType);
			await busTaskValue.Match(
				async byteResponse =>
				{
					var response = new ResponseSignalRDto(busTask.TraceId, true, byteResponse.ResponseBytes, byteResponse.ResposneType);
					LogSendingResponse(proxyRequest.MessageType);
					await Clients.Caller.ReceiveRequestResult(response);
				},
				async busRequestError =>
				{
					var failure = new RequestFailedSignalRDto(busTask.TraceId, busRequestError.Message);
					LogSendingResponse(proxyRequest.MessageType);
					await Clients.Caller.ReceiveRequestFailed(failure);
				});
		}
		else
		{
			var busTask = messageBus.SendAsync(proxyRequest.MessageType, proxyRequest.MessageBytes!, proxyRequest.RequestContext);
			var busTaskValue = await busTask.Task;
			await busTaskValue.Match(
				async success =>
				{
					var response = new ResponseSignalRDto(busTask.TraceId, false);
					LogSendingResponse(proxyRequest.MessageType);
					await Clients.Caller.ReceiveRequestResult(response);
				},
				async error =>
				{
					var failure = new RequestFailedSignalRDto(busTask.TraceId, error.Message);
					LogSendingResponse(proxyRequest.MessageType);
					await Clients.Caller.ReceiveRequestFailed(failure);
				});
		}

		reqeustActivity.Stop();
	}

	[LoggerMessage(0, LogLevel.Information, "Received {messageType}")]
	partial void LogRequestReceived(string messageType);

	[LoggerMessage(1, LogLevel.Information, "Bus response for {messageType} received")]
	partial void LogBusResponseReceived(string messageType);

	[LoggerMessage(2, LogLevel.Information, "Sending {messageType} response")]
	partial void LogSendingResponse(string messageType);
}
