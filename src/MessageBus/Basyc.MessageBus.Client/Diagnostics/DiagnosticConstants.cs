using System.Diagnostics;

namespace Basyc.MessageBus.Client.Diagnostics;

public static class DiagnosticConstants
{
	public static readonly ActivitySource HandlerStarted = new ActivitySource("Basyc.Bus.HandlerStarted", "1.0.0");
	public const string ShouldBeReceived = "IsHandlerActivityBaggage";
}
