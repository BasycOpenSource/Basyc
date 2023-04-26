﻿using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure;

public static class TypedProviderHelper
{
    public static ParameterInfo[] HarvestParameterInfos(Type requestType, ITypedParameterNameFormatter parameterNameFormatter) => HarvestParameterInfos(requestType, parameterNameFormatter.GetCustomTypeName);

    public static ParameterInfo[] HarvestParameterInfos(Type requestType, Func<Type, string> parameterDisplayNameFormattter) => requestType.GetConstructors()
            .First()
            .GetParameters()
            .Select(paramInfo =>
            {
                paramInfo.Name.ThrowIfNull();
                return new ParameterInfo(paramInfo.ParameterType,
                    paramInfo.Name,
                    parameterDisplayNameFormattter.Invoke(paramInfo.ParameterType));
            })
            .ToArray();
}
