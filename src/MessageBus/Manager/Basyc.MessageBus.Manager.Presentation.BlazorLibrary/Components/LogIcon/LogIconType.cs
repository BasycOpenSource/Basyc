using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.LogIcon;
public enum LogIconType
{
	Info,
	Warning,
	Error,
}

public static class LogIconTypeHelper
{
	public static LogIconType LogLevelToLogIconType(LogLevel logLevel)
	{
		return logLevel switch
		{
			LogLevel.Trace => LogIconType.Info,
			LogLevel.Debug => LogIconType.Info,
			LogLevel.Information => LogIconType.Info,
			LogLevel.Warning => LogIconType.Warning,
			LogLevel.Error => LogIconType.Error,
			LogLevel.Critical => LogIconType.Error,
			_ => throw new ArgumentException()
		};
	}
}

