using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary;

public partial class BusManager
{
	[Inject] private BusManagerJsInterop BusManagerJSInterop { get; set; } = null!;

	public MessageInfo? SelectedMessageInfo { get; private set; } = null;

	public MudTheme MudTheme = new()
	{
		Palette = new Palette()
		{
			Background = "#0C0B10",
			Primary = "#9E184A",
			TextPrimary = Colors.Green.Default,
		},
	};

	protected override void OnInitialized()
	{
		base.OnInitialized();
	}

	protected override async Task OnParametersSetAsync()
	{
		await BusManagerJSInterop.ApplyChangesToIndexHtml();
		await base.OnParametersSetAsync();
	}

	protected override async Task OnInitializedAsync()
	{
		await BusManagerJSInterop.ApplyChangesToIndexHtml();
		await base.OnInitializedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			await BusManagerJSInterop.ApplyChangesToIndexHtml();

		await base.OnAfterRenderAsync(firstRender);
	}

	private void OnSelectedRequestMenuItemChanged(MessageInfo messageInfo)
	{
		SelectedMessageInfo = messageInfo;
		//SelectedRequestViewModel = newSelectedRequest;
		StateHasChanged();
	}
}
