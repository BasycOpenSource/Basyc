using Basyc.MessageBus.Manager.Application.Initialization;

namespace Basyc.MessageBus.Manager.Application;

public class Parameter
{
    public Parameter(ParameterInfo parameterInfo, object? value)
    {
        ParameterInfo = parameterInfo;
        Value = value;
    }

    public ParameterInfo ParameterInfo { get; init; }

    public object? Value { get; init; }
}
