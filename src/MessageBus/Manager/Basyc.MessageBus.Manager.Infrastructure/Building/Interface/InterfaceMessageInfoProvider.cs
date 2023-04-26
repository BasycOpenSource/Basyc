using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class InterfaceMessageInfoProvider : IMessageInfoProvider
{
    private readonly IOptions<InterfaceDomainProviderOptions> options;
    private readonly IEnumerable<IRequestHandler> requesters;
    private readonly IRequesterSelector requesterSelector;
    private readonly IRequestInfoTypeStorage requestInfoTypeStorage;

    public InterfaceMessageInfoProvider(IOptions<InterfaceDomainProviderOptions> options,
        IRequesterSelector requesterSelector,
        IRequestInfoTypeStorage requestInfoTypeStorage,
        IEnumerable<IRequestHandler> requesters)
    {
        this.options = options;
        this.requesterSelector = requesterSelector;
        this.requestInfoTypeStorage = requestInfoTypeStorage;
        this.requesters = requesters;
    }

    public List<MessageGroup> GetMessageInfos()
    {
        var groups = new Dictionary<string, List<MessageInfo>>();

        foreach (var registration in options.Value.InterfaceRegistrations)
        {
            registration.GroupName.ThrowIfNull();
            registration.MessageInterfaceType.ThrowIfNull();
            registration.DisplayNameFormatter.ThrowIfNull();

            groups.TryAdd(registration.GroupName, new List<MessageInfo>());
            var infos = groups[registration.GroupName];
            foreach (var assemblyType in registration.AssembliesToScan.SelectMany(assembly => assembly.GetTypes()))
            {
                if (registration.HasResponse)
                {
                    registration.ResponseType.ThrowIfNull();
                    registration.ResponseDisplayName.ThrowIfNull();
                }

                bool implementsInterface = assemblyType.GetInterface(registration.MessageInterfaceType.Name) is not null;
                if (implementsInterface is false)
                    continue;

                var paramInfos = TypedProviderHelper.HarvestParameterInfos(assemblyType, x => x.Name);
                string messageDisplayName = registration.DisplayNameFormatter.Invoke(assemblyType);
                var requestInfo = registration.HasResponse
                    ? new MessageInfo(registration.RequestType,
                        paramInfos,
                        registration.ResponseType.Value(),
                        messageDisplayName,
                        registration.ResponseDisplayName.Value())
                    : new MessageInfo(registration.RequestType, paramInfos, messageDisplayName);
                string requesterName;
                if (registration.RequestHandlerUniqueName == InterfaceRegistration.DefaultRequestHandlerUniqueName)
                {
                    var defaultRequester = requesters.FirstOrDefault().Value("Cant determine default requester, 0 requesters found");

                    requesterName = defaultRequester.UniqueName;
                }
                else
                {
                    if (requesters.Any(x => x.UniqueName == registration.RequestHandlerUniqueName) is false)
                        throw new InvalidOperationException($"Requester {registration.RequestHandlerUniqueName} not found");

                    registration.RequestHandlerUniqueName.ThrowIfNull();
                    requesterName = registration.RequestHandlerUniqueName;
                }

                requesterSelector.AssignRequesterForMessage(requestInfo, requesterName);
                requestInfoTypeStorage.AddRequest(requestInfo, assemblyType);
                infos.Add(requestInfo);
            }
        }

        var domainInfos = groups.Select(x => new MessageGroup(x.Key, x.Value)).ToList();
        return domainInfos;
    }
}
