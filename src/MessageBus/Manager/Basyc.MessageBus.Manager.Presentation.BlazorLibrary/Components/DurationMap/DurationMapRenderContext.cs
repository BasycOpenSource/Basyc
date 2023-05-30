namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap;
public class DurationMapRenderContext
{
    public event EventHandler? FastTimer;

    public event EventHandler? MediumTimer;

    public double PixelsPerMs { get; set; }

    public void OnFastTimer() => FastTimer?.Invoke(this, EventArgs.Empty);

    public void OnMediumTimer() => MediumTimer?.Invoke(this, EventArgs.Empty);
}
