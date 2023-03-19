using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.Generic;
using System.Linq;

namespace Basyc.MessageBus.Manager.Application;

public class DomainInfoProviderManager : IMessagesProvider
{
	private readonly IDomainInfoProvider[] messageDomainLoaders;
	private IReadOnlyList<MessageGroup>? domainInfos;

	public DomainInfoProviderManager(IEnumerable<IDomainInfoProvider> messageDomainLoaders)
	{
		this.messageDomainLoaders = messageDomainLoaders.ToArray();
	}

	public IReadOnlyList<MessageGroup> GetMessageGroups()
	{
		domainInfos = messageDomainLoaders.SelectMany(x => x.GenerateDomainInfos()).ToList();
		return domainInfos;
	}
}
