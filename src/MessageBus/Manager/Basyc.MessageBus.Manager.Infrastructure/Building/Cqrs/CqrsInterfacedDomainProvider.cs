using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using static Basyc.MessageBus.Manager.Infrastructure.CqrsInterfacedDomainProviderOptions;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class CqrsInterfacedDomainProvider : IDomainInfoProvider
{
	private readonly ITypedDomainNameFormatter domainNameFormatter;
	private readonly ITypedRequestNameFormatter requestNameFormatter;
	private readonly ITypedParameterNameFormatter parameterNameFormatter;
	private readonly ITypedResponseNameFormatter responseNameFormatter;
	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;
	private readonly IOptions<CqrsInterfacedDomainProviderOptions> options;
	private readonly IRequesterSelector requesterSelector;

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

		foreach (var registration in options.Value.GetCQRSRegistrations())
		{
			domains.TryAdd(registration.DomainName, new List<RequestInfo>());
			var requestInfos = domains[registration.DomainName];

			foreach (var assemblyWithMessages in registration.AssembliesToScan)
			{
				foreach (var type in assemblyWithMessages.GetTypes())
				{
					if (TryParseRequestType(registration, type, out RequestType requestType, out bool hasResponse, out Type responseType))
					{
						List<ParameterInfo> paramInfos = TypedProviderHelper.HarvestParameterInfos(type, parameterNameFormatter);

						RequestInfo requestInfo = hasResponse
							? new RequestInfo(requestType, paramInfos, responseType, requestNameFormatter.GetFormattedName(type), responseNameFormatter.GetFormattedName(responseType))
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
	/// False if type is not a request
	/// </summary>
	/// <param name="type"></param>
	/// <param name="requesType"></param>
	/// <param name="hasResponse"></param>
	/// <param name="responseType"></param>
	/// <returns></returns>
	private bool TryParseRequestType(CQRSRegistration cQRSRegistration, Type type, out RequestType requesType, out bool hasResponse, out Type responseType)
	{
		responseType = null;
		hasResponse = false;
		requesType = default;

		if (type.IsClass is false)
			return false;
		if (type.IsAbstract is true)
			return false;

		if (cQRSRegistration.IQueryType is not null && type.GetInterface(cQRSRegistration.IQueryType.Name) is not null)
		{
			requesType = RequestType.Query;
			hasResponse = true;
			responseType = GenericsHelper.GetTypeArgumentsFromParent(type, cQRSRegistration.IQueryType)[0];
			return true;
		}

		if (cQRSRegistration.ICommandType is not null && type.GetInterface(cQRSRegistration.ICommandType.Name) is not null)
		{
			requesType = RequestType.Command;
			return true;
		}

		if (cQRSRegistration.ICommandWithResponseType is not null && type.GetInterface(cQRSRegistration.ICommandWithResponseType.Name) is not null)
		{
			requesType = RequestType.Command;
			hasResponse = true;
			responseType = GenericsHelper.GetTypeArgumentsFromParent(type, cQRSRegistration.ICommandWithResponseType)[0];
			return true;
		}

		if (cQRSRegistration.IMessageType is not null && type.GetInterface(cQRSRegistration.IMessageType.Name) is not null)
		{
			requesType = RequestType.Generic;
			return true;
		}

		if (cQRSRegistration.IMessageWithResponseType is not null && type.GetInterface(cQRSRegistration.IMessageWithResponseType.Name) is not null)
		{
			requesType = RequestType.Generic;
			hasResponse = true;
			responseType = GenericsHelper.GetTypeArgumentsFromParent(type, cQRSRegistration.IMessageWithResponseType)[0];
			return true;
		}

		return false;
	}
}