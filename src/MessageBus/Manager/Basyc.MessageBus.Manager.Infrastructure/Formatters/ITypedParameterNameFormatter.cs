using System;

namespace Basyc.MessageBus.Manager.Application;

public interface ITypedParameterNameFormatter
{
    string GetCustomTypeName(Type type);
}
