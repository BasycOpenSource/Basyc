using Basyc.MessageBus.Manager.Application.Building;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application.Initialization;

public interface IDomainInfoProvider
{
	List<MessageGroup> GenerateDomainInfos();
}
