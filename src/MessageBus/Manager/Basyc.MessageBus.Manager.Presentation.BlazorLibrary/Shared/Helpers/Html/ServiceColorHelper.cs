namespace Basyc.Blazor.Controls.Colors;

public static class ServiceColorHelper
{
    public const double BackgroundSaturation = 0.4;
    public const double BackgroundSaturationRandomness = 0;
    public const double BackgroundOpacity = 0.15;

    public static Color GetBackground(string serviceName) => ColorHelper.GetColorFromText(serviceName, BackgroundSaturation, BackgroundSaturationRandomness, BackgroundOpacity);
}
