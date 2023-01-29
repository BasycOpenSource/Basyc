using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Microsoft.Extensions.Options;
using Throw;
using static Basyc.MessageBus.Manager.Infrastructure.CqrsInterfacedDomainProviderOptions;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class CqrsInterfacedDomainProvider : IDomainInfoProvider
{
	private readonly ITypedDomainNameFormatter domainNameFormatter;
	private readonly IOptions<CqrsInterfacedDomainProviderOptions> options;
	private readonly ITypedParameterNameFormatter parameterNameFormatter;
	private readonly IRequesterSelector requesterSelector;
	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;
	private readonly ITypedRequestNameFormatter requestNameFormatter;
	private readonly ITypedResponseNameFormatter responseNameFormatter;

	public CqrsInterfacedDomainProvider(
		ITypedDomainNameFormatter domainNameFormatter,
		ITypedRequestNameFormatter requestNameFormatter,
		ITypedParameterNameFormatter parameterNameFormatter,
		ITypedResponseNameFormatter responseNameFormatter,
		IRequestInfoTypeStorage requestInfoTypeStorage,
		IOptions<CqrsInterfacedDomainProviderOptions> options,
		IRequesterSelector requesterSelector
	)
	{
		this.parameterNameFormatter = parameterNameFormatter;
		this.responseNameFormatter = responseNameFormatter;
		this.requestInfoTypeStorage = requestInfoTypeStorage;
		this.domainNameFormatter = domainNameFormatter;
		this.requestNameFormatter = requestNameFormatter;
		this.options = options;
		this.requesterSelector = requesterSelector;
	}

	public List<DomainInfo> GenerateDomainInfos()
	{
		//var domains = new List<DomainInfo>();
		var domains = new Dictionary<string, List<RequestInfo>>();

		foreach (var registration in options.Value.GetcqrsRegistrations())
		{
			registration.DomainName.ThrowIfNull();
			domains.TryAdd(registration.DomainName, new List<RequestInfo>());
			var requestInfos = domains[registration.DomainName];

			foreach (var assemblyWithMessages in registration.AssembliesToScan)
			{
				foreach (var type in assemblyWithMessages.GetTypes())
				{
					if (TryParseRequestType(registration, type, out var requestType, out var hasResponse, out var responseType))
					{
						var paramInfos = TypedProviderHelper.HarvestParameterInfos(type, parameterNameFormatter);
						var requestInfo = hasResponse
							? new RequestInfo(requestType, paramInfos, responseType!, requestNameFormatter.GetFormattedName(type),
								responseNameFormatter.GetFormattedName(responseType!))
							: new RequestInfo(requestType, paramInfos, requestNameFormatter.GetFormattedName(type));
						requestInfos.Add(requestInfo);

						requesterSelector.AssignRequester(requestInfo, BasycTypedMessageBusRequester.BasycTypedMessageBusRequesterUniqueName);
						requestInfoTypeStorage.AddRequest(requestInfo, type);
					}
				}
			}
		}

		var domainInfos = domains.Select(x => new DomainInfo(x.Key, x.Value)).ToList();
		return domainInfos;
	}

	/// <summary>
	///     False if type is not a request
	/// </summary>
	/// <param name="type"></param>
	/// <param name="requestType"></param>
	/// <param name="hasResponse"></param>
	/// <param name="responseType"></param>
	/// <returns></returns>
	private bool TryParseRequestType(CqrsRegistration cqrsRegistration, Type type, out RequestType requestType, out bool hasResponse,
		out Type? responseType)
	{
		responseType = null;
		hasResponse = false;
		requestType = default;

		if (type.IsClass is false)
		{
			return false;
		}

		if (type.IsAbstract is true)
		{
			return false;
		}

		if (cqrsRegistration.QueryInterfaceType is not null && type.GetInterface(cqrsRegistration.QueryInterfaceType.Name) is not null)
		{
			requestType = RequestType.Query;
			hasResponse = true;
			responseType = type.GetTypeArgumentsFromParent(cqrsRegistration.QueryInterfaceType)[0];
			return true;
		}

		if (cqrsRegistration.CommandInterfaceType is not null && type.GetInterface(cqrsRegistration.CommandInterfaceType.Name) is not null)
		{
			requestType = RequestType.Command;
			return true;
		}

		if (cqrsRegistration.CommandWithResponseInterfaceType is not null &&
			type.GetInterface(cqrsRegistration.CommandWithResponseInterfaceType.Name) is not null)
		{
			requestType = RequestType.Command;
			hasResponse = true;
			responseType = type.GetTypeArgumentsFromParent(cqrsRegistration.CommandWithResponseInterfaceType)[0];
			return true;
		}

		if (cqrsRegistration.MessageInterfaceType is not null && type.GetInterface(cqrsRegistration.MessageInterfaceType.Name) is not null)
		{
			requestType = RequestType.Generic;
			return true;
		}

		if (cqrsRegistration.MessageWithResponseInterfaceType is not null &&
			type.GetInterface(cqrsRegistration.MessageWithResponseInterfaceType.Name) is not null)
		{
			requestType = RequestType.Generic;
			hasResponse = true;
			responseType = type.GetTypeArgumentsFromParent(cqrsRegistration.MessageWithResponseInterfaceType)[0];
			return true;
		}

		return false;
	}
}
