using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class NullRequester : IRequester
{
    public string UniqueName => nameof(NullRequester);

    public void StartRequest(RequestContext requestResult, ILogger requestLogger)
    {
        if (requestResult.Request.RequestInfo.HasResponse)
        {
            requestResult.Complete("NullRequester dummy response");
        }
        else
        {
            requestResult.Complete();
        }
    }
}
