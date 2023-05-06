using Basyc.MessageBus.Manager.Application;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.RequestTag;

public static class RequestTagTypeHelper
{
    public static RequestTagType FromRequestType(MessageType requestType) => requestType switch
    {
        MessageType.Query => RequestTagType.Query,
        MessageType.Command => RequestTagType.Command,
        MessageType.Generic => RequestTagType.Generic,
        MessageType.Event => RequestTagType.Event,
        _ => throw new NotImplementedException(),
    };
}
