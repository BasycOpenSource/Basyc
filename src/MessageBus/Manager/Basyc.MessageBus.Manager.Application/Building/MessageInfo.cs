using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;

namespace Basyc.MessageBus.Manager.Application.Building;

public class MessageInfo
{
	public MessageInfo(MessageType requestType, IEnumerable<ParameterInfo> parameters, Type responseType, string requestDisplayName, string responseDisplayName)
		: this(requestType, parameters, requestDisplayName, true, responseType)
	{
		MessageType = requestType;
		ResponseDisplayName = responseDisplayName;
	}

	public MessageInfo(MessageType requestType, IEnumerable<ParameterInfo> parameters, string requestDisplayName)
		: this(requestType, parameters, requestDisplayName, false, null)
	{
	}

	private MessageInfo(MessageType requestType, IEnumerable<ParameterInfo> parameters, string requestDisplayName, bool hasResponse, Type? responseType)
	{
		MessageType = requestType;
		Parameters = parameters.ToList();
		RequestDisplayName = requestDisplayName;
		HasResponse = hasResponse;
		ResponseType = responseType;
	}

	public string RequestDisplayName { get; init; }
	public MessageType MessageType { get; init; }
	public IReadOnlyList<ParameterInfo> Parameters { get; init; }
	public bool HasResponse { get; init; }
	public Type? ResponseType { get; init; }
	public string ResponseDisplayName { get; init; } = string.Empty;

	/// <summary>
	///     Custom metadata that can be created in custom <see cref="IDomainInfoProvider" /> and later be used in custom <see cref="IRequestHandler." />
	/// </summary>
	public Dictionary<string, object> AdditionalMetadata { get; } = new();
}
