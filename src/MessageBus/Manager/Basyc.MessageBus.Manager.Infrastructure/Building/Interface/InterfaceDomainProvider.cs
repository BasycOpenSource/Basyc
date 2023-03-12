using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class InterfaceDomainProvider : IDomainInfoProvider
{
	private readonly IOptions<InterfaceDomainProviderOptions> options;
	private readonly IEnumerable<IRequestHandler> requesters;
	private readonly IRequesterSelector requesterSelector;
	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;

	public InterfaceDomainProvider(IOptions<InterfaceDomainProviderOptions> options, IRequesterSelector requesterSelector,
		IRequestInfoTypeStorage requestInfoTypeStorage, IEnumerable<IRequestHandler> requesters)
	{
		this.options = options;
		this.requesterSelector = requesterSelector;
		this.requestInfoTypeStorage = requestInfoTypeStorage;
		this.requesters = requesters;
	}

	public List<DomainInfo> GenerateDomainInfos()
	{
		var domains = new Dictionary<string, List<MessageInfo>>();

		foreach (var registration in options.Value.InterfaceRegistrations)
		{
			registration.GroupName.ThrowIfNull();
			domains.TryAdd(registration.GroupName, new List<MessageInfo>());
			var infos = domains[registration.GroupName];
			foreach (var assemblyType in registration.AssembliesToScan.SelectMany(assembly => assembly.GetTypes()))
			{
				registration.MessageInterfaceType.ThrowIfNull();
				registration.DisplayNameFormatter.ThrowIfNull();
				if (registration.HasResponse)
				{
					registration.ResponseType.ThrowIfNull();
					registration.ResponseDisplayName.ThrowIfNull();
				}

				var implementsInterface = assemblyType.GetInterface(registration.MessageInterfaceType.Name) is not null;
				if (implementsInterface is false)
					continue;

				var paramInfos = TypedProviderHelper.HarvestParameterInfos(assemblyType, x => x.Name);
				var messageDisplayName = registration.DisplayNameFormatter.Invoke(assemblyType);
				var requestInfo = registration.HasResponse
					? new MessageInfo(registration.RequestType, paramInfos, registration.ResponseType.Value(), messageDisplayName,
						registration.ResponseDisplayName.Value())
					: new MessageInfo(registration.RequestType, paramInfos, messageDisplayName);
				string requesterName;
				if (registration.RequestHandlerUniqueName == InterfaceRegistration.DefaultRequestHandlerUniqueName)
				{
					var defaultRequester = requesters.FirstOrDefault();
					if (defaultRequester is null)
						throw new InvalidOperationException("Cant determine default requester, 0 requesters found");

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

		var domainInfos = domains.Select(x => new DomainInfo(x.Key, x.Value)).ToList();
		return domainInfos;
	}
}
