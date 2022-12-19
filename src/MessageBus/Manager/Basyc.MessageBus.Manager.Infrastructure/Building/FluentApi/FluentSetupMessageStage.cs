using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using ParameterInfo = Basyc.MessageBus.Manager.Application.Initialization.ParameterInfo;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi
{
    public class FluentSetupMessageStage : BuilderStageBase
    {
        private readonly InProgressMessageRegistration inProgressMessage;
        private readonly InProgressDomainRegistration inProgressDomain;

        public FluentSetupMessageStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressDomainRegistration inProgressDomain) : base(services)
        {
            this.inProgressMessage = inProgressMessage;
            this.inProgressDomain = inProgressDomain;
        }

        public FluentSetupMessageStage WithParameter<TParameter>(string parameterDisplayName)
        {
            inProgressMessage.Parameters.Add(new ParameterInfo(typeof(TParameter), parameterDisplayName, typeof(TParameter).Name));
            return new FluentSetupMessageStage(services, inProgressMessage, inProgressDomain);
        }

        /// <summary>
        /// Registeres <typeparamref name="TMessage"/> public properties as message parameters
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public FluentTMessageSetupMessageStage<TMessage> WithParameters<TMessage>()
        {
            foreach (var parameter in typeof(TMessage).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                inProgressMessage.Parameters.Add(new ParameterInfo(parameter.PropertyType, parameter.Name, parameter.PropertyType.Name));
            }
            return new FluentTMessageSetupMessageStage<TMessage>(services, inProgressMessage, inProgressDomain);
        }

        public FluentSetupNoReturnStage NoReturn()
        {
            return new FluentSetupNoReturnStage(services, inProgressMessage, inProgressDomain);
        }

        public FluentSetupTypeOfReturnStage Returns(Type messageResponseRuntimeType, string repsonseTypeDisplayName)
        {
            inProgressMessage.ResponseRunTimeType = messageResponseRuntimeType;
            inProgressMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
            return new FluentSetupTypeOfReturnStage(services, inProgressMessage, inProgressDomain);
        }

        public FluentSetupTypeOfReturnStage Returns(Type messageResponseRuntimeType)
        {
            return Returns(messageResponseRuntimeType, messageResponseRuntimeType.Name);
        }

        public FluentSetupTypeOfReturnStage Returns<TReponse>()
        {
            var responseType = typeof(TReponse);
            return Returns(responseType);
        }

        public FluentSetupTypeOfReturnStage Returns<TReponse>(string repsonseTypeDisplayName)
        {
            var responseType = typeof(TReponse);
            return Returns(responseType, repsonseTypeDisplayName);
        }


    }
}