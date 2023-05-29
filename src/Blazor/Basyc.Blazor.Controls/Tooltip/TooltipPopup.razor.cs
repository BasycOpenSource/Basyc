using Basyc.Blazor.Controls.Interops;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Basyc.Blazor.Controls.Tooltip;

public partial class TooltipPopup
{
    private readonly DotNetObjectReference<TooltipPopup> selfJsReference;
    private ElementReference tooltipPopupComponent;
    private double mousePositionX;
    private double mousePositionY;
    private bool isMouseOver;
    private bool freezeTooltip;

    private bool shouldRender;

    public TooltipPopup()
    {
        selfJsReference = DotNetObjectReference.Create(this);
    }

    [Parameter]
    public RenderFragment? ChildContent { get; set; } = null;

    [Parameter, EditorRequired]
    public TooltipPartsMessenger Messenger { get; set; } = null!;

    [Parameter, EditorRequired]
    public string OwnerId { get; set; } = null!;

    [Inject]
    private ILogger<TooltipPopup> Logger { get; init; } = null!;

    [Inject]
    private TooltipJsInterop TooltipJsInterop { get; init; } = null!;

    [Inject]
    private ElementJsInterop ElementJsInterop { get; init; } = null!;

    private string TooltipPopupComponentId { get; init; } = Random.Shared.Next().ToString();

    [JSInvokable]
    public void ChangeFreeze(bool value)
    {
        freezeTooltip = value;
        if (freezeTooltip is false)
        {
            TooltipJsInterop.HideTooltip(selfJsReference, $"{TooltipPopupComponentId}", OwnerId);
        }
    }

    public void Dispose()
    {
        selfJsReference.Dispose();
        Messenger.MouseOver -= MouseOver;
        Messenger.MouseMove -= MouseMove;
        Messenger.MouseOut -= MouseOut;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Messenger.MouseOver += MouseOver;
        Messenger.MouseMove += MouseMove;
        Messenger.MouseOut += MouseOut;
    }

    protected override bool ShouldRender() => shouldRender;

    protected override void OnAfterRender(bool firstRender)
    {
        Logger.LogInformation("TooltipPopup rendered");
        base.OnAfterRender(firstRender);
    }

    private void MouseMove(object? sender, MouseEventArgs e)
    {
        if (freezeTooltip)
            return;
        mousePositionX = e.PageX;
        mousePositionY = e.PageY;
        UpdateStyle();
    }

    private void MouseOver(object? sender, MouseEventArgs e)
    {
        if (isMouseOver)
            return;
        TooltipJsInterop.ShowTooltip(selfJsReference, $"{TooltipPopupComponentId}");
        isMouseOver = true;
        shouldRender = true;
        InvokeAsync(StateHasChanged);
        shouldRender = false;
    }

    private void MouseOut(object? sender, MouseEventArgs e)
    {
        if (isMouseOver is false)
            return;
        if (freezeTooltip is false)
        {
            TooltipJsInterop.HideTooltip(selfJsReference, $"{TooltipPopupComponentId}", OwnerId);
        }

        InvokeAsync(StateHasChanged);
        isMouseOver = false;
    }

    private void UpdateStyle()
    {
        string visibility = isMouseOver || freezeTooltip ? "visible" : "collapse";
        string cssText = $"left: {mousePositionX + 4}px; top: {mousePositionY + 4}px; visibility: {visibility}";
        ElementJsInterop.SetStyle(tooltipPopupComponent, cssText);
    }
}
