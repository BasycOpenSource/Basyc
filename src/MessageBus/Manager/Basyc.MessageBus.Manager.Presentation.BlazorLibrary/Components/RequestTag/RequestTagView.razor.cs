using Microsoft.AspNetCore.Components;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.RequestTag;

public partial class RequestTagView
{
	private RequestTagType requestType;

	public string WordContent { get; private set; }

	[Parameter]
	public RequestTagType RequestType
	{
		get => requestType;
		set
		{
			requestType = value;
			WordContent = requestType == RequestTagType.Generic ? "message" : requestType.ToString().ToLower();
		}
	}

	[Parameter]
	public TagStyle TagStyle
	{
		get;
		set;
	}
}
