using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration
{
    public class RegisterMessagesFromAssemblyStage : BuilderStageBase
    {
        public RegisterMessagesFromAssemblyStage(IServiceCollection services, string domainName, params Assembly[] assembliesToScan) : base(services)
        {
            this.assembliesToScan = assembliesToScan;
            this.domainName = domainName;
        }

        public readonly Assembly[] assembliesToScan;
        public readonly string domainName;
    }
}