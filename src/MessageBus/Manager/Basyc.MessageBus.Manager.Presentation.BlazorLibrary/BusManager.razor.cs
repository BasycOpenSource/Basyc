using Basyc.Blazor.Controls;
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
			TextPrimary = new MudBlazor.Utilities.MudColor("#9F9F9F"),
		},
	};

	protected override void OnInitialized()
	{
		base.OnInitialized();

		BasycStyleSection.AddStyleSection(new Blazor.Controls.StyleSections.StyleDefinition(
			"1",
			"",
			"style1"));

		BasycStyleSection.AddStyleSection(new Blazor.Controls.StyleSections.StyleDefinition(
			"2",
			"",
			"style2"));

		BasycStyleSection.AddStyleSection(new Blazor.Controls.StyleSections.StyleDefinition(
			"3",
			"",
			"style3"));

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
}
