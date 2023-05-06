using Basyc.Blazor.Controls.Tooltip;
using Microsoft.AspNetCore.Components;

namespace Basyc.Blazor.Controls;

public partial class BasycTooltip
{
    private readonly TooltipPartsMessenger messenger = new TooltipPartsMessenger();

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    [Parameter]
    public RenderFragment? TooltipContent { get; set; } = null;

    private string ChildContentId { get; init; } = Random.Shared.Next().ToString();

    protected override void OnInitialized() => base.OnInitialized();
}
