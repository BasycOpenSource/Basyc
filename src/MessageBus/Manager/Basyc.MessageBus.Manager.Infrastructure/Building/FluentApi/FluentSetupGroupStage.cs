using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupGroupStage : BuilderStageBase
{
	private readonly InProgressDomainRegistration inProgressDomain;

	public FluentSetupGroupStage(IServiceCollection services, InProgressDomainRegistration inProgressDomain) : base(services)
	{
		this.inProgressDomain = inProgressDomain;
	}

	public FluentSetupMessageStage AddMessage(string messageDisplayName, RequestType messageType = RequestType.Generic)
	{
		var newMessage = new InProgressMessageRegistration();
		newMessage.MessagDisplayName = messageDisplayName;
		newMessage.MessageType = messageType;
		inProgressDomain.InProgressMessages.Add(newMessage);
		return new FluentSetupMessageStage(services, newMessage, inProgressDomain);
	}
}
