using Microsoft.AspNetCore.Components;

namespace Basyc.Blazor.Controls.HtmlExtensions;

public static partial class HtmlIcons
{
    private static readonly string assemblyFolderName = typeof(IconsEnum).Assembly.GetName().Name!;

    public static MarkupString Icon(this IHtmlMethods methods, IconsEnum icon)
    {
        var svgFilePath = $"_content/{assemblyFolderName}/{icon}.svg";
        //var svgCode = File.ReadAllText(svgFilePath);
        return (MarkupString)$"<img src=\"{svgFilePath}\" alt=\"{icon}\">";
    }
}
