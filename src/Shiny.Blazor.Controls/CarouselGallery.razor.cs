using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Shiny.Blazor.Controls;

public partial class CarouselGallery<TItem>
{
    ElementReference trackRef;

    [Inject] IJSRuntime JS { get; set; } = default!;

    [Parameter] public IReadOnlyList<TItem>? Items { get; set; }
    [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }
    [Parameter] public double FocusedItemScale { get; set; } = 1.0;
    [Parameter] public double UnfocusedItemScale { get; set; } = 0.85;
    [Parameter] public double ItemWidth { get; set; } = 280;
    [Parameter] public double ItemHeight { get; set; } = 400;
    [Parameter] public double ItemSpacing { get; set; } = 16;
    [Parameter] public double PeekAmount { get; set; } = 40;
    [Parameter] public int CurrentPosition { get; set; }
    [Parameter] public EventCallback<int> CurrentPositionChanged { get; set; }
    [Parameter] public EventCallback<TItem> ItemSelected { get; set; }
    [Parameter] public bool ShowIndicators { get; set; } = true;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    string ContainerStyle => $"--carousel-item-width:{ItemWidth}px;--carousel-item-height:{ItemHeight}px;" +
                             $"--carousel-spacing:{ItemSpacing}px;--carousel-peek:{PeekAmount}px;";

    string GetItemStyle(double scale) =>
        $"width:{ItemWidth}px;height:{ItemHeight}px;transform:scale({scale});transition:transform 0.3s ease;";

    async Task OnItemClicked(int index)
    {
        if (Items is not null && index < Items.Count)
        {
            CurrentPosition = index;
            if (CurrentPositionChanged.HasDelegate)
                await CurrentPositionChanged.InvokeAsync(index);
            if (ItemSelected.HasDelegate)
                await ItemSelected.InvokeAsync(Items[index]);
        }
    }

    async Task ScrollToPosition(int index)
    {
        CurrentPosition = index;
        if (CurrentPositionChanged.HasDelegate)
            await CurrentPositionChanged.InvokeAsync(index);

        await JS.InvokeVoidAsync("eval",
            $"document.querySelector('[_bl_{trackRef.Id}]')?.children[{index}]?.scrollIntoView({{behavior:'smooth',inline:'center',block:'nearest'}})");
    }

    void OnScroll()
    {
        // Scroll position tracking is handled by CSS scroll-snap
        // The focused state updates on click/indicator interaction
    }
}
