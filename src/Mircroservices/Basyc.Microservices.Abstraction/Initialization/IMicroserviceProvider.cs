using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MicroService.Abstraction.Initialization;

public interface IMicroserviceProvider
{
	void RegisterActor<TActor>();
	void RegisterActor(Type actorType);
}