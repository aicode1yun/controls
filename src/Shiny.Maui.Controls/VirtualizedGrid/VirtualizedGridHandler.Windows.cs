#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Shiny.Maui.Controls.Collections;
using WinScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility;
using WinScrollMode = Microsoft.UI.Xaml.Controls.ScrollMode;
using WinHorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;

namespace Shiny.Maui.Controls.VirtualizedGrid;

public partial class VirtualizedGridHandler : ViewHandler<VirtualizedGrid, ScrollViewer>
{
    ItemsRepeater? repeater;
    MauiElementFactory? elementFactory;
    UniformGridLayout? gridLayout;
    StackPanel? rootPanel;
    bool isLoadingMore;

    protected override ScrollViewer CreatePlatformView()
    {
        gridLayout = new UniformGridLayout
        {
            Orientation = Orientation.Horizontal,
            MinItemWidth = 0,
            MinItemHeight = 0,
            MinRowSpacing = VirtualView.ItemSpacing,
            MinColumnSpacing = VirtualView.ItemSpacing,
            ItemsStretch = UniformGridLayoutItemsStretch.Fill,
            MaximumRowsOrColumns = VirtualView.GetEffectiveColumnCount()
        };

        elementFactory = new MauiElementFactory(VirtualView, MauiContext!);
        rootPanel = new StackPanel { Orientation = Orientation.Vertical };

        var scrollViewer = new ScrollViewer
        {
            Content = rootPanel,
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
        platformView.ViewChanged += OnScrollViewChanged;
        RebuildContent();
    }

    protected override void DisconnectHandler(ScrollViewer platformView)
    {
        platformView.ViewChanged -= OnScrollViewChanged;
        base.DisconnectHandler(platformView);
    }

    void OnScrollViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (e.IsIntermediate || VirtualView.ShowLoadMoreButton || isLoadingMore)
            return;

        var scrollableHeight = PlatformView.ScrollableHeight;
        var verticalOffset = PlatformView.VerticalOffset;

        if (scrollableHeight > 0 && verticalOffset >= scrollableHeight * 0.8)
        {
            isLoadingMore = true;
            VirtualView.IsLoadingMore = true;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                VirtualView.RaiseLoadMoreRequested();
                VirtualView.IsLoadingMore = false;
                isLoadingMore = false;
            });
        }
    }

    void RebuildContent()
    {
        if (rootPanel is null || elementFactory is null)
            return;

        rootPanel.Children.Clear();

        if (VirtualView.IsGroupingEnabled)
        {
            var (groups, _) = VirtualView.GetGroupedData();
            foreach (var group in groups)
            {
                // Group header
                if (VirtualView.GroupHeaderTemplate is not null)
                {
                    var headerView = VirtualView.CreateGroupHeaderView(group.Key);
                    var headerPlatform = headerView.ToPlatform(MauiContext!);
                    rootPanel.Children.Add(headerPlatform);
                }

                // Group items in their own ItemsRepeater
                var groupRepeater = new ItemsRepeater
                {
                    Layout = CreateGridLayout(),
                    ItemTemplate = elementFactory,
                    ItemsSource = group.Items
                };
                groupRepeater.ElementPrepared += OnElementPrepared;
                rootPanel.Children.Add(groupRepeater);
            }
        }
        else
        {
            var items = VirtualView.GetItemsList();
            repeater = new ItemsRepeater
            {
                Layout = gridLayout,
                ItemTemplate = elementFactory,
                ItemsSource = items
            };
            repeater.ElementPrepared += OnElementPrepared;
            rootPanel.Children.Add(repeater);
        }

        // Load more button
        if (VirtualView.ShowLoadMoreButton)
        {
            var loadMoreView = VirtualView.CreateLoadMoreView();
            var loadMorePlatform = loadMoreView.ToPlatform(MauiContext!);
            rootPanel.Children.Add(loadMorePlatform);
        }
    }

    UniformGridLayout CreateGridLayout()
    {
        return new UniformGridLayout
        {
            Orientation = Orientation.Horizontal,
            MinRowSpacing = VirtualView.ItemSpacing,
            MinColumnSpacing = VirtualView.ItemSpacing,
            ItemsStretch = UniformGridLayoutItemsStretch.Fill,
            MaximumRowsOrColumns = VirtualView.GetEffectiveColumnCount()
        };
    }

    void OnElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        var items = VirtualView.GetItemsList();
        if (args.Index < items.Count)
            VirtualView.RaiseItemVisible(items[args.Index], args.Index);
    }

    static partial void MapItemsSource(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.RebuildContent();
    }

    static partial void MapColumnCount(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        if (handler.gridLayout is not null)
            handler.gridLayout.MaximumRowsOrColumns = view.GetEffectiveColumnCount();
        handler.RebuildContent();
    }

    static partial void MapGrouping(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.RebuildContent();
    }

    static partial void MapStickyHeaders(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        // WinUI doesn't natively support sticky headers in ItemsRepeater.
        // For grouped content, headers stay in their natural scroll position.
        handler.RebuildContent();
    }

    static partial void MapCellPadding(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.RebuildContent();
    }

    static partial void MapItemTemplate(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.RebuildContent();
    }

    static partial void MapLoadMore(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.RebuildContent();
    }
}
#endif
