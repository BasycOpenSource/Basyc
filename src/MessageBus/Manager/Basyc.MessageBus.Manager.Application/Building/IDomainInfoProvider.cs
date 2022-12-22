using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application.Initialization;

public interface IDomainInfoProvider
{
	List<DomainInfo> GenerateDomainInfos();
}