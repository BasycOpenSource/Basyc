using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Basyc.MessageBus.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Basyc.MessageBus.HttpProxy.Client.SignalR.Sessions;

public class SignalRSessionManager : IClientMethodsServerCanCall
{
	private readonly Channel<object> clientServerChannel = Channel.CreateUnbounded<object>();
	private readonly Dictionary<string, SignalRSession> sessionMap = new();
	private readonly ISharedRequestIdCounter requestIdCounter;

	public SignalRSessionManager(ISharedRequestIdCounter requestIdCounter)
	{
		this.requestIdCounter = requestIdCounter;
	}

	public Task ReceiveRequestFailed(RequestFailedSignalRDTO requestFailed)
	{
		return clientServerChannel.Writer.WriteAsync(requestFailed).AsTask();
	}

	public Task ReceiveRequestResult(ResponseSignalRDTO response)
	{
		return clientServerChannel.Writer.WriteAsync(response).AsTask();
	}

	public Task ReceiveRequestResultMetadata(RequestMetadataSignalRDTO requestMetadata)
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
					case ResponseSignalRDTO response:
						session = sessionMap[response.TraceId];
						session.Complete(response);
						sessionMap.Remove(response.TraceId);
						break;
					case RequestFailedSignalRDTO error:
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
