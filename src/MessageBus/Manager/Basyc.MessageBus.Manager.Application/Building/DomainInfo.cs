using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Application.Initialization;

public record DomainInfo(string DomainName, IReadOnlyCollection<RequestInfo> Requests);
