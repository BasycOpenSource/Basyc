using Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class LoggingBuilderProducingExtensions
{
    public static void AddBasycExporterLog(this ILoggingBuilder loggingBuilder) => loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ExporterLoggerProvider>());
}
