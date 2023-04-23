﻿using Basyc.MessageBus.Client;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;
using Basyc.MessageBus.Manager.Infrastructure.Formatters;

namespace Basyc.MessageBus.Manager.Infrastructure.MassTransit;

public class MassTransitRequestHandler : BasycTypedMessageBusRequestHandler, IRequestHandler
{
    public MassTransitRequestHandler(ITypedMessageBusClient messageBusManager, IRequestInfoTypeStorage requestInfoTypeStorage, IResponseFormatter responseFormatter)
        : base(messageBusManager, requestInfoTypeStorage, responseFormatter, null!, null!, null!)
    {
        throw new NotImplementedException();
    }
}
