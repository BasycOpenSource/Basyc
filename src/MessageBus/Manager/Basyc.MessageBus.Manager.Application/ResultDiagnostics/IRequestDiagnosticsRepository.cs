using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public interface IRequestDiagnosticsRepository
{
	RequestDiagnostic CreateDiagnostics(string traceId);
	bool TryGetDiagnostics(string traceId, [NotNullWhen(true)]out RequestDiagnostic? diagnosticContext);
}
