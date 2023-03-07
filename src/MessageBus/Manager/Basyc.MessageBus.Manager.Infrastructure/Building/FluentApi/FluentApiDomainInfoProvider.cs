using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Building;

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

	public List<DomainInfo> GenerateDomainInfos()
	{
		List<DomainInfo> domainInfos = new();
		foreach (var domain in options.Value.DomainRegistrations)
		{
			var requestInfos = domain.InProgressMessages.Select(inProgressMessage =>
			{
				RequestInfo requestInfo;
				if (inProgressMessage.HasResponse)
					requestInfo = new RequestInfo(inProgressMessage.MessageType, inProgressMessage.Parameters, inProgressMessage.ResponseRunTimeType!,
						inProgressMessage.MessagDisplayName!, inProgressMessage.ResponseRunTimeTypeDisplayName!);
				else
					requestInfo = new RequestInfo(inProgressMessage.MessageType, inProgressMessage.Parameters, inProgressMessage.MessagDisplayName!);

				inMemoryRequestHandler.AddHandler(requestInfo, inProgressMessage.RequestHandler!);
				requesterSelector.AssignRequester(requestInfo, InMemoryRequestHandler.InMemoryDelegateRequesterUniqueName);
				return requestInfo;
			}).ToList();
			domainInfos.Add(new DomainInfo(domain.DomainName!, requestInfos));
		}

		return domainInfos;
	}
}
