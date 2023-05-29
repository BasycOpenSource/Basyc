namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap;
public class DurationMapRenderContext
{
    public event EventHandler? FastTimer;

    public double PixelsPerMs { get; set; }

    public void OnFastTimer() => FastTimer?.Invoke(this, EventArgs.Empty);
}
