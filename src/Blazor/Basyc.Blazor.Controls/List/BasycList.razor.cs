using Basyc.Blazor.Controls.Interops;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace Basyc.Blazor.Controls;

public partial class BasycList<TItem>
{
    private readonly string scrollId = Random.Shared.Next().ToString();
    private Action? unsubcribeAction;
    private string styleToRender = string.Empty;

    [Inject]
    public ScrollJsInterop ScrollJsInterop { get; init; } = null!;

    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; } = null!;

    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    [Parameter]
    public bool Virtualize { get; set; } = true;

    [Parameter] public SpaceSize RowGap { get; set; }

    private ICollection<TItem> ItemsCasted { get; set; } = null!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        unsubcribeAction?.Invoke();
        if (Items is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += ItemAdded;
            unsubcribeAction = () =>
            {
                collection.CollectionChanged -= ItemAdded;
            };
        }

        ItemsCasted = (ICollection<TItem>)Items;
        styleToRender = $"row-gap: {RowGap.GetSizeCssVariable()};";
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        ScrollJsInterop.AddDragToScroll(scrollId);
    }

    private void ItemAdded(object? sender, NotifyCollectionChangedEventArgs e) => InvokeAsync(StateHasChanged);
}
