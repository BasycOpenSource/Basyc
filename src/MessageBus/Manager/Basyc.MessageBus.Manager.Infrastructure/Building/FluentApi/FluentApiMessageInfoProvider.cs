using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentApiMessageInfoProvider : IMessageInfoProvider
{
    private readonly InMemoryRequestHandler inMemoryRequestHandler;
    private readonly IOptions<FluentApiDomainInfoProviderOptions> options;
    private readonly IRequesterSelector requesterSelector;

    public FluentApiMessageInfoProvider(IOptions<FluentApiDomainInfoProviderOptions> options,
        InMemoryRequestHandler inMemoryRequestHandler,
        IRequesterSelector requesterSelector
    )
    {
        this.options = options;
        this.inMemoryRequestHandler = inMemoryRequestHandler;
        this.requesterSelector = requesterSelector;
    }

    public List<MessageGroup> GetMessageInfos()
    {
        List<MessageGroup> domainInfos = new();
        return domainInfos;
        //TODO: maybe remove - probably will be repalced with CommonMessageInfoProvider
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
            domainInfos.Add(new MessageGroup(domain.Name!, requestInfos));
        }

        return domainInfos;
    }
}
