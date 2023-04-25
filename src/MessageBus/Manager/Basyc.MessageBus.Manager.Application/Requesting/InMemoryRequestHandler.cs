using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandler : IRequestHandler
{
    public const string InMemoryDelegateRequesterUniqueName = nameof(InMemoryRequestHandler);

    private readonly Dictionary<MessageInfo, RequestHandler> handlersMap;
    private readonly IOptions<InMemoryRequestHandlerOptions> options;

    public InMemoryRequestHandler(IOptions<InMemoryRequestHandlerOptions> options)
    {
        this.options = options;
        handlersMap = options.Value.ResolveHandlers();
        CtorCounter++;
    }

    public static int CtorCounter { get; private set; }

    public string UniqueName => InMemoryDelegateRequesterUniqueName;

    public void StartRequest(MessageRequest requestResult, ILogger logger)
    {
        //var inMemoryRequesterAct = logger.StartActivity("InMemoryRequester", requestResult.TraceId);
        logger.LogInformation("Starting invoking in-memory delegate");
        //using var findingHandlerActivity = logger.StartActivity("Finding handler");
        bool handlerFound = handlersMap.TryGetValue(requestResult.RequestInput.MessageInfo, out var handler);
        if (handlerFound is false)
        {
            requestResult.Fail(
                $"Requester: '{nameof(InMemoryRequestHandler)}' doesn't have handler for message with display name: '{requestResult.RequestInput.MessageInfo.RequestDisplayName}'");
            //findingHandlerActivity.Stop();
            return;
        }

        //findingHandlerActivity.Stop();
        //var waitingForTaskRun = DiagnosticHelper.Start("Waiting for Task.Run");
        Task.Run(() =>
        {
            //waitingForTaskRun.Stop();
            using var requestActivity = requestResult.Start();
            using var invokingHandlerActivity = logger.StartActivity("Invoking handler", requestActivity.TraceId, requestActivity.Id);
            var handlerOutput = handler.Value().Invoke(requestResult, logger);
            logger.LogInformation("Handler finished");
            var endTime = invokingHandlerActivity.Stop();
            requestActivity.End(endTime);
            requestResult.Stop(endTime);

            if (requestResult.RequestInput.MessageInfo.HasResponse)
                requestResult.SetResponse(handlerOutput);
            else
                requestResult.SetResponse();

            //inMemoryRequesterAct.Stop();
        }).ContinueWith(x =>
        {
            if (x.Status is TaskStatus.Faulted)
            {
                logger.LogError("Handler failed. {Message}", x.Exception.Value().Message);
                requestResult.Fail(x.Exception.Value().Message);
                //inMemoryRequesterAct.Stop();
            }
        });
    }

    public void AddHandler(MessageInfo requestInfo, RequestHandler handler) => handlersMap.Add(requestInfo, handler);
}
