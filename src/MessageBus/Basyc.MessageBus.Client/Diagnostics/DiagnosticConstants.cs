using System.Diagnostics;

namespace Basyc.MessageBus.Client.Diagnostics;

public static class DiagnosticConstants
{
    public const string ShouldBeReceived = "IsHandlerActivityBaggage";

    public static readonly ActivitySource HandlerStarted = new("Basyc.Bus.HandlerStarted", "1.0.0");
}
