#if ANDROID
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Shiny.Maui.Controls.Collections;
using AView = Android.Views.View;

namespace Shiny.Maui.Controls.VirtualizedGrid;

public partial class VirtualizedGridHandler : ViewHandler<VirtualizedGrid, RecyclerView>
{
    GridAdapter? adapter;
    GridLayoutManager? layoutManager;
    StickyHeaderDecoration? stickyDecoration;
    CellPaddingDecoration? paddingDecoration;

    protected override RecyclerView CreatePlatformView()
    {
        var recyclerView = new RecyclerView(Context);

        var columnCount = VirtualView.GetEffectiveColumnCount();
        layoutManager = new GridLayoutManager(Context, columnCount);
        recyclerView.SetLayoutManager(layoutManager);
        recyclerView.SetItemAnimator(new DefaultItemAnimator());

        return recyclerView;
    }

    protected override void ConnectHandler(RecyclerView platformView)
    {
        base.ConnectHandler(platformView);

        adapter = new GridAdapter(VirtualView, MauiContext!);

        // Group headers span the full width
        layoutManager!.SetSpanSizeLookup(new GroupSpanSizeLookup(adapter, layoutManager));

        platformView.SetAdapter(adapter);
        platformView.AddOnScrollListener(new GridScrollListener(VirtualView, adapter));

        UpdatePaddingDecoration();
        UpdateStickyHeaders();
    }

    protected override void DisconnectHandler(RecyclerView platformView)
    {
        platformView.SetAdapter(null);
        adapter?.Dispose();
        adapter = null;
        base.DisconnectHandler(platformView);
    }

    void UpdatePaddingDecoration()
    {
        if (PlatformView is null)
            return;

        if (paddingDecoration is not null)
            PlatformView.RemoveItemDecoration(paddingDecoration);

        paddingDecoration = new CellPaddingDecoration(VirtualView);
        PlatformView.AddItemDecoration(paddingDecoration);
    }

    void UpdateStickyHeaders()
    {
        if (PlatformView is null || adapter is null)
            return;

        if (stickyDecoration is not null)
            PlatformView.RemoveItemDecoration(stickyDecoration);

        if (VirtualView.HasStickyHeaders && VirtualView.IsGroupingEnabled)
        {
            stickyDecoration = new StickyHeaderDecoration(adapter, VirtualView, MauiContext!);
            PlatformView.AddItemDecoration(stickyDecoration);
        }
    }

    static partial void MapItemsSource(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.adapter?.UpdateItems();
    }

    static partial void MapColumnCount(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        var columnCount = view.GetEffectiveColumnCount();
        if (handler.layoutManager is not null)
            handler.layoutManager.SpanCount = columnCount;
        handler.adapter?.NotifyDataSetChanged();
    }

    static partial void MapGrouping(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.adapter?.UpdateItems();
        handler.UpdateStickyHeaders();
    }

    static partial void MapStickyHeaders(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.UpdateStickyHeaders();
    }

    static partial void MapCellPadding(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.UpdatePaddingDecoration();
    }

    static partial void MapItemTemplate(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.adapter?.NotifyDataSetChanged();
    }

    static partial void MapLoadMore(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.adapter?.UpdateItems();
    }

    internal enum ItemType
    {
        Item = 0,
        GroupHeader = 1,
        LoadMoreButton = 2,
        Header = 3,
        Footer = 4
    }

    internal class FlatItem
    {
        public ItemType Type { get; init; }
        public object? Data { get; init; }
        public int GroupIndex { get; init; } = -1;
    }

    internal class GridAdapter : RecyclerView.Adapter
    {
        readonly VirtualizedGrid control;
        readonly IMauiContext mauiContext;
        List<FlatItem> flatItems = [];

        public GridAdapter(VirtualizedGrid control, IMauiContext mauiContext)
        {
            this.control = control;
            this.mauiContext = mauiContext;
            UpdateItems();
        }

        public override int ItemCount => flatItems.Count;
        public List<FlatItem> FlatItems => flatItems;

        public FlatItem GetFlatItem(int position) =>
            position >= 0 && position < flatItems.Count ? flatItems[position] : new FlatItem { Type = ItemType.Item };

        public void UpdateItems()
        {
            flatItems = BuildFlatList();
            NotifyDataSetChanged();
        }

        List<FlatItem> BuildFlatList()
        {
            var result = new List<FlatItem>();

            // Header
            if (control.HeaderTemplate is not null)
                result.Add(new FlatItem { Type = ItemType.Header });

            if (control.IsGroupingEnabled)
            {
                var (groups, _) = control.GetGroupedData();
                for (var g = 0; g < groups.Count; g++)
                {
                    result.Add(new FlatItem
                    {
                        Type = ItemType.GroupHeader,
                        Data = groups[g].Key,
                        GroupIndex = g
                    });

                    foreach (var item in groups[g].Items)
                    {
                        result.Add(new FlatItem
                        {
                            Type = ItemType.Item,
                            Data = item,
                            GroupIndex = g
                        });
                    }
                }
            }
            else
            {
                var items = control.GetItemsList();
                foreach (var item in items)
                    result.Add(new FlatItem { Type = ItemType.Item, Data = item });
            }

            // Load more button
            if (control.ShowLoadMoreButton)
                result.Add(new FlatItem { Type = ItemType.LoadMoreButton });

            // Footer
            if (control.FooterTemplate is not null)
                result.Add(new FlatItem { Type = ItemType.Footer });

            return result;
        }

        public override int GetItemViewType(int position) => (int)GetFlatItem(position).Type;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new MauiViewHolder(mauiContext, parent);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is not MauiViewHolder mauiHolder)
                return;

            var flatItem = GetFlatItem(position);

            switch (flatItem.Type)
            {
                case ItemType.Item when flatItem.Data is not null:
                    mauiHolder.Bind(control, flatItem.Data, position);
                    control.RaiseItemVisible(flatItem.Data, position);
                    break;

                case ItemType.GroupHeader when flatItem.Data is not null:
                    var headerView = control.CreateGroupHeaderView(flatItem.Data);
                    mauiHolder.Bind(control, flatItem.Data, position);
                    break;

                case ItemType.LoadMoreButton:
                    mauiHolder.BindCustomView(control.CreateLoadMoreView(), mauiContext);
                    break;

                case ItemType.Header when control.HeaderTemplate is not null:
                    mauiHolder.Bind(control, new TemplateMarker("Header"), position);
                    break;

                case ItemType.Footer when control.FooterTemplate is not null:
                    mauiHolder.Bind(control, new TemplateMarker("Footer"), position);
                    break;
            }
        }

        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            base.OnViewRecycled(holder);
            if (holder is MauiViewHolder mauiHolder && mauiHolder.MauiView?.BindingContext is { } item)
            {
                control.RaiseItemHidden(item, mauiHolder.BindingAdapterPosition);
                mauiHolder.Recycle();
            }
        }
    }

    class GroupSpanSizeLookup : GridLayoutManager.SpanSizeLookup
    {
        readonly GridAdapter adapter;
        readonly GridLayoutManager layoutManager;

        public GroupSpanSizeLookup(GridAdapter adapter, GridLayoutManager layoutManager)
        {
            this.adapter = adapter;
            this.layoutManager = layoutManager;
        }

        public override int GetSpanSize(int position)
        {
            var flatItem = adapter.GetFlatItem(position);
            // Group headers, header, footer, and load-more span full width
            return flatItem.Type == ItemType.Item ? 1 : layoutManager.SpanCount;
        }
    }

    class CellPaddingDecoration : RecyclerView.ItemDecoration
    {
        readonly VirtualizedGrid grid;

        public CellPaddingDecoration(VirtualizedGrid grid) => this.grid = grid;

        public override void GetItemOffsets(Android.Graphics.Rect outRect, AView view,
            RecyclerView parent, RecyclerView.State state)
        {
            var density = parent.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
            var padding = grid.CellPadding;
            var spacing = grid.ItemSpacing;

            outRect.Left = (int)((padding.Left + spacing / 2) * density);
            outRect.Right = (int)((padding.Right + spacing / 2) * density);
            outRect.Top = (int)((padding.Top + spacing / 2) * density);
            outRect.Bottom = (int)((padding.Bottom + spacing / 2) * density);
        }
    }

    class StickyHeaderDecoration : RecyclerView.ItemDecoration
    {
        readonly GridAdapter adapter;
        readonly VirtualizedGrid control;
        readonly IMauiContext mauiContext;

        public StickyHeaderDecoration(GridAdapter adapter, VirtualizedGrid control, IMauiContext mauiContext)
        {
            this.adapter = adapter;
            this.control = control;
            this.mauiContext = mauiContext;
        }

        public override void OnDrawOver(Canvas c, RecyclerView parent, RecyclerView.State state)
        {
            base.OnDrawOver(c, parent, state);

            if (parent.ChildCount == 0)
                return;

            var topChild = parent.GetChildAt(0);
            if (topChild is null)
                return;

            var topPosition = parent.GetChildAdapterPosition(topChild);
            if (topPosition == RecyclerView.NoPosition)
                return;

            // Find the group header for the current top position
            var headerPosition = FindGroupHeaderBefore(topPosition);
            if (headerPosition < 0)
                return;

            var flatItem = adapter.GetFlatItem(headerPosition);
            if (flatItem.Data is null)
                return;

            // Create and draw the sticky header
            var headerView = control.CreateGroupHeaderView(flatItem.Data);
            var platformHeader = headerView.ToPlatform(mauiContext);

            var widthSpec = AView.MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly);
            var heightSpec = AView.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            platformHeader.Measure(widthSpec, heightSpec);
            platformHeader.Layout(0, 0, parent.Width, platformHeader.MeasuredHeight);

            c.Save();
            c.ClipRect(0, 0, parent.Width, platformHeader.MeasuredHeight);
            platformHeader.Draw(c);
            c.Restore();
        }

        int FindGroupHeaderBefore(int position)
        {
            for (var i = position; i >= 0; i--)
            {
                if (adapter.GetFlatItem(i).Type == ItemType.GroupHeader)
                    return i;
            }
            return -1;
        }
    }

    class GridScrollListener : RecyclerView.OnScrollListener
    {
        readonly VirtualizedGrid control;
        readonly GridAdapter adapter;
        bool isLoadingMore;

        public GridScrollListener(VirtualizedGrid control, GridAdapter adapter)
        {
            this.control = control;
            this.adapter = adapter;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            if (control.ShowLoadMoreButton || isLoadingMore || control.LoadMoreCommand is null)
                return;

            var layoutManager = recyclerView.GetLayoutManager() as GridLayoutManager;
            if (layoutManager is null)
                return;

            var totalItemCount = layoutManager.ItemCount;
            var lastVisible = layoutManager.FindLastVisibleItemPosition();

            if (lastVisible >= totalItemCount - control.LoadMoreThreshold)
            {
                isLoadingMore = true;
                control.IsLoadingMore = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    control.RaiseLoadMoreRequested();
                    control.IsLoadingMore = false;
                    isLoadingMore = false;
                });
            }
        }
    }

    internal record TemplateMarker(string Name);
}
#endif
