using System.Globalization;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Html;

public static partial class Html
{
    private static readonly NumberFormatInfo numberFormatter;

    static Html()
    {
        numberFormatter = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        numberFormatter.NumberDecimalSeparator = ".";
    }

    public static string Number(int number) => number.ToString(numberFormatter);

    public static string Number(double number) => number.ToString(numberFormatter);

    public static string Number(decimal number) => number.ToString(numberFormatter);
}
