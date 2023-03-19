using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Microsoft.Extensions.Options;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedDomainProvider : IDomainInfoProvider
{
	private readonly ITypedDomainNameFormatter domainNameFormatter;
	private readonly IOptions<TypedDomainProviderOptions> options;
	private readonly ITypedParameterNameFormatter parameterNameFormatter;
	private readonly IRequesterSelector requesterSelector;
	private readonly IRequestInfoTypeStorage requestInfoTypeStorage;
	private readonly ITypedRequestNameFormatter requestNameFormatter;
	private readonly ITypedResponseNameFormatter responseNameFormatter;

	public TypedDomainProvider(
		IOptions<TypedDomainProviderOptions> options,
		ITypedDomainNameFormatter domainNameFormatter,
		ITypedRequestNameFormatter requestNameFormatter,
		ITypedParameterNameFormatter parameterNameFormatter,
		ITypedResponseNameFormatter responseNameFormatter,
		IRequestInfoTypeStorage requestInfoTypeStorage,
		IRequesterSelector requesterSelector
	)
	{
		this.options = options;
		this.domainNameFormatter = domainNameFormatter;
		this.requestNameFormatter = requestNameFormatter;
		this.parameterNameFormatter = parameterNameFormatter;
		this.responseNameFormatter = responseNameFormatter;
		this.requestInfoTypeStorage = requestInfoTypeStorage;
		this.requesterSelector = requesterSelector;
	}

	public List<MessageGroup> GenerateDomainInfos()
	{
		var domains = new List<MessageGroup>();
		foreach (var domainOption in options.Value.TypedDomainOptions)
		{
			var requestInfos = new List<MessageInfo>();
			foreach (var genericRequestType in domainOption.GenericRequestTypes)
			{
				var paramInfos = TypedProviderHelper.HarvestParameterInfos(genericRequestType, parameterNameFormatter);
				var requestInfo = new MessageInfo(MessageType.Command, paramInfos, requestNameFormatter.GetFormattedName(genericRequestType));
				requestInfos.Add(requestInfo);
				requestInfoTypeStorage.AddRequest(requestInfo, genericRequestType);
			}

			foreach (var typePair in domainOption.GenericRequestWithResponseTypes)
			{
				var paramInfos = TypedProviderHelper.HarvestParameterInfos(typePair.RequestType, parameterNameFormatter);
				var requestInfo = new MessageInfo(MessageType.Generic,
					paramInfos,
					typePair.ResponseType,
					requestNameFormatter.GetFormattedName(typePair.RequestType),
					responseNameFormatter.GetFormattedName(typePair.ResponseType)
				);
				requestInfos.Add(requestInfo);
				requestInfoTypeStorage.AddRequest(requestInfo, typePair.RequestType);
			}

			foreach (var commandType in domainOption.CommandTypes)
			{
				var paramInfos = TypedProviderHelper.HarvestParameterInfos(commandType, parameterNameFormatter);
				var requestInfo = new MessageInfo(MessageType.Command, paramInfos, requestNameFormatter.GetFormattedName(commandType));
				requestInfos.Add(requestInfo);
				requestInfoTypeStorage.AddRequest(requestInfo, commandType);
			}

			foreach (var typePair in domainOption.CommandWithResponseTypes)
			{
				var paramInfos = TypedProviderHelper.HarvestParameterInfos(typePair.RequestType, parameterNameFormatter);
				var requestInfo = new MessageInfo(MessageType.Command,
					paramInfos,
					typePair.ResponseType,
					requestNameFormatter.GetFormattedName(typePair.RequestType),
					responseNameFormatter.GetFormattedName(typePair.ResponseType)
				);
				requestInfos.Add(requestInfo);
				requestInfoTypeStorage.AddRequest(requestInfo, typePair.RequestType);
			}

			foreach (var typePair in domainOption.QueryTypes)
			{
				var paramInfos = TypedProviderHelper.HarvestParameterInfos(typePair.RequestType, parameterNameFormatter);
				var requestInfo = new MessageInfo(MessageType.Query,
					paramInfos,
					typePair.ResponseType,
					requestNameFormatter.GetFormattedName(typePair.RequestType),
					responseNameFormatter.GetFormattedName(typePair.ResponseType)
				);
				requestInfos.Add(requestInfo);
				requestInfoTypeStorage.AddRequest(requestInfo, typePair.RequestType);
			}

			foreach (var requestInfo in requestInfos)
				requesterSelector.AssignRequesterForMessage(requestInfo, BasycTypedMessageBusRequestHandler.BasycTypedMessageBusRequesterUniqueName);

			domainOption.DomainName.ThrowIfNull();
			domains.Add(new MessageGroup(domainOption.DomainName, requestInfos));
		}

		return domains;
	}
}
