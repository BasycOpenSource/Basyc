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
	public static RequestTagType FromRequestType(MessageType requestType)
	{
		return requestType switch
		{
			MessageType.Query => RequestTagType.Query,
			MessageType.Command => RequestTagType.Command,
			MessageType.Generic => RequestTagType.Generic,
			MessageType.Event => RequestTagType.Event,
			_ => throw new NotImplementedException(),
		};
	}
}
