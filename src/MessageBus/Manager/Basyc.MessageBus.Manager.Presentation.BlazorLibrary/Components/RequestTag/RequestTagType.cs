using Basyc.MessageBus.Manager.Application;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.RequestTag;

public enum RequestTagType
{
    Query,
    Command,
    Response,
    Generic,
    Event,
}

public static class RequestTagTypeHelper
{
    public static RequestTagType FromRequestType(RequestType requestType)
    {
        return requestType switch
        {
            RequestType.Query => RequestTagType.Query,
            RequestType.Command => RequestTagType.Command,
            RequestType.Generic => RequestTagType.Generic,
            RequestType.Event => RequestTagType.Event,
            _ => throw new NotImplementedException(),
        };
    }
}
