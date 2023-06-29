using Basyc.Blazor.Controls.Interops;
using Basyc.Blazor.Interops;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Basyc.Blazor.Controls.Tooltip;

public partial class TooltipPopup
{
    private static readonly PeriodicTimer tooltipPopupTimer = new PeriodicTimer(TimeSpan.FromSeconds(0.05));

    private readonly bool shouldRender;
    private readonly DotNetObjectReference<TooltipPopup> selfJsReference;
    private ElementReference tooltipPopupComponent;
    private double lastMousePositionX;
    private double lastMousePositionY;
    private bool isMouseOver;
    private bool isToolTipShown;
    private bool freezeTooltip;

    static TooltipPopup()
    {
        StartTimer();
    }

    public TooltipPopup()
    {
        selfJsReference = DotNetObjectReference.Create(this);
        shouldRender = true;
    }

    private static event EventHandler? TooltipPopupTimerTick;

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
            TooltipJsInterop.HideTooltip(selfJsReference, $"{TooltipPopupComponentId}", OwnerId);
    }

    public void Dispose()
    {
        selfJsReference.Dispose();
        Messenger.MouseOver -= MouseOver;
        Messenger.MouseMove -= MouseMove;
        Messenger.MouseOut -= MouseOut;
        TooltipPopupTimerTick -= OnTimerTick;
    }

    protected override void OnParametersSet()
    {
        Logger.LogDebug(nameof(OnParametersSet));

        Messenger.MouseOver += MouseOver;
        Messenger.MouseMove += MouseMove;
        Messenger.MouseOut += MouseOut;
        base.OnParametersSet();
    }

    protected override bool ShouldRender() => shouldRender;

    protected override void OnAfterRender(bool firstRender)
    {
        Logger.LogDebug(nameof(OnAfterRender));
        base.OnAfterRender(firstRender);
    }

    private static async void StartTimer()
    {
        while (await tooltipPopupTimer.WaitForNextTickAsync())
        {
            TooltipPopupTimerTick?.Invoke(null, EventArgs.Empty);
        }
    }

    private async void MouseMove(object? sender, MouseEventArgs e)
    {
        if (freezeTooltip)
            return;

        var mouseNewPositionDifference = Math.Abs(lastMousePositionX - e.PageX) + Math.Abs(lastMousePositionY - e.PageY);
        if (mouseNewPositionDifference < 5)
            return;

        lastMousePositionX = e.PageX;
        lastMousePositionY = e.PageY;
        await UpdateStyle(lastMousePositionX, lastMousePositionY);
    }

    private void MouseOver(object? sender, MouseEventArgs e)
    {
        if (isMouseOver)
            return;

        if (isToolTipShown is false)
        {
            Logger.LogDebug("First MouseOver showing tooltip");
            isToolTipShown = true;
            TooltipJsInterop.ShowTooltip(selfJsReference, $"{TooltipPopupComponentId}");
            TooltipPopupTimerTick += OnTimerTick;
        }

        isMouseOver = true;
    }

    private void MouseOut(object? sender, MouseEventArgs e)
    {
        if (isMouseOver is false)
            return;

        isMouseOver = false;
    }

    private async Task UpdateStyle(double mousePositionX, double mousePositionY)
    {
        const double mouseOffsetMs = 20;
        string visibility = isMouseOver || freezeTooltip ? "visible" : "collapse";
        string cssText = $"left: {mousePositionX + mouseOffsetMs}px; top: {mousePositionY + mouseOffsetMs}px; visibility: {visibility}";
        await ElementJsInterop.SetStyle(tooltipPopupComponent, cssText);
    }

    private void OnTimerTick(object? sender, EventArgs args)
    {
        if (isMouseOver is true && isToolTipShown is false)
        {
            Logger.LogDebug("Timer showing tooltip");
            isToolTipShown = true;
            TooltipJsInterop.ShowTooltip(selfJsReference, $"{TooltipPopupComponentId}");
            return;
        }

        if (isMouseOver is false && isToolTipShown is true)
        {
            if (freezeTooltip)
                return;
            Logger.LogDebug("Timer hiding tooltip");
            isToolTipShown = false;
            TooltipJsInterop.HideTooltip(selfJsReference, $"{TooltipPopupComponentId}", OwnerId);
            TooltipPopupTimerTick -= OnTimerTick;
            return;
        }
    }
}
