#if ANDROID
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Handlers;
using Shiny.Maui.Controls.Collections;
using AView = Android.Views.View;

namespace Shiny.Maui.Controls.CarouselGallery;

public partial class CarouselGalleryHandler : ViewHandler<CarouselGallery, RecyclerView>
{
    CarouselAdapter? adapter;
    LinearLayoutManager? layoutManager;
    LinearSnapHelper? snapHelper;
    bool isUpdatingPosition;

    protected override RecyclerView CreatePlatformView()
    {
        var recyclerView = new RecyclerView(Context);
        recyclerView.SetClipToPadding(false);

        layoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);
        recyclerView.SetLayoutManager(layoutManager);

        snapHelper = new LinearSnapHelper();
        snapHelper.AttachToRecyclerView(recyclerView);

        recyclerView.SetItemAnimator(new DefaultItemAnimator());

        return recyclerView;
    }

    protected override void ConnectHandler(RecyclerView platformView)
    {
        base.ConnectHandler(platformView);
        adapter = new CarouselAdapter(VirtualView, MauiContext!);
        platformView.SetAdapter(adapter);
        platformView.AddOnScrollListener(new CarouselScrollListener(this));
        UpdatePeekInsets();
        UpdateItemsSource();
    }

    protected override void DisconnectHandler(RecyclerView platformView)
    {
        platformView.SetAdapter(null);
        adapter?.Dispose();
        adapter = null;
        base.DisconnectHandler(platformView);
    }

    void UpdateItemsSource()
    {
        adapter?.UpdateItems();
    }

    void UpdatePeekInsets()
    {
        if (PlatformView is null)
            return;

        var density = Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        var insets = VirtualView.PeekAreaInsets;
        PlatformView.SetPadding(
            (int)(insets.Left * density),
            (int)(insets.Top * density),
            (int)(insets.Right * density),
            (int)(insets.Bottom * density));
    }

    void ApplyScaleTransforms()
    {
        if (layoutManager is null || PlatformView is null)
            return;

        var midX = PlatformView.Width / 2f;

        for (var i = 0; i < PlatformView.ChildCount; i++)
        {
            var child = PlatformView.GetChildAt(i);
            if (child is null)
                continue;

            var childMidX = (child.Left + child.Right) / 2f;
            var distFromCenter = Math.Abs(midX - childMidX) / (float)midX;
            distFromCenter = Math.Min(1f, distFromCenter);

            var scale = (float)(VirtualView.FocusedItemScale +
                (VirtualView.UnfocusedItemScale - VirtualView.FocusedItemScale) * distFromCenter);

            child.ScaleX = scale;
            child.ScaleY = scale;
        }
    }

    void OnScrollSettled()
    {
        if (isUpdatingPosition || layoutManager is null)
            return;

        var snapView = snapHelper?.FindSnapView(layoutManager);
        if (snapView is null)
            return;

        var position = layoutManager.GetPosition(snapView);
        if (position != RecyclerView.NoPosition && position != VirtualView.CurrentPosition)
        {
            isUpdatingPosition = true;
            VirtualView.RaisePositionChanged(position);
            isUpdatingPosition = false;
        }
    }

    static partial void MapItemsSource(CarouselGalleryHandler handler, CarouselGallery view)
    {
        handler.UpdateItemsSource();
    }

    static partial void MapCurrentPosition(CarouselGalleryHandler handler, CarouselGallery view)
    {
        if (handler.isUpdatingPosition)
            return;

        handler.layoutManager?.ScrollToPositionWithOffset(view.CurrentPosition, 0);
    }

    static partial void MapItemSize(CarouselGalleryHandler handler, CarouselGallery view)
    {
        handler.adapter?.NotifyDataSetChanged();
    }

    static partial void MapScaling(CarouselGalleryHandler handler, CarouselGallery view)
    {
        handler.ApplyScaleTransforms();
    }

    static partial void MapPeekInsets(CarouselGalleryHandler handler, CarouselGallery view)
    {
        handler.UpdatePeekInsets();
    }

    static partial void MapItemTemplate(CarouselGalleryHandler handler, CarouselGallery view)
    {
        handler.adapter?.NotifyDataSetChanged();
    }

    class CarouselAdapter : RecyclerView.Adapter
    {
        readonly CarouselGallery control;
        readonly IMauiContext mauiContext;
        IList<object> items = [];

        public CarouselAdapter(CarouselGallery control, IMauiContext mauiContext)
        {
            this.control = control;
            this.mauiContext = mauiContext;
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
                // Set fixed size for carousel items
                var density = mauiContext.Context!.Resources!.DisplayMetrics!.Density;
                holder.ItemView.LayoutParameters = new ViewGroup.LayoutParams(
                    (int)(control.ItemWidth * density),
                    (int)(control.ItemHeight * density));

                mauiHolder.Bind(control, items[position], position);
            }
        }
    }

    class CarouselScrollListener : RecyclerView.OnScrollListener
    {
        readonly CarouselGalleryHandler handler;

        public CarouselScrollListener(CarouselGalleryHandler handler) => this.handler = handler;

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            handler.ApplyScaleTransforms();
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            if (newState == RecyclerView.ScrollStateIdle)
                handler.OnScrollSettled();
        }
    }
}
#endif
