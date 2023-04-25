using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;

public class CommonMessageInfoProvider : IMessageInfoProvider
{
    private readonly IOptions<CommonMessageInfoProviderOptions> options;
    private readonly InMemoryRequestHandler inMemoryRequestHandler;
    private readonly IRequesterSelector requesterSelector;

    public CommonMessageInfoProvider(IOptions<CommonMessageInfoProviderOptions> options, InMemoryRequestHandler inMemoryRequestHandler, IRequesterSelector requesterSelector)
    {
        this.options = options;
        this.inMemoryRequestHandler = inMemoryRequestHandler;
        this.requesterSelector = requesterSelector;
    }

    public List<MessageGroup> GetMessageInfos()
    {
        var messageGroups = new List<MessageGroup>(options.Value.MessageGroupRegistration.Count);
        foreach (var messageGroupRegistration in options.Value.MessageGroupRegistration)
        {
            var messageInfos = new List<MessageInfo>(messageGroupRegistration.MessageRegistrations.Count);
            foreach (var messageRegistration in messageGroupRegistration.MessageRegistrations)
            {
                var messageInfo = messageRegistration.HasResponse ?
                    new MessageInfo(MessageType.Generic, messageRegistration.Parameters, messageRegistration.ResponseRunTimeType.Value(), messageRegistration.MessageDisplayName.Value(), messageRegistration.ResponseRunTimeTypeDisplayName.Value())
                    : new MessageInfo(MessageType.Generic, messageRegistration.Parameters, messageRegistration.MessageDisplayName.Value());

                messageInfos.Add(messageInfo);
                if (messageRegistration.HandlerDelegate is not null && messageRegistration.HandlerUniqueName is null or InMemoryRequestHandler.InMemoryDelegateRequesterUniqueName)
                {
                    inMemoryRequestHandler.AddHandler(messageInfo, messageRegistration.HandlerDelegate);
                    requesterSelector.AssignRequesterForMessage(messageInfo, inMemoryRequestHandler.UniqueName);
                }
            }

            var group = new MessageGroup(messageGroupRegistration.Name, messageInfos);
            messageGroups.Add(group);
        }

        return messageGroups;
    }
}
