using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging
{
	public class ExporterLoggerProvider : ILoggerProvider

	{
		private readonly ExporterLogger exporterLogger;

		public ExporterLoggerProvider(MicrosoftLoggingDiagnosticListener listener)
		{
			exporterLogger = new ExporterLogger(listener);

		}
		public ILogger CreateLogger(string categoryName)
		{
			return exporterLogger;
		}

		public void Dispose()
		{
		}
	}
}
