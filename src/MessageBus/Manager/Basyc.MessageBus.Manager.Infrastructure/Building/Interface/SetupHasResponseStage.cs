using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SetupHasResponseStage : BuilderStageBase
{
    private readonly InterfaceRegistration registration;

    public SetupHasResponseStage(IServiceCollection services, InterfaceRegistration registration) : base(services)
    {
        this.registration = registration;
    }

    public SelectHandlerStage NoResponse()
    {
        registration.HasResponse = false;
        return new SelectHandlerStage(services, registration);
    }

    public SetupResponseStage HasResponse(Type responseType)
    {
        registration.HasResponse = true;
        registration.ResponseType = responseType;
        return new SetupResponseStage(services, registration);
    }

    public SetupResponseStage HasResponse<TResponse>() => HasResponse(typeof(TResponse));
}
