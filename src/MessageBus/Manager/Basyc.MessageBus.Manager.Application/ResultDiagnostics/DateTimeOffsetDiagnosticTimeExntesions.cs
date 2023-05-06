namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;
public static class DateTimeOffsetDiagnosticTimeExntesions
{
    public static DiagnosticTime GetDiagnosticTime(this DateTimeOffset toConvertTime, DateTimeOffset baseTime) => new DiagnosticTime(toConvertTime, baseTime);
}
