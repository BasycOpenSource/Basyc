using System.Globalization;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Htmls;

public static partial class Html
{
    private static readonly NumberFormatInfo numberFormatter = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();

    static Html()
    {
        numberFormatter.NumberDecimalSeparator = ".";
    }

    public static string Number(int number) => number.ToString(numberFormatter);

    public static string Number(double number) => number.ToString(numberFormatter);

    public static string Number(decimal number) => number.ToString(numberFormatter);
}
