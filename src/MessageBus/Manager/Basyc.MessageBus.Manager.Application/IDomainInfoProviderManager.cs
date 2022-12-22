using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application;

public interface IDomainInfoProviderManager
{
	IReadOnlyList<DomainInfo> GetDomainInfos();
}