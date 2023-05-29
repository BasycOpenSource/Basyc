using System.Globalization;

namespace Basyc.Blazor.Controls.HtmlExtensions;

public static partial class HtmlNumbers
{
    private static readonly NumberFormatInfo numberFormatter = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();

    static HtmlNumbers()
    {
        numberFormatter.NumberDecimalSeparator = ".";
    }

    public static string Number(this IHtmlMethods? methods, int number) => number.ToString(numberFormatter);

    public static string Number(this IHtmlMethods? methods, double number) => number.ToString(numberFormatter);

    public static string Number(this IHtmlMethods? methods, decimal number) => number.ToString(numberFormatter);

    public static int IntFromHtml(this IHtmlMethods? methods, string number) => int.Parse(number, numberFormatter);

    public static double DoubleFromHtml(this IHtmlMethods? methods, string number) => double.Parse(number, numberFormatter);
}
