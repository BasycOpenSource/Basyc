using Microsoft.AspNetCore.Components.Web;

namespace Basyc.Blazor.Controls.Tooltip;
public class TooltipPartsMessenger
{
    public event EventHandler<MouseEventArgs>? MouseOver;

    public event EventHandler<MouseEventArgs>? MouseMove;

    public event EventHandler<MouseEventArgs>? MouseOut;

    public void OnMouseOver(MouseEventArgs args) => MouseOver?.Invoke(this, args);

    public void OnMouseMove(MouseEventArgs args) => MouseMove?.Invoke(this, args);

    public void OnMouseOut(MouseEventArgs args) => MouseOut?.Invoke(this, args);
}
