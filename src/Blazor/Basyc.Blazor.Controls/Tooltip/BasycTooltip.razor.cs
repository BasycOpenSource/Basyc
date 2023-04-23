using Basyc.Blazor.Controls.Interops;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Basyc.Blazor.Controls;

public partial class BasycTooltip
{
    private readonly string toolTipId = $"{Random.Shared.Next()}";

    private readonly string childContentId = $"{Random.Shared.Next()}";

    private double x;

    private double y;

    private bool isMouseOver;

    private bool keepVisible;

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    [Parameter]
    public RenderFragment? TooltipContent { get; set; } = null;

    [Inject]
    private TooltipJsInterop TooltipJsInterop { get; init; } = null!;

    public void KeyDown(KeyboardEventArgs e)
    {
        if (e.Code == "LeftControl")
        {
            keepVisible = true;
        }
    }

    public void KeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "LeftControl")
        {
            keepVisible = false;
        }
    }

    public void MouseOver(MouseEventArgs e)
    {
        TooltipJsInterop.ShowTooltip($"{toolTipId}");
        isMouseOver = true;
    }

    public void MouseOut(MouseEventArgs e)
    {
        TooltipJsInterop.HideTooltip($"{toolTipId}", childContentId);
        isMouseOver = false;
    }
}
