using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Basyc.MessageBus.Shared;
using System.Threading.Channels;

namespace Basyc.MessageBus.HttpProxy.Client.SignalR.Sessions;

public class SignalRSessionManager : IClientMethodsServerCanCall
{
	private readonly Channel<object> clientServerChannel = Channel.CreateUnbounded<object>();
	private readonly ISharedRequestIdCounter requestIdCounter;
	private readonly Dictionary<string, SignalRSession> sessionMap = new();

	public SignalRSessionManager(ISharedRequestIdCounter requestIdCounter)
	{
		this.requestIdCounter = requestIdCounter;
	}

	public Task ReceiveRequestFailed(RequestFailedSignalRDto requestFailed)
	{
		return clientServerChannel.Writer.WriteAsync(requestFailed).AsTask();
	}

	public Task ReceiveRequestResult(ResponseSignalRDto response)
	{
		return clientServerChannel.Writer.WriteAsync(response).AsTask();
	}

	public Task ReceiveRequestResultMetadata(RequestMetadataSignalRDto requestMetadata)
	{
		//Not used for now
		return Task.CompletedTask;
	}

	public Task Start()
	{
		Task.Run(async () =>
		{
			await foreach (var responseObject in clientServerChannel.Reader.ReadAllAsync())
			{
				SignalRSession session = default;

				switch (responseObject)
				{
					case ResponseSignalRDto response:
						session = sessionMap[response.TraceId];
						session.Complete(response);
						sessionMap.Remove(response.TraceId);
						break;
					case RequestFailedSignalRDto error:
						session = sessionMap[error.TraceId];
						session.Complete(new ErrorMessage(error.Message));
						sessionMap.Remove(error.TraceId);
						break;
					default:
						throw new ArgumentException("message not recognized");
				}
			}
		});

		return Task.CompletedTask;
	}

	public SignalRSession StartSession(string traceId)
	{
		//var sessionIndex = requestIdCounter.GetNextId();
		//var traceId = sessionIndex.ToString();
		var session = new SignalRSession(traceId, traceId);
		sessionMap.Add(traceId, session);
		return session;
	}
}
