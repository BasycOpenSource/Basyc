using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building.Common;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public static class ReturnStageHelper
{
    public static void RegisterMessageRegistration(
        IServiceCollection services,
        FluentApiGroupRegistration fluentApiGroup,
        FluentApiMessageRegistration fluentApiMessage,
        RequestHandlerDelegate handler) => services.Configure<CommonMessageInfoProviderOptions>(x =>
                                                {
                                                    var messageRegistration = new MessageRegistration();
                                                    messageRegistration.MessageDisplayName = fluentApiMessage.MessageDisplayName;
                                                    messageRegistration.Parameters.AddRange(fluentApiMessage.Parameters);
                                                    messageRegistration.ResponseRunTimeType = fluentApiMessage.ResponseRunTimeType;
                                                    messageRegistration.ResponseRunTimeTypeDisplayName = fluentApiMessage.ResponseRunTimeTypeDisplayName;
                                                    messageRegistration.HandlerDelegate = handler;
                                                    var group = x.MessageGroupRegistration.FirstOrDefault(x => x.Name == fluentApiGroup.Name);
                                                    if (group == default)
                                                    {
                                                        group = new MessageGroupRegistration(fluentApiGroup.Name.Value());
                                                        x.MessageGroupRegistration.Add(group);
                                                    }

                                                    group.MessageRegistrations.Add(messageRegistration);
                                                });
}
