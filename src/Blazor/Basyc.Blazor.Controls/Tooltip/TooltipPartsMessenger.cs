using Microsoft.AspNetCore.Components.Web;

namespace Basyc.Blazor.Controls.Tooltip;
public class TooltipPartsMessenger
{
	public void OnMouseOver(MouseEventArgs args)
	{
		MouseOver?.Invoke(this, args);
	}
	public event EventHandler<MouseEventArgs>? MouseOver;

	public void OnMouseMove(MouseEventArgs args)
	{
		MouseMove?.Invoke(this, args);
	}
	public event EventHandler<MouseEventArgs>? MouseMove;

	public void OnMouseOut(MouseEventArgs args)
	{
		MouseOut?.Invoke(this, args);
	}
	public event EventHandler<MouseEventArgs>? MouseOut;
}
