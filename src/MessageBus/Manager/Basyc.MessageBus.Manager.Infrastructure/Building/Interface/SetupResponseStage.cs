﻿using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class SetupResponseStage : BuilderStageBase
{
    private readonly InterfaceRegistration registration;

    public SetupResponseStage(IServiceCollection services, InterfaceRegistration registration) : base(services)
    {
        this.registration = registration;
    }

    public SelectHandlerStage SetResponseDisplayName(string responseTypeDislpayName)
    {
        registration.ResponseDisplayName = responseTypeDislpayName;
        return new SelectHandlerStage(Services, registration);
    }
}
