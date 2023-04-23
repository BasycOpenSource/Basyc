namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;
public static class DateTimeOffsetDiagnosticTimeExntesions
{
	public static DiagnosticTime GetDiagnosticTime(this DateTimeOffset toConvertTime, DateTimeOffset baseTime)
	{
		return new DiagnosticTime(toConvertTime, baseTime);
	}
}
