using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandler : IRequestHandler
{
    public const string InMemoryDelegateRequesterUniqueName = nameof(InMemoryRequestHandler);
    public static int ctorCounter;

    private readonly Dictionary<MessageInfo, RequestHandlerDelegate> handlersMap;
    private readonly IOptions<InMemoryRequestHandlerOptions> options;

    public InMemoryRequestHandler(IOptions<InMemoryRequestHandlerOptions> options)
    {
        this.options = options;
        handlersMap = options.Value.ResolveHandlers();
        ctorCounter++;
    }

    public string UniqueName => InMemoryDelegateRequesterUniqueName;

    public void StartRequest(MessageRequest request, ILogger logger)
    {
        //var inMemoryRequesterAct = logger.StartActivity("InMemoryRequester", requestResult.TraceId);
        logger.LogInformation("Starting invoking in-memory delegate");
        //using var findingHandlerActivity = logger.StartActivity("Finding handler");
        var handlerFound = handlersMap.TryGetValue(request.Request.MessageInfo, out var handler);
        if (handlerFound is false)
        {
            request.Fail(
                $"Requester: '{nameof(InMemoryRequestHandler)}' doesn't have handler for message with display name: '{request.Request.MessageInfo.RequestDisplayName}'");
            //findingHandlerActivity.Stop();
            return;
        }

        //findingHandlerActivity.Stop();
        //var waitingForTaskRun = DiagnosticHelper.Start("Waiting for Task.Run");
        Task.Run(() =>
        {
            //waitingForTaskRun.Stop();
            using var requestActivity = request.Start();
            using var invokingHandlerActivity = logger.StartActivity("Invoking handler", requestActivity.TraceId, requestActivity.Id);
            var handlerOutput = handler.Value().Invoke(request, logger);
            logger.LogInformation("Handler finished");
            var endTime = invokingHandlerActivity.Stop();
            requestActivity.End(endTime);
            request.Stop(endTime);

            if (request.Request.MessageInfo.HasResponse)
                request.SetResponse(handlerOutput);
            else
                request.SetResponse();

            //inMemoryRequesterAct.Stop();
        }).ContinueWith(x =>
        {
            if (x.Status is TaskStatus.Faulted)
            {
                logger.LogError($"Handler failed. {x.Exception.Value().Message}");
                request.Fail(x.Exception.Value().Message);
                //inMemoryRequesterAct.Stop();
            }
        });
    }

    public void AddHandler(MessageInfo requestInfo, RequestHandlerDelegate handler) => handlersMap.Add(requestInfo, handler);
}
