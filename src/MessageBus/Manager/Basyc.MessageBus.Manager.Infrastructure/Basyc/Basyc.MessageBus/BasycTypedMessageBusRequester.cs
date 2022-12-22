using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Client;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Formatters;
using Basyc.MessageBus.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;

public class BasycTypedMessageBusRequester : IRequester
{
	public const string BasycTypedMessageBusRequesterUniqueName = nameof(BasycTypedMessageBusRequester);

	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;
	private readonly IResponseFormatter responseFormatter;
	private readonly IRequestDiagnosticsManager resultLoggingManager;
	private readonly BusManagerBasycDiagnosticsReceiverTraceIDMapper inMemorySessionMapper;
	private readonly ILogger<BasycTypedMessageBusRequester> logger;
	private readonly ITypedMessageBusClient typedMessageBusClient;

	public BasycTypedMessageBusRequester(ITypedMessageBusClient typedMessageBusClient,
		IRequestInfoTypeStorage requestInfoTypeStorage,
		IResponseFormatter responseFormatter,
		IRequestDiagnosticsManager resultLoggingManager,
		BusManagerBasycDiagnosticsReceiverTraceIDMapper inMemorySessionMapper,
		ILogger<BasycTypedMessageBusRequester> logger)
	{
		this.typedMessageBusClient = typedMessageBusClient;
		this.requestInfoTypeStorage = requestInfoTypeStorage;
		this.responseFormatter = responseFormatter;
		this.resultLoggingManager = resultLoggingManager;
		this.inMemorySessionMapper = inMemorySessionMapper;
		this.logger = logger;
	}

	public string UniqueName => BasycTypedMessageBusRequesterUniqueName;

	public void StartRequest(Application.RequestContext requestContext, ILogger requestLogger)
	{
		//var dummyStartSegment = requestContext.StartNewSegment("BasycTypedMessageBusRequester.StartRequest DUMMy");
		//var startSegment = DiagnosticHelper.Start("BasycTypedMessageBusRequester.StartRequest", dummyStartSegment.TraceId, dummyStartSegment.Id);
		var startSegment = DiagnosticHelper.Start("BasycTypedMessageBusRequester.StartRequest");

		var busRequestContext = new Shared.RequestContext(startSegment.Activity?.SpanId.ToString(), startSegment.Activity?.TraceId.ToString());
		var prepareSegment = DiagnosticHelper.Start("Creating request instance");
		var requestType = requestInfoTypeStorage.GetRequestType(requestContext.Request.RequestInfo);
		var paramValues = requestContext.Request.Parameters.Select(x => x.Value).ToArray();
		var requestObject = Activator.CreateInstance(requestType, paramValues);
		prepareSegment.Stop();
		logger.LogInformation("Request istance created");

		if (requestContext.Request.RequestInfo.HasResponse)
		{
			//var busRequestActivity = startSegment.StartNested("BasycTypedMessageBusRequester.StartRequest Bus Request");
			var busRequestActivity = DiagnosticHelper.Start("Bus Request");

			var messageBusClientRequestActivity = DiagnosticHelper.Start("MessageBusClient Request");
			logger.LogInformation("Requesting to message bus");

			var busTask = typedMessageBusClient.RequestAsync(requestType, requestObject, requestContext.Request.RequestInfo.ResponseType, busRequestContext);
			messageBusClientRequestActivity.Stop();

			inMemorySessionMapper.AddMapping(requestContext.TraceId, busTask.TraceId);
			busTask.Task.ContinueWith(async x =>
			{
				//await busRequestActivity.Log("MessageBusClient Response received", LogLevel.Information);
				logger.LogInformation("MessageBusClient Response received");
				//busRequestActivity.End();
				busRequestActivity.Stop();

				if (x.IsFaulted)
				{
					requestContext.Fail(x.Exception.ToString());
					//await startSegment.Log($"Request handeling failed with exception: {x.Exception}", LogLevel.Error);
					logger.LogError($"Request handeling failed with exception: {x.Exception}");
				}

				if (x.IsCanceled)
				{
					requestContext.Fail("canceled");
					//await startSegment.Log($"Request handeling was canceled", LogLevel.Error);
					logger.LogError("BusTask Canceled");

				}

				if (x.IsCompletedSuccessfully)
				{
					if (x.Result.Value is ErrorMessage error)
					{
						requestContext.Fail(error.Message);
						//await startSegment.Log($"Request handler returned error. {error.Message}", LogLevel.Error);
						logger.LogError($"Request handler returned error. {error.Message}");

					}
					else
					{
						var resultObject = x.Result.AsT0;
						requestContext.Complete(responseFormatter.Format(resultObject));
						//await startSegment.Log($"Request completed", LogLevel.Information);
						logger.LogInformation($"Request completed");
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
			var busTask = typedMessageBusClient.SendAsync(requestType, requestObject, requestContext: busRequestContext);
			inMemorySessionMapper.AddMapping(requestContext.TraceId, busTask.TraceId);

			busTask.Task.ContinueWith(x =>
			{
				busStartSegment.Stop();

				if (x.IsFaulted)
				{
					requestContext.Fail(x.Exception.ToString());
					logger.LogError($"Request handeling failed with exception: {x.Exception.ToString()}");
				}

				if (x.IsCanceled)
				{
					requestContext.Fail("canceled");
					logger.LogError($"Request handeling was canceled");
				}

				if (x.IsCompletedSuccessfully)
				{
					if (x.Result.Value is ErrorMessage error)
					{
						requestContext.Fail(error.Message);
						logger.LogError($"Request handler returned error. {error.Message}");
					}
					else
					{
						requestContext.Complete();
						logger.LogInformation($"Request completed");
					}
				}

				startSegment.Stop();
				//dummyStartSegment.End();

			});
		}
	}
}