using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentApiDomainInfoProvider : IDomainInfoProvider
{
	private readonly InMemoryRequestHandler inMemoryRequestHandler;
	private readonly IOptions<FluentApiDomainInfoProviderOptions> options;
	private readonly IRequesterSelector requesterSelector;

	public FluentApiDomainInfoProvider(IOptions<FluentApiDomainInfoProviderOptions> options,
		InMemoryRequestHandler inMemoryRequestHandler,
		IRequesterSelector requesterSelector
	)
	{
		this.options = options;
		this.inMemoryRequestHandler = inMemoryRequestHandler;
		this.requesterSelector = requesterSelector;
	}

	public List<MessageGroup> GenerateDomainInfos()
	{
		List<MessageGroup> domainInfos = new();
		foreach (var domain in options.Value.GroupRegistrations)
		{
			var requestInfos = domain.InProgressMessages.Select(inProgressMessage =>
			{
				var requestInfo = inProgressMessage.HasResponse
					? new MessageInfo(inProgressMessage.MessageType, inProgressMessage.Parameters, inProgressMessage.ResponseRunTimeType!,
						inProgressMessage.MessageDisplayName!, inProgressMessage.ResponseRunTimeTypeDisplayName!)
					: new MessageInfo(inProgressMessage.MessageType, inProgressMessage.Parameters, inProgressMessage.MessageDisplayName!);
				inMemoryRequestHandler.AddHandler(requestInfo, inProgressMessage.RequestHandler!);
				requesterSelector.AssignRequesterForMessage(requestInfo, InMemoryRequestHandler.InMemoryDelegateRequesterUniqueName);
				return requestInfo;
			}).ToList();
			domainInfos.Add(new MessageGroup(domain.DomainName!, requestInfos));
		}

		return domainInfos;
	}
}
