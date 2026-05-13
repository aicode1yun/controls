using Microsoft.AspNetCore.Components;

namespace Shiny.Blazor.Controls;

public partial class StaggeredGrid<TItem>
{
    [Parameter] public IReadOnlyList<TItem>? Items { get; set; }
    [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
    [Parameter] public RenderFragment? EmptyTemplate { get; set; }
    [Parameter] public int ColumnCount { get; set; } = 2;
    [Parameter] public double ColumnSpacing { get; set; } = 16;
    [Parameter] public double RowSpacing { get; set; } = 16;
    [Parameter] public EventCallback<TItem> ItemSelected { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    string ContainerStyle =>
        $"column-count:{ColumnCount};column-gap:{ColumnSpacing}px;" +
        $"--staggered-row-spacing:{RowSpacing}px;";

    async Task OnItemClicked(int index)
    {
        if (Items is not null && index < Items.Count && ItemSelected.HasDelegate)
            await ItemSelected.InvokeAsync(Items[index]);
    }
}
