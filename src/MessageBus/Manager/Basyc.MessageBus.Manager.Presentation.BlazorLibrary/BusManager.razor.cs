using Basyc.Blazor.Controls;
using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary;

public partial class BusManager
{
    public MessageInfo? SelectedMessageInfo { get; private set; }

    public MudTheme MudTheme { get; init; } = new()
    {
        Palette = new Palette()
        {
            Background = "#0C0B10",
            Primary = "#9E184A",
            TextPrimary = new MudBlazor.Utilities.MudColor("#9F9F9F"),
        },
    };

    [Inject] private BusManagerJsInterop BusManagerJSInterop { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        BasycStyleSection.AddStyleSection(new Blazor.Controls.StyleSections.StyleDefinition(
            "1",
            string.Empty,
            "style1"));

        BasycStyleSection.AddStyleSection(new Blazor.Controls.StyleSections.StyleDefinition(
            "2",
            string.Empty,
            "style2"));

        BasycStyleSection.AddStyleSection(new Blazor.Controls.StyleSections.StyleDefinition(
            "3",
            string.Empty,
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
