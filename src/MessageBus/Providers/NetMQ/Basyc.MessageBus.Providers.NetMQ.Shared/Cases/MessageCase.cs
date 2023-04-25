using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.NetMQ.Shared.Cases;

public enum MessageCase
{
    CheckIn,
    Request,
    Response,
    Event,
}
