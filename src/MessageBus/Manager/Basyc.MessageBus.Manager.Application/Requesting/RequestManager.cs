using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics.Durations;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class RequestManager : IRequestManager
{
    private readonly InMemoryRequestDiagnosticsSource inMemoryRequestDiagnosticsSource;
    private readonly IRequestDiagnosticsRepository requestDiagnosticsRepository;
    private readonly IRequesterSelector requesterSelector;
    private readonly ServiceIdentity requestManagerServiceIdentity;
    private int requestCounter;

    public RequestManager(IRequesterSelector requesterSelector,
        IRequestDiagnosticsRepository loggingManager,
        InMemoryRequestDiagnosticsSource inMemoryRequestDiagnosticsSource)
    {
        this.requesterSelector = requesterSelector;
        requestDiagnosticsRepository = loggingManager;
        this.inMemoryRequestDiagnosticsSource = inMemoryRequestDiagnosticsSource;
        requestManagerServiceIdentity = ServiceIdentity.ApplicationWideIdentity;
        MessageContexts = new ReadOnlyObservableCollection<MessageContext>(Requests);
    }

    public ReadOnlyObservableCollection<MessageContext> MessageContexts { get; }

    private ObservableCollection<MessageContext> Requests { get; } = new();

    public MessageRequest StartRequest(RequestInput request)
    {
        string traceId = Interlocked.Increment(ref requestCounter).ToString().PadLeft(32, '0');
        var messageContext = MessageContexts.FirstOrDefault(x => x.MessageInfo == request.MessageInfo);
        if (messageContext == default)
        {
            messageContext = new MessageContext(request.MessageInfo);
            Requests.Add(messageContext);
        }

        var requestDiagnostics = requestDiagnosticsRepository.CreateDiagnostics(traceId);
        requestDiagnostics.AddLog(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Choosing requester", null);
        var requester = requesterSelector.PickRequester(request.MessageInfo);
        IDurationMapBuilder durationMapBuilder =
            new InMemoryDiagnosticsSourceDurationMapBuilder(requestManagerServiceIdentity, traceId, "root", inMemoryRequestDiagnosticsSource);
        var messageRequest = new MessageRequest(request, DateTime.Now, traceId, durationMapBuilder, requestDiagnostics, messageContext.MessageRequests.Count + 1);
        messageContext.MessageRequests.Add(messageRequest);
        requestDiagnostics.AddLog(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Giving request to requester", null);
        requester.StartRequest(messageRequest, new ResultLoggingContextLogger(requestManagerServiceIdentity, requestDiagnostics));

        return messageRequest;
    }
}
