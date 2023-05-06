using Basyc.Blazor.Controls.StyleSections;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Basyc.Blazor.Controls;

public partial class BasycStyleSection
{
    private static readonly ObservableCollection<StyleDefinition> styleSections = new();

    private StyleDefinition styleSection = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter, EditorRequired]
    public string SectionName { get; set; } = null!;

    [Parameter]
    public bool ThrowWhenStyleSectioNotfound { get; set; }

    public static void AddStyleSection(StyleDefinition styleSection) => styleSections.Add(styleSection);

    protected override void OnInitialized()
    {
        styleSections.CollectionChanged += (s, a) =>
        {
            StateHasChanged();
        };
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        var styleSectionPeak = styleSections.FirstOrDefault(x => x.Name == SectionName);
        if (styleSectionPeak is null)
        {
            if (ThrowWhenStyleSectioNotfound)
                throw new InvalidOperationException($"Style section '{SectionName}' not found");
            return;
        }

        styleSection = styleSectionPeak;
    }
}
