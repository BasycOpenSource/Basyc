namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Colors
{
	public static class ServiceColorHelper
	{
		public const double BackgroundSaturation = 0.4;
		public const double BackgroundSaturationRandomness = 0;
		public const double BackgroundOpacity = 0.15;

		public static Color GetBackground(string serviceName)
		{
			return ColorHelper.GetColorFromText(serviceName, BackgroundSaturation, BackgroundSaturationRandomness, BackgroundOpacity);
		}

	}
}
