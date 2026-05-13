#if ANDROID
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Handlers;
using Shiny.Maui.Controls.Collections;
using AView = Android.Views.View;

namespace Shiny.Maui.Controls.StaggeredGrid;

public partial class StaggeredGridHandler : ViewHandler<StaggeredGrid, RecyclerView>
{
    StaggeredGridAdapter? adapter;
    StaggeredGridLayoutManager? layoutManager;

    protected override RecyclerView CreatePlatformView()
    {
        var recyclerView = new RecyclerView(Context);
        layoutManager = new StaggeredGridLayoutManager(
            VirtualView.ColumnCount,
            StaggeredGridLayoutManager.Vertical);
        layoutManager.GapStrategy = StaggeredGridLayoutManager.GapHandlingNone;

        recyclerView.SetLayoutManager(layoutManager);
        recyclerView.SetItemAnimator(new DefaultItemAnimator());

        return recyclerView;
    }

    protected override void ConnectHandler(RecyclerView platformView)
    {
        base.ConnectHandler(platformView);
        adapter = new StaggeredGridAdapter(VirtualView, MauiContext!);
        platformView.SetAdapter(adapter);
        platformView.AddOnScrollListener(new LoadMoreScrollListener(VirtualView));
        UpdateSpacing();
    }

    protected override void DisconnectHandler(RecyclerView platformView)
    {
        platformView.SetAdapter(null);
        adapter?.Dispose();
        adapter = null;
        base.DisconnectHandler(platformView);
    }

    static partial void MapItemsSource(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.adapter?.UpdateItems();
    }

    static partial void MapColumnCount(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.layoutManager?.SpanCount = view.ColumnCount;
        handler.adapter?.NotifyDataSetChanged();
    }

    static partial void MapSpacing(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.UpdateSpacing();
    }

    static partial void MapItemTemplate(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.adapter?.NotifyDataSetChanged();
    }

    void UpdateSpacing()
    {
        if (PlatformView is null)
            return;

        // Remove existing decorations
        while (PlatformView.ItemDecorationCount > 0)
            PlatformView.RemoveItemDecorationAt(0);

        PlatformView.AddItemDecoration(new SpacingDecoration(VirtualView));
    }

    class StaggeredGridAdapter : RecyclerView.Adapter
    {
        readonly CollectionControlBase control;
        readonly IMauiContext mauiContext;
        IList<object> items = [];

        public StaggeredGridAdapter(CollectionControlBase control, IMauiContext mauiContext)
        {
            this.control = control;
            this.mauiContext = mauiContext;
            UpdateItems();
        }

        public override int ItemCount => items.Count;

        public void UpdateItems()
        {
            items = control.GetItemsList();
            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new MauiViewHolder(mauiContext, parent);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is MauiViewHolder mauiHolder && position < items.Count)
            {
                mauiHolder.Bind(control, items[position], position);
                control.RaiseItemAppearing(items[position], position);
            }
        }

        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            base.OnViewRecycled(holder);
            if (holder is MauiViewHolder mauiHolder && mauiHolder.MauiView?.BindingContext is { } item)
            {
                var idx = items.IndexOf(item);
                if (idx >= 0)
                    control.RaiseItemDisappearing(item, idx);
                mauiHolder.Recycle();
            }
        }
    }

    class SpacingDecoration : RecyclerView.ItemDecoration
    {
        readonly StaggeredGrid grid;

        public SpacingDecoration(StaggeredGrid grid) => this.grid = grid;

        public override void GetItemOffsets(Android.Graphics.Rect outRect, AView view,
            RecyclerView parent, RecyclerView.State state)
        {
            var density = parent.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
            var hSpacing = (int)(grid.ColumnSpacing / 2 * density);
            var vSpacing = (int)(grid.RowSpacing / 2 * density);

            outRect.Left = hSpacing;
            outRect.Right = hSpacing;
            outRect.Top = vSpacing;
            outRect.Bottom = vSpacing;
        }
    }

    class LoadMoreScrollListener : RecyclerView.OnScrollListener
    {
        readonly CollectionControlBase control;
        bool isLoadingMore;

        public LoadMoreScrollListener(CollectionControlBase control) => this.control = control;

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            if (isLoadingMore || control.LoadMoreCommand is null)
                return;

            var layoutManager = recyclerView.GetLayoutManager();
            if (layoutManager is null)
                return;

            var totalItemCount = layoutManager.ItemCount;
            var lastVisible = GetLastVisiblePosition(layoutManager);

            if (lastVisible >= totalItemCount - control.LoadMoreThreshold)
            {
                isLoadingMore = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    control.RaiseLoadMoreRequested();
                    isLoadingMore = false;
                });
            }
        }

        static int GetLastVisiblePosition(RecyclerView.LayoutManager layoutManager)
        {
            if (layoutManager is StaggeredGridLayoutManager sglm)
            {
                var positions = new int[sglm.SpanCount];
                sglm.FindLastVisibleItemPositions(positions);
                return positions.Max();
            }
            return 0;
        }
    }
}
#endif
