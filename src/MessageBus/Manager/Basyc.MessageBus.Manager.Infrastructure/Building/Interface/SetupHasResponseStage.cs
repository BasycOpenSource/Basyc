using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SetupHasResponseStage : BuilderStageBase
{
    private readonly InterfaceRegistration registration;

    public SetupHasResponseStage(IServiceCollection services, InterfaceRegistration registration) : base(services)
    {
        this.registration = registration;
    }

    public SelectRequesterStage NoResponse()
    {
        registration.HasResponse = false;
        return new SelectRequesterStage(services, registration);
    }

    public SetupResponseStage HasResponse(Type responseType)
    {
        registration.HasResponse = true;
        registration.ResponseType = responseType;
        return new(services, registration);
    }

    public SetupResponseStage HasResponse<TResponse>()
    {
        return HasResponse(typeof(TResponse));
    }
}
