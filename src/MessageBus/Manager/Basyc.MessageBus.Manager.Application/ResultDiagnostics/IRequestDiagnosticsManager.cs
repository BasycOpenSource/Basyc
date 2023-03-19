namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public interface IRequestDiagnosticsManager
{
	RequestDiagnostic CreateDiagnostics(string traceId);
	bool TryGetDiagnostics(string traceId, out RequestDiagnostic? diagnosticContext);
}
