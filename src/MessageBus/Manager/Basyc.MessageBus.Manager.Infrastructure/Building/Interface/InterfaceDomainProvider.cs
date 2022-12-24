using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class InterfaceDomainProvider : IDomainInfoProvider
{
	private readonly IOptions<InterfaceDomainProviderOptions> options;
	private readonly IRequesterSelector requesterSelector;
	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;
	private readonly IEnumerable<IRequester> requesters;

	public InterfaceDomainProvider(IOptions<InterfaceDomainProviderOptions> options, IRequesterSelector requesterSelector, IRequestInfoTypeStorage requestInfoTypeStorage, IEnumerable<IRequester> requesters)
	{
		this.options = options;
		this.requesterSelector = requesterSelector;
		this.requestInfoTypeStorage = requestInfoTypeStorage;
		this.requesters = requesters;
	}

	public List<DomainInfo> GenerateDomainInfos()
	{
		var domains = new Dictionary<string, List<RequestInfo>>();

		foreach (var registration in options.Value.InterfaceRegistrations)
		{
			domains.TryAdd(registration.DomainName, new List<RequestInfo>());
			var infos = domains[registration.DomainName];
			foreach (var assembly in registration.AssembliesToScan)
			{
				foreach (var type in assembly.GetTypes())
				{
					var implementsInterface = type.GetInterface(registration.MessageInterfaceType.Name) is not null;
					if (implementsInterface is false)
						continue;

					List<ParameterInfo> paramInfos = TypedProviderHelper.HarvestParameterInfos(type, x => x.Name);
					string messageDisplayName = registration.DisplayNameFormatter.Invoke(type);
					RequestInfo requestInfo = registration.HasResponse
						? new RequestInfo(registration.RequestType, paramInfos, registration.ResponseType, messageDisplayName, registration.ResponseDisplayName)
						: new RequestInfo(registration.RequestType, paramInfos, messageDisplayName);
					string requesterName = null;
					if (registration.RequesterUniqueName == InterfaceRegistration.DefaultRequester)
					{
						var defaultRequester = requesters.FirstOrDefault();
						if (defaultRequester is null)
						{
							throw new InvalidOperationException("Cant determine default requester, 0 requesters found");
						}
						else
						{
							requesterName = defaultRequester.UniqueName;
						}
					}
					else
					{
						if (requesters.Any(x => x.UniqueName == registration.RequesterUniqueName) is false)
						{
							throw new InvalidOperationException($"Requester {registration.RequesterUniqueName} not found");
						}

						requesterName = registration.RequesterUniqueName;
					}

					requesterSelector.AssignRequester(requestInfo, requesterName);
					requestInfoTypeStorage.AddRequest(requestInfo, type);
					infos.Add(requestInfo);
				}
			}
		}

		var domainInfos = domains.Select(x => new DomainInfo(x.Key, x.Value)).ToList();
		return domainInfos;
	}
}
