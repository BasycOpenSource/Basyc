using System.Text;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Colors;

public struct Color
{
	public Color(int red, int green, int blue, double opacity = 1)
	{
		IsValidColorValue(red);
		IsValidColorValue(green);
		IsValidColorValue(blue);
		IsValidPercentage(opacity);

		Red = red;
		Green = green;
		Blue = blue;
		Opacity = opacity;
	}

	private static void IsValidPercentage(double opacity)
	{
		if (opacity < 0 || opacity > 1)
			throw new ArgumentException("bad value", nameof(opacity));
	}

	public int Red { get; }
	public int Green { get; }
	public int Blue { get; }
	public double Opacity { get; }

	/// <summary>
	/// Create new color with edited values
	/// </summary>
	/// <param name="red"></param>
	/// <param name="green"></param>
	/// <param name="blue"></param>
	/// <param name="opacity"></param>
	/// <returns></returns>
	public Color Edit(int? red = null, int? green = null, int? blue = null, double? opacity = null)
	{
		red ??= Red;
		green ??= Green;
		blue ??= Blue;
		opacity ??= Opacity;

		return new Color(red.Value, green.Value, blue.Value, opacity.Value);
	}

	public string ToHtml()
	{
		int saturation255 = (int)Math.Round(255 * Opacity);
		StringBuilder stringBuilder = new StringBuilder(6);
		stringBuilder.Append('#');
		stringBuilder.Append(Red.ToString("X2"));
		stringBuilder.Append(Green.ToString("X2"));
		stringBuilder.Append(Blue.ToString("X2"));
		stringBuilder.Append(saturation255.ToString("X2"));
		string finalColor = stringBuilder.ToString();
		return finalColor;
	}

	public Color Darker(double percentage = 1)
	{
		IsValidPercentage(percentage);

		int red = (int)Math.Round(Red - (Red) * percentage);
		int green = (int)Math.Round(Green - (Green) * percentage);
		int blue = (int)Math.Round(Blue - (Blue) * percentage);

		return new Color(red, green, blue, Opacity);
	}

	public override string ToString()
	{
		return ToHtml();
	}

	private static void IsValidColorValue(int value)
	{
		if (value is < 0 or > 255)
			throw new ArgumentException("Invalid color value. Value must be between 0 and 255");
	}
}
