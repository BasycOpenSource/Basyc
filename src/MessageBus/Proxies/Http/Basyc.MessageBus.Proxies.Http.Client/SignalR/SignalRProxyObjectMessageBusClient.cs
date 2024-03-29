﻿using Basyc.Diagnostics.Shared;
using Basyc.Extensions.SignalR.Client;
using Basyc.MessageBus.Client;
using Basyc.MessageBus.HttpProxy.Client.SignalR.Sessions;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using OneOf;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MessageBus.HttpProxy.Client.Http;

public class SignalRProxyObjectMessageBusClient : IObjectMessageBusClient
{
	private readonly IStrongTypedHubConnectionPusherAndReceiver<IMethodsClientCanCall, IClientMethodsServerCanCall> hubConnection;
	private readonly IObjectToByteSerailizer byteSerializer;
	private readonly SignalRSessionManager sessionManager;

	public SignalRProxyObjectMessageBusClient(IOptions<SignalROptions> options, IObjectToByteSerailizer byteSerializer, ISharedRequestIdCounter requestIdCounter)
	{
		sessionManager = new SignalRSessionManager(requestIdCounter);
		hubConnection = new HubConnectionBuilder()
		.WithUrl(options.Value.SignalRServerUri + options.Value.ProxyClientHubPattern)
		.WithAutomaticReconnect()
		.BuildStrongTyped<IMethodsClientCanCall, IClientMethodsServerCanCall>(sessionManager);
		this.byteSerializer = byteSerializer;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await sessionManager.Start();
		await hubConnection.StartAsync();
	}

	public void Dispose()
	{
		hubConnection.DisposeAsync().GetAwaiter().GetResult();
	}

	public BusTask PublishAsync(string eventType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return CreateAndStartBusTask(eventType, null, requestContext, cancellationToken).ToBusTask();

	}

	public BusTask PublishAsync(string eventType, object eventData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return CreateAndStartBusTask(eventType, eventData, requestContext, cancellationToken).ToBusTask();
	}

	public BusTask<object> RequestAsync(string requestType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return BusTask<object>.FromBusTask(CreateAndStartBusTask(requestType, null, requestContext, cancellationToken), x => x!);

	}

	public BusTask<object> RequestAsync(string requestType, object requestData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return BusTask<object>.FromBusTask(CreateAndStartBusTask(requestType, requestData, requestContext, cancellationToken), x => x!);
	}

	public BusTask SendAsync(string commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return CreateAndStartBusTask(commandType, null, requestContext, cancellationToken).ToBusTask();
	}

	public BusTask SendAsync(string commandType, object commandData, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		return CreateAndStartBusTask(commandType, commandData, requestContext, cancellationToken).ToBusTask();
	}

	private BusTask<object?> CreateAndStartBusTask(string requestType, object? requestData = null, RequestContext requestContext = default, CancellationToken cancellationToken = default)
	{
		var createAndStartBusTaskActivity = DiagnosticHelper.Start("SignalRProxyObjectMessageBusClient.CreateAndStartBusTask", requestContext.TraceId, requestContext.ParentSpanId);
		SignalRSession session = sessionManager.StartSession(requestContext.TraceId);
		var waintingForTaskRunActivity = DiagnosticHelper.Start("Waiting for Task.Run");
		Task<OneOf<object?, ErrorMessage>> reqeustTask = Task.Run(async () => await BustaskMethod(requestType, requestData, requestContext, createAndStartBusTaskActivity, session, waintingForTaskRunActivity));
		return BusTask<object?>.FromTask(session.TraceId, reqeustTask);

	}

	private async Task<OneOf<object?, ErrorMessage>> BustaskMethod(string requestType, object? requestData, RequestContext requestContext, DiagnosticHelperActivityDisposer createAndStartBusTaskActivity, SignalRSession session, DiagnosticHelperActivityDisposer waintingForTaskRunActivity)
	{
		waintingForTaskRunActivity.Stop();
		var busTaskActivity = DiagnosticHelper.Start("BustaskMethod");
		var seriActivity = new Activity("Serializating request").Start();

		if (byteSerializer.TrySerialize(requestData, requestType, out var requestDataBytes, out var error) is false)
		{
			seriActivity.Stop();
			return new ErrorMessage("Failed to serailize request");
		}

		seriActivity.Stop();

		var signalRActivity = DiagnosticHelper.Start("SignalR Sending request");

		try
		{
			await hubConnection.Call.Request(new RequestSignalRDTO(requestType, true, requestDataBytes, RequestContext: requestContext));
		}
		catch (Exception ex)
		{
			signalRActivity.Stop();
			return new ErrorMessage("Failed while requesting. " + ex.Message);
		}

		signalRActivity.Stop();

		var waitingForResult = DiagnosticHelper.Start("Waiting for result");
		var sessionRsult = await session.WaitForCompletion();
		waitingForResult.Stop();

		return sessionRsult.Match<OneOf<object?, ErrorMessage>>(resultDTO =>
		{
			if (resultDTO.HasResponse)
			{
				var deseriActivity = DiagnosticHelper.Start("SignalR Request");

				if (byteSerializer.TryDeserialize(resultDTO.ResponseData!, resultDTO.ResponseType!, out var deserializedResult, out var error) is false)
				{
					busTaskActivity.Stop();
					createAndStartBusTaskActivity.Stop();
					deseriActivity.Stop();
					return new ErrorMessage("Failed to serailize response");
				}

				deseriActivity.Stop();
				busTaskActivity.Stop();
				createAndStartBusTaskActivity.Stop();
				return deserializedResult;

			}
			else
			{
				busTaskActivity.Stop();
				createAndStartBusTaskActivity.Stop();
				return (OneOf<object?, ErrorMessage>)null;
			}
		},
		error =>
		{
			busTaskActivity.Stop();
			createAndStartBusTaskActivity.Stop();
			return error;
		});

	}
}
