﻿using Basyc.Blazor.Controls;
using Basyc.MessageBus.Manager.Application.Building;
using Excubo.Blazor.ScriptInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary;

public partial class BusManager
{
    private bool jsLoaded;

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

    [Inject]
    private IScriptInjectionTracker Script_injection_tracker { get; init; } = null!;

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

    //TODO: this might be enought for not using the nuget
    //protected override async Task OnInitializedAsync()
    //{
    //    await JSHost.ImportAsync("CallDotNet1",
    //        "../Pages/CallDotNet1.razor.js");
    //}

    protected override async Task OnInitializedAsync()
    {
        await Script_injection_tracker.LoadedAsync("_content/MudBlazor/MudBlazor.min.js");
        jsLoaded = true;
        await base.OnInitializedAsync();
    }
}
