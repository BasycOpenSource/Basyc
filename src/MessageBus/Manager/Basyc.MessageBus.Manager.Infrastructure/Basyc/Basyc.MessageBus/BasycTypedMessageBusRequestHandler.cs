﻿using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Client;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Formatters;
using Basyc.MessageBus.Shared;
using Microsoft.Extensions.Logging;
using Throw;
using MessageRequest = Basyc.MessageBus.Manager.Application.MessageRequest;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;

public class BasycTypedMessageBusRequestHandler : IRequestHandler
{
	public const string BasycTypedMessageBusRequesterUniqueName = nameof(BasycTypedMessageBusRequestHandler);
	private readonly BusManagerBasycDiagnosticsReceiverTraceIdMapper inMemorySessionMapper;
	private readonly ILogger<BasycTypedMessageBusRequestHandler> logger;

	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;
	private readonly IResponseFormatter responseFormatter;
	private readonly IRequestDiagnosticsManager resultLoggingManager;
	private readonly ITypedMessageBusClient typedMessageBusClient;

	public BasycTypedMessageBusRequestHandler(ITypedMessageBusClient typedMessageBusClient,
		IRequestInfoTypeStorage requestInfoTypeStorage,
		IResponseFormatter responseFormatter,
		IRequestDiagnosticsManager resultLoggingManager,
		BusManagerBasycDiagnosticsReceiverTraceIdMapper inMemorySessionMapper,
		ILogger<BasycTypedMessageBusRequestHandler> logger)
	{
		this.typedMessageBusClient = typedMessageBusClient;
		this.requestInfoTypeStorage = requestInfoTypeStorage;
		this.responseFormatter = responseFormatter;
		this.resultLoggingManager = resultLoggingManager;
		this.inMemorySessionMapper = inMemorySessionMapper;
		this.logger = logger;
	}

	public string UniqueName => BasycTypedMessageBusRequesterUniqueName;

	public void StartRequest(MessageRequest requestContext, ILogger logger)
	{
		//var dummyStartSegment = requestContext.StartNewSegment("BasycTypedMessageBusRequester.StartRequest DUMMy");
		//var startSegment = DiagnosticHelper.Start("BasycTypedMessageBusRequester.StartRequest", dummyStartSegment.TraceId, dummyStartSegment.Id);
		var startSegment = DiagnosticHelper.Start("BasycTypedMessageBusRequester.StartRequest");
		var busRequestContext = new Shared.RequestContext(startSegment.Activity?.SpanId.ToString()!, startSegment.Activity?.TraceId.ToString()!);
		var prepareSegment = DiagnosticHelper.Start("Creating request instance");
		var requestType = requestInfoTypeStorage.GetRequestType(requestContext.Request.MessageInfo);
		var paramValues = requestContext.Request.Parameters.Select(x => x.Value).ToArray();
		var requestObject = Activator.CreateInstance(requestType, paramValues);
		requestObject.ThrowIfNull();
		prepareSegment.Stop();
		this.logger.LogDebug("Request instance created");

		if (requestContext.Request.MessageInfo.HasResponse)
		{
			requestContext.Request.MessageInfo.ResponseType.ThrowIfNull();
			//var busRequestActivity = startSegment.StartNested("BasycTypedMessageBusRequester.StartRequest Bus Request");
			var busRequestActivity = DiagnosticHelper.Start("Bus Request");

			var messageBusClientRequestActivity = DiagnosticHelper.Start("MessageBusClient Request");
			this.logger.LogInformation("Requesting to message bus");

			var busTask = typedMessageBusClient.RequestAsync(requestType, requestObject, requestContext.Request.MessageInfo.ResponseType, busRequestContext);
			messageBusClientRequestActivity.Stop();

			inMemorySessionMapper.AddMapping(requestContext.TraceId, busTask.TraceId);
			busTask.Task.ContinueWith(async x =>
			{
				//await busRequestActivity.Log("MessageBusClient Response received", LogLevel.Information);
				this.logger.LogInformation("MessageBusClient Response received");
				//busRequestActivity.End();
				busRequestActivity.Stop();

				if (x.IsFaulted)
				{
					x.Exception.ThrowIfNull();
					requestContext.Fail(x.Exception.ToString());
					//await startSegment.Log($"Request handeling failed with exception: {x.Exception}", LogLevel.Error);
					this.logger.LogError($"Request handeling failed with exception: {x.Exception}");
				}

				if (x.IsCanceled)
				{
					requestContext.Fail("canceled");
					//await startSegment.Log($"Request handeling was canceled", LogLevel.Error);
					this.logger.LogError("BusTask Canceled");
				}

				if (x.IsCompletedSuccessfully)
				{
					if (x.Result.Value is ErrorMessage error)
					{
						requestContext.Fail(error.Message);
						//await startSegment.Log($"Request handler returned error. {error.Message}", LogLevel.Error);
						this.logger.LogError($"Request handler returned error. {error.Message}");
					}
					else
					{
						var resultObject = x.Result.AsT0;
						requestContext.Complete(responseFormatter.Format(resultObject));
						//await startSegment.Log($"Request completed", LogLevel.Information);
						this.logger.LogInformation("Request completed");
					}
				}

				startSegment.Stop();
				//dummyStartSegment.End();
			});
		}
		else
		{
			//var busStartSegment = startSegment.StartNested("Requesting to bus");
			var busStartSegment = DiagnosticHelper.Start("Requesting to bus");
			var busTask = typedMessageBusClient.SendAsync(requestType, requestObject, busRequestContext);
			inMemorySessionMapper.AddMapping(requestContext.TraceId, busTask.TraceId);

			busTask.Task.ContinueWith(x =>
			{
				busStartSegment.Stop();

				if (x.IsFaulted)
				{
					x.Exception.ThrowIfNull();
					requestContext.Fail(x.Exception.ToString());
					this.logger.LogError($"Request handeling failed with exception: {x.Exception}");
				}

				if (x.IsCanceled)
				{
					requestContext.Fail("canceled");
					this.logger.LogError("Request handeling was canceled");
				}

				if (x.IsCompletedSuccessfully)
				{
					if (x.Result.Value is ErrorMessage error)
					{
						requestContext.Fail(error.Message);
						this.logger.LogError($"Request handler returned error. {error.Message}");
					}
					else
					{
						requestContext.Complete();
						this.logger.LogInformation("Request completed");
					}
				}

				startSegment.Stop();
				//dummyStartSegment.End();
			});
		}
	}
}
