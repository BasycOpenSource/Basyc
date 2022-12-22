namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public interface IRequestDiagnosticsManager
{
	RequestDiagnosticContext CreateDiagnostics(string traceId);
	bool TryGetDiagnostics(string traceId, out RequestDiagnosticContext? diagnosticContext);
}