using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Colors;
using System.Text;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Html;

public static partial class Html
{
    public static string Color(Color color)
    {
        int saturation255 = (int)Math.Round(255 * color.Opacity);
        StringBuilder stringBuilder = new StringBuilder(6);
        stringBuilder.Append('#');
        stringBuilder.Append(color.Red.ToString("X2"));
        stringBuilder.Append(color.Green.ToString("X2"));
        stringBuilder.Append(color.Blue.ToString("X2"));
        stringBuilder.Append(saturation255.ToString("X2"));
        string finalColor = stringBuilder.ToString();
        return finalColor;
    }
}
