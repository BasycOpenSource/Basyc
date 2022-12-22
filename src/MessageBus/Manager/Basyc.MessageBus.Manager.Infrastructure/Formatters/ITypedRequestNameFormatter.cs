using System;

namespace Basyc.MessageBus.Manager.Application;

public interface ITypedRequestNameFormatter
{
	string GetFormattedName(Type requestType);
}