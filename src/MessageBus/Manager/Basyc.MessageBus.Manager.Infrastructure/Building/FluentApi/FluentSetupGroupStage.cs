﻿using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupGroupStage : BuilderStageBase
{
	private readonly InProgressGroupRegistration inProgressGroup;

	public FluentSetupGroupStage(IServiceCollection services, InProgressGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressGroup = inProgressGroup;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName, MessageType messageType = MessageType.Generic)
	{
		var newMessage = new InProgressMessageRegistration();
		newMessage.MessageDisplayName = messageDisplayName;
		newMessage.MessageType = messageType;
		inProgressGroup.InProgressMessages.Add(newMessage);
		return new FluentSetupMessageStage(services, newMessage, inProgressGroup);
	}
}