using Basyc.MessageBus.Manager.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basyc.MessageBus.Manager.Infrastructure
{
    public static class TypedProviderHelper
    {
        public static List<Application.Initialization.ParameterInfo> HarvestParameterInfos(Type requestType, ITypedParameterNameFormatter parameterNameFormatter)
        {
            return HarvestParameterInfos(requestType, type => parameterNameFormatter.GetCustomTypeName(type));
        }

        public static List<Application.Initialization.ParameterInfo> HarvestParameterInfos(Type requestType, Func<Type, string> parameterDisplayNameFormattter)
        {
            return requestType.GetConstructors()
                .First()
                .GetParameters()
                .Select(paramInfo => new Application.Initialization.ParameterInfo(paramInfo.ParameterType, paramInfo.Name, parameterDisplayNameFormattter.Invoke(paramInfo.ParameterType)))
                .ToList();
        }
    }
}