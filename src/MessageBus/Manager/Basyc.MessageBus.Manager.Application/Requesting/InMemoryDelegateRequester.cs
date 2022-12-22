using Basyc.MessageBus.Manager.Application.Initialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryDelegateRequester : IRequester
{
    public const string InMemoryDelegateRequesterUniqueName = nameof(InMemoryDelegateRequester);

    private readonly IOptions<InMemoryDelegateRequesterOptions> options;
    private readonly Dictionary<RequestInfo, Action<RequestContext>> handlersMap;
    public string UniqueName => InMemoryDelegateRequesterUniqueName;

    public InMemoryDelegateRequester(IOptions<InMemoryDelegateRequesterOptions> options)
    {
        this.options = options;
        handlersMap = options.Value.ResolveHandlers();
    }

    public void StartRequest(RequestContext requestResult, ILogger requestLogger)
    {
        requestLogger.LogInformation("Starting invoking in-memory delegate");
        var handler = handlersMap[requestResult.Request.RequestInfo];
        try
        {
            Task.Run(() =>
            {
                handler.Invoke(requestResult);
                requestLogger.LogInformation("In-memory delegate completed");
            });

        }
        catch (Exception ex)
        {
            requestLogger.LogInformation("In-memory delegate failed");
            requestResult.Fail(ex.Message);
        }
    }

    public void AddHandler(RequestInfo requestInfo, Action<RequestContext> handler)
    {
        handlersMap.Add(requestInfo, handler);
    }
}
