using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Html;

public static partial class Html
{
	public static readonly string assemblyFolderName = typeof(IconsEnum).Assembly.GetName().Name!;

	public static MarkupString Icon(IconsEnum icon)
	{
		return (MarkupString)$"<img src=\"_content/{assemblyFolderName}/{icon}.svg\" alt=\"{icon}\">";
	}
}

public enum IconsEnum
{
	folder
}
