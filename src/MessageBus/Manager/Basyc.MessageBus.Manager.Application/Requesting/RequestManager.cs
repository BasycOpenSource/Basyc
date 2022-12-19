using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics.Durations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Basyc.MessageBus.Manager.Application.Requesting
{
	public class RequestManager : IRequestManager
	{
		private readonly IRequesterSelector requesterSelector;
		private readonly IRequestDiagnosticsManager requestDiagnosticsManager;
		private readonly InMemoryRequestDiagnosticsSource inMemoryRequestDiagnosticsSource;
		private int requestCounter;
		private readonly ServiceIdentity requestManagerServiceIdentity;

		public RequestManager(IRequesterSelector requesterSelector, IRequestDiagnosticsManager loggingManager, InMemoryRequestDiagnosticsSource inMemoryRequestDiagnosticsSource)
		{
			this.requesterSelector = requesterSelector;
			this.requestDiagnosticsManager = loggingManager;
			this.inMemoryRequestDiagnosticsSource = inMemoryRequestDiagnosticsSource;
			requestManagerServiceIdentity = ServiceIdentity.ApplicationWideIdentity;
		}

		public Dictionary<RequestInfo, List<RequestContext>> Results { get; } = new Dictionary<RequestInfo, List<RequestContext>>();


		public RequestContext StartRequest(Request request)
		{
			var traceId = Interlocked.Increment(ref requestCounter).ToString().PadLeft(32, '0');
			if (Results.TryGetValue(request.RequestInfo, out var reqeustContexts) is false)
			{
				reqeustContexts = new List<RequestContext>();
				Results.Add(request.RequestInfo, reqeustContexts);
			}

			RequestDiagnosticContext requestDiagnostics = requestDiagnosticsManager.CreateDiagnostics(traceId);
			requestDiagnostics.Log(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Choosing requester", null);
			var requester = requesterSelector.PickRequester(request.RequestInfo);
			IDurationMapBuilder durationMapBuilder = new InMemoryDiagnosticsSourceDurationMapBuilder(requestManagerServiceIdentity, traceId, "root", inMemoryRequestDiagnosticsSource);
			var requestContext = new RequestContext(request, DateTime.Now, traceId, durationMapBuilder, requestDiagnostics);
			reqeustContexts.Add(requestContext);
			requestDiagnostics.Log(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Giving request to requester", null);
			var dummyStartSegment = durationMapBuilder.StartNewSegment("RequesterManager Dummy1");
			var startSegment = DiagnosticHelper.Start("RequesterManager Dummy2", dummyStartSegment.TraceId, dummyStartSegment.Id);
			requester.StartRequest(requestContext, new ResultLoggingContextLogger(requestManagerServiceIdentity, requestDiagnostics));
			startSegment.Stop();
			dummyStartSegment.End();
			requestDiagnostics.Log(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Requester finished", null);


			return requestContext;
		}
	}
}
