using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Basyc.MessageBus.Manager.Application.Building;

public class FluentApiDomainInfoProvider : IDomainInfoProvider
{
	private readonly IOptions<FluentApiDomainInfoProviderOptions> options;
	private readonly InMemoryDelegateRequester inMemoryDelegateRequester;
	private readonly IRequesterSelector requesterSelector;

	public FluentApiDomainInfoProvider(IOptions<FluentApiDomainInfoProviderOptions> options,
		InMemoryDelegateRequester inMemoryDelegateRequester,
		IRequesterSelector requesterSelector
		)
	{
		this.options = options;
		this.inMemoryDelegateRequester = inMemoryDelegateRequester;
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
				{
					requestInfo = new RequestInfo(inProgressMessage.MessageType, inProgressMessage.Parameters, inProgressMessage.ResponseRunTimeType!, inProgressMessage.MessagDisplayName!, inProgressMessage.ResponseRunTimeTypeDisplayName!);
				}
				else
				{
					requestInfo = new RequestInfo(inProgressMessage.MessageType, inProgressMessage.Parameters, inProgressMessage.MessagDisplayName!);
				}

				inMemoryDelegateRequester.AddHandler(requestInfo, inProgressMessage.RequestHandler!);
				requesterSelector.AssignRequester(requestInfo, InMemoryDelegateRequester.InMemoryDelegateRequesterUniqueName);
				return requestInfo;
			}).ToList();
			domainInfos.Add(new(domain.DomainName!, requestInfos));
		}

		return domainInfos;

	}
}
