using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Icons;
public static class Icon
{
	public static MarkupString Get(IconsEnum icon)
	{
		return (MarkupString)$"<img src=\"_content/Basyc.MessageBus.Manager.Presentation.BlazorLibrary/{icon}.svg\" alt=\"{icon}\">";
	}
}
