#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Shiny.Maui.Controls.Collections;
using WinScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility;
using WinScrollMode = Microsoft.UI.Xaml.Controls.ScrollMode;

namespace Shiny.Maui.Controls.StaggeredGrid;

public partial class StaggeredGridHandler : ViewHandler<StaggeredGrid, ScrollViewer>
{
    ItemsRepeater? repeater;
    MauiElementFactory? elementFactory;
    WaterfallVirtualizingLayout? waterfallLayout;

    protected override ScrollViewer CreatePlatformView()
    {
        waterfallLayout = new WaterfallVirtualizingLayout(VirtualView);
        elementFactory = new MauiElementFactory(VirtualView, MauiContext!);

        repeater = new ItemsRepeater
        {
            Layout = waterfallLayout,
            ItemTemplate = elementFactory
        };

        var scrollViewer = new ScrollViewer
        {
            Content = repeater,
            HorizontalScrollBarVisibility = WinScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = WinScrollBarVisibility.Auto,
            HorizontalScrollMode = WinScrollMode.Disabled,
            VerticalScrollMode = WinScrollMode.Enabled
        };

        return scrollViewer;
    }

    protected override void ConnectHandler(ScrollViewer platformView)
    {
        base.ConnectHandler(platformView);
        UpdateItemsSource();
        if (repeater is not null)
            repeater.ElementPrepared += OnElementPrepared;
        platformView.ViewChanged += OnScrollViewChanged;
    }

    protected override void DisconnectHandler(ScrollViewer platformView)
    {
        if (repeater is not null)
            repeater.ElementPrepared -= OnElementPrepared;
        platformView.ViewChanged -= OnScrollViewChanged;
        base.DisconnectHandler(platformView);
    }

    void OnElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        var items = VirtualView.GetItemsList();
        if (args.Index < items.Count)
            VirtualView.RaiseItemAppearing(items[args.Index], args.Index);
    }

    void OnScrollViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (e.IsIntermediate)
            return;

        var items = VirtualView.GetItemsList();
        if (items.Count == 0)
            return;

        var scrollableHeight = PlatformView.ScrollableHeight;
        var verticalOffset = PlatformView.VerticalOffset;

        if (scrollableHeight > 0 && verticalOffset >= scrollableHeight * 0.8)
            VirtualView.RaiseLoadMoreRequested();
    }

    void UpdateItemsSource()
    {
        if (repeater is null)
            return;

        var items = VirtualView.GetItemsList();
        repeater.ItemsSource = items;
    }

    static partial void MapItemsSource(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.UpdateItemsSource();
    }

    static partial void MapColumnCount(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.UpdateItemsSource();
    }

    static partial void MapSpacing(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.UpdateItemsSource();
    }

    static partial void MapItemTemplate(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.UpdateItemsSource();
    }

    class WaterfallVirtualizingLayout : VirtualizingLayout
    {
        readonly StaggeredGrid grid;

        public WaterfallVirtualizingLayout(StaggeredGrid grid) => this.grid = grid;

        protected override Windows.Foundation.Size MeasureOverride(
            VirtualizingLayoutContext context, Windows.Foundation.Size availableSize)
        {
            var columnCount = Math.Max(1, grid.ColumnCount);
            var columnSpacing = grid.ColumnSpacing;
            var rowSpacing = grid.RowSpacing;
            var totalSpacing = columnSpacing * (columnCount - 1);
            var columnWidth = (availableSize.Width - totalSpacing) / columnCount;
            var columnHeights = new double[columnCount];

            for (var i = 0; i < context.ItemCount; i++)
            {
                var shortestCol = 0;
                for (var c = 1; c < columnCount; c++)
                {
                    if (columnHeights[c] < columnHeights[shortestCol])
                        shortestCol = c;
                }

                var element = context.GetOrCreateElementAt(i,
                    ElementRealizationOptions.ForceCreate);

                element.Measure(new Windows.Foundation.Size(columnWidth, double.PositiveInfinity));
                var itemHeight = element.DesiredSize.Height;

                var x = shortestCol * (columnWidth + columnSpacing);
                var y = columnHeights[shortestCol];
                if (y > 0)
                    y += rowSpacing;

                context.GetOrCreateElementAt(i).Arrange(
                    new Windows.Foundation.Rect(x, y, columnWidth, itemHeight));

                columnHeights[shortestCol] = y + itemHeight;
            }

            var maxHeight = columnHeights.Length > 0 ? columnHeights.Max() : 0;
            return new Windows.Foundation.Size(availableSize.Width, maxHeight);
        }

        protected override Windows.Foundation.Size ArrangeOverride(
            VirtualizingLayoutContext context, Windows.Foundation.Size finalSize)
        {
            // Arrangement is done in MeasureOverride for simplicity with waterfall
            return finalSize;
        }
    }
}
#endif
