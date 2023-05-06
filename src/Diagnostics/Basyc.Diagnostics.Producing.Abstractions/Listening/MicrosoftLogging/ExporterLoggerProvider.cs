using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging;

public class ExporterLoggerProvider : ILoggerProvider
{
    private readonly ExporterLogger exporterLogger;

    public ExporterLoggerProvider(MicrosoftLoggingDiagnosticListener listener)
    {
        exporterLogger = new(listener);
    }

    public ILogger CreateLogger(string categoryName) => exporterLogger;

    public void Dispose()
    {
    }
}
