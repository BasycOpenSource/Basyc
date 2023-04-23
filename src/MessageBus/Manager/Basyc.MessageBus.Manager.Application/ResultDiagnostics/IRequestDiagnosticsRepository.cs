namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public interface IRequestDiagnosticsRepository
{
    MessageDiagnostic CreateDiagnostics(string traceId);
    bool TryGetDiagnostics(string traceId, [NotNullWhen(true)] out MessageDiagnostic? diagnosticContext);
}
