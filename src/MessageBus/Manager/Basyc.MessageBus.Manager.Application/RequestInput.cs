using Basyc.MessageBus.Manager.Application.Building;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Application;

//public class Request
//{
//	public MessageInfo RequestInfo { get; init; }
//	public IReadOnlyCollection<Parameter> Parameters { get; init; }

//	public Request(MessageInfo requestInfo, IEnumerable<Parameter> parameters)
//	{
//		RequestInfo = requestInfo;
//		Parameters = parameters.ToList();
//	}
//}

public record class RequestInput(MessageInfo MessageInfo, ReadOnlyCollection<Parameter> Parameters);
