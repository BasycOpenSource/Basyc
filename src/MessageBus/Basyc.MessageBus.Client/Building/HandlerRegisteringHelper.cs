using Basyc.MessageBus.Client.Diagnostics;
using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Basyc.MessageBus.Client.Building;
public static class HandlerRegisteringHelper
{
    public static IServiceCollection RegisterHandlerWithDecoratedLogger(IServiceCollection services, Type handlerType)
    {
        var serviceType = handlerType.IsAssignableToGenericType(typeof(IMessageHandler<>))
            ? typeof(IMessageHandler<>).MakeGenericType(handlerType.GetTypeArgumentsFromParent(typeof(IMessageHandler<>)))
            : handlerType.IsAssignableToGenericType(typeof(IMessageHandler<,>))
                ? typeof(IMessageHandler<,>).MakeGenericType(handlerType.GetTypeArgumentsFromParent(typeof(IMessageHandler<,>)))
                : throw new ArgumentException();
        services.AddScoped(serviceType, serviceProvider => CreateHandlerWithDecoratedLogger(handlerType, serviceProvider));
        EnsureDecoratedLoggerRegistered(services, handlerType);
        return services;

    }

    public static IServiceCollection RegisterHandlerWithDecoratedLogger<TMessage>(IServiceCollection services, Action<TMessage, ILogger> handlerDelegate)
        where TMessage : IMessage
    {
        var serviceType = typeof(IMessageHandler<>).MakeGenericType(typeof(TMessage));
        var handlerType = typeof(MessageDelegateHandler<TMessage>);
        services.AddScoped(serviceType, serviceProvider => new MessageDelegateHandler<TMessage>(GetDecoratedLogger(serviceProvider, typeof(MessageDelegateHandler<TMessage>)), handlerDelegate));
        EnsureDecoratedLoggerRegistered(services, handlerType);
        return services;
    }

    private static ILogger GetDecoratedLogger(IServiceProvider services, Type? loggerTParam)
    {
        if (loggerTParam is null)
        {
            var decoratedLogger = services.GetRequiredService<BusHandlerLogger>();
            return decoratedLogger;
        }
        else
        {
            var decoLoggerType = typeof(BusHandlerLogger<>).MakeGenericType(loggerTParam);
            var decoLogger = (ILogger)services.GetRequiredService(decoLoggerType);
            return decoLogger;
        }
    }

    private static object CreateHandlerWithDecoratedLogger(Type handlerType, IServiceProvider services)
    {
        var handlerConstrucor = GetHandlerConstructor(handlerType);

        var ctorParams = handlerConstrucor.GetParameters();
        var ctorArguments = new object[ctorParams.Length];
        for (var paramIndex = 0; paramIndex < ctorParams.Length; paramIndex++)
        {
            var ctorParam = ctorParams[paramIndex];
            //if (ctorParam.ParameterType == typeof(ILogger))
            //{
            //	var handlerLogger = services.GetRequiredService(typeof(BusHandlerLogger));
            //	ctorArguments[paramIndex] = handlerLogger;
            //	continue;
            //}

            //if (ctorParam.ParameterType.IsAssignableToGenericType(typeof(ILogger<>)))
            //{
            //	var originalLoggerGenericArgument = ctorParam.ParameterType.GetTypeArgumentsFromParent(typeof(ILogger<>))[0];
            //	var decoLoggerType = typeof(BusHandlerLogger<>).MakeGenericType(originalLoggerGenericArgument);
            //	var decoLogger = services.GetRequiredService(decoLoggerType);
            //	ctorArguments[paramIndex] = decoLogger;
            //	continue;
            //}
            if (ctorParam.ParameterType.IsAssignableTo(typeof(ILogger)))
            {
                Type? loggerTParam = null;
                if (ctorParam.ParameterType.IsAssignableToGenericType(typeof(ILogger<>)))
                {
                    loggerTParam = ctorParam.ParameterType.GetTypeArgumentsFromParent(typeof(ILogger<>))[0];
                }
                GetDecoratedLogger(services, loggerTParam);
                continue;
            }

            ctorArguments[paramIndex] = services.GetRequiredService(ctorParam.ParameterType);
        }

        var handlerInstance = handlerConstrucor.Invoke(ctorArguments);
        return handlerInstance;
    }

    private static void EnsureDecoratedLoggerRegistered(IServiceCollection services, Type originalHandlerType)
    {
        var ctor = GetHandlerConstructor(originalHandlerType);
        var ctorParams = ctor.GetParameters();
        var loggerParam = ctorParams.FirstOrDefault(x => x.ParameterType == typeof(ILogger));
        if (loggerParam is null)
        {
            loggerParam = ctorParams.FirstOrDefault(x => x.ParameterType.IsAssignableToGenericType(typeof(ILogger<>)));
            if (loggerParam is null)
                return; //Handler does not have logger, we can skip

            var loggerCategory = loggerParam.ParameterType.GetTypeArgumentsFromParent(typeof(ILogger<>))[0];

            var busLoggerType = typeof(BusHandlerLogger<>).MakeGenericType(loggerCategory);
            services.TryAddSingleton(busLoggerType);
        }
        else
            services.TryAddSingleton(typeof(BusHandlerLogger));
    }

    private static ConstructorInfo GetHandlerConstructor(Type handlerType)
    {
        var handlerConstructors = handlerType.GetConstructors();
        if (handlerConstructors.Length > 1)
            throw new Exception("Multiple contructors not supported");

        var handlerConstrucor = handlerConstructors[0];
        return handlerConstrucor;
    }
}
