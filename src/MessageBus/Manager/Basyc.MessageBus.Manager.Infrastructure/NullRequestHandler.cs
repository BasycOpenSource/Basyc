﻿using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class NullRequestHandler : IRequestHandler
{
    public string UniqueName => nameof(NullRequestHandler);

    public void StartRequest(MessageRequest requestResult, ILogger logger)
    {
        if (requestResult.RequestInput.MessageInfo.HasResponse)
            requestResult.SetResponse("NullRequester dummy response");
        else
            requestResult.SetResponse();
    }
}
