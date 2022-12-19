using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi
{
	public class FluentTMessageSetupMessageStage<TMessage> : BuilderStageBase
	{
		private readonly InProgressMessageRegistration inProgressMessage;
		private readonly InProgressDomainRegistration inProgressDomain;

		public FluentTMessageSetupMessageStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressDomainRegistration inProgressDomain) : base(services)
		{
			this.inProgressMessage = inProgressMessage;
			this.inProgressDomain = inProgressDomain;
		}

		public FluentSetupNoReturnStage NoReturn()
		{
			return new FluentSetupNoReturnStage(services, inProgressMessage, inProgressDomain);
		}

		public FluentTMessageSetupReturnStage<TMessage> Returns(Type messageResponseRuntimeType, string repsonseTypeDisplayName)
		{
			inProgressMessage.ResponseRunTimeType = messageResponseRuntimeType;
			inProgressMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
			return new FluentTMessageSetupReturnStage<TMessage>(services, inProgressMessage, inProgressDomain);
		}

		public FluentTMessageSetupReturnStage<TMessage> Returns(Type messageResponseRuntimeType)
		{
			return Returns(messageResponseRuntimeType, messageResponseRuntimeType.Name);
		}

		public FluentTMessageTReturnSetupReturnStage<TMessage, TResponse> Returns<TResponse>()
		{
			var responseType = typeof(TResponse);
			return Returns<TResponse>(responseType.Name);
		}

		public FluentTMessageTReturnSetupReturnStage<TMessage, TResponse> Returns<TResponse>(string repsonseTypeDisplayName)
		{
			inProgressMessage.ResponseRunTimeType = typeof(TResponse);
			inProgressMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
			return new FluentTMessageTReturnSetupReturnStage<TMessage, TResponse>(services, inProgressMessage, inProgressDomain);

		}


	}
}
