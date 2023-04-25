using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Html;

public static partial class Html
{
    private static readonly string assemblyFolderName = typeof(IconsEnum).Assembly.GetName().Name!;

    public static MarkupString Icon(IconsEnum icon)
    {
        var svgFilePath = $"_content/{assemblyFolderName}/{icon}.svg";
        //var svgCode = File.ReadAllText(svgFilePath);
        return (MarkupString)$"<img src=\"{svgFilePath}\" alt=\"{icon}\">";
    }
}
