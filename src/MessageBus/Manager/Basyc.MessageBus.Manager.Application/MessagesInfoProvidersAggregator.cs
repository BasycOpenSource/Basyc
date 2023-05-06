using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;

namespace Basyc.MessageBus.Manager.Application;

public class MessagesInfoProvidersAggregator : IMessagesInfoProvidersAggregator
{
    private readonly IMessageInfoProvider[] messageDomainLoaders;
    private IReadOnlyList<MessageGroup>? domainInfos;

    public MessagesInfoProvidersAggregator(IEnumerable<IMessageInfoProvider> messageDomainLoaders)
    {
        this.messageDomainLoaders = messageDomainLoaders.ToArray();
    }

    public IReadOnlyList<MessageGroup> GetMessageGroups()
    {
        domainInfos = messageDomainLoaders.SelectMany(x => x.GetMessageInfos()).ToList();
        return domainInfos;
    }
}
