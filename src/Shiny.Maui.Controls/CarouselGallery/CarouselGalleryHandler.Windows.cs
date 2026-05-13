#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Shiny.Maui.Controls.Collections;
using WinScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility;
using WinScrollMode = Microsoft.UI.Xaml.Controls.ScrollMode;

namespace Shiny.Maui.Controls.CarouselGallery;

public partial class CarouselGalleryHandler : ViewHandler<CarouselGallery, ScrollViewer>
{
    ItemsRepeater? repeater;
    MauiElementFactory? elementFactory;
    Microsoft.UI.Xaml.Controls.StackLayout? stackLayout;
    bool isUpdatingPosition;

    protected override ScrollViewer CreatePlatformView()
    {
        stackLayout = new Microsoft.UI.Xaml.Controls.StackLayout
        {
            Orientation = Orientation.Horizontal,
            Spacing = VirtualView.ItemSpacing
        };

        elementFactory = new MauiElementFactory(VirtualView, MauiContext!);

        repeater = new ItemsRepeater
        {
            Layout = stackLayout,
            ItemTemplate = elementFactory
        };

        var scrollViewer = new ScrollViewer
        {
            Content = repeater,
            HorizontalScrollBarVisibility = WinScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = WinScrollBarVisibility.Disabled,
            HorizontalScrollMode = WinScrollMode.Enabled,
            VerticalScrollMode = WinScrollMode.Disabled,
            HorizontalSnapPointsType = VirtualView.SnapCount >= 1
                ? Microsoft.UI.Xaml.Controls.SnapPointsType.MandatorySingle
                : Microsoft.UI.Xaml.Controls.SnapPointsType.None,
            HorizontalSnapPointsAlignment = Microsoft.UI.Xaml.Controls.Primitives.SnapPointsAlignment.Center,
            ZoomMode = ZoomMode.Disabled
        };

        return scrollViewer;
    }

    protected override void ConnectHandler(ScrollViewer platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ViewChanged += OnScrollViewChanged;
        UpdatePeekInsets();
        UpdateItemsSource();
    }

    protected override void DisconnectHandler(ScrollViewer platformView)
    {
        platformView.ViewChanged -= OnScrollViewChanged;
        base.DisconnectHandler(platformView);
    }

    void OnScrollViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        ApplyScaleTransforms();
        if (!e.IsIntermediate)
            OnScrollSettled();
    }

    void UpdateItemsSource()
    {
        if (repeater is null)
            return;

        var items = VirtualView.GetItemsList();
        repeater.ItemsSource = items;
    }

    void UpdatePeekInsets()
    {
        if (PlatformView is null)
            return;

        var insets = VirtualView.PeekAreaInsets;
        PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(
            insets.Left, insets.Top, insets.Right, insets.Bottom);
    }

    void ApplyScaleTransforms()
    {
        if (repeater is null || PlatformView is null)
            return;

        var midX = PlatformView.HorizontalOffset + PlatformView.ViewportWidth / 2;

        for (var i = 0; i < repeater.ItemsSourceView?.Count; i++)
        {
            var element = repeater.TryGetElement(i);
            if (element is null)
                continue;

            var transform = element.TransformToVisual(repeater);
            var point = transform.TransformPoint(new Windows.Foundation.Point(
                element.ActualSize.X / 2, 0));

            var distance = Math.Abs(midX - point.X);
            var maxDistance = PlatformView.ViewportWidth / 2;
            var normalizedDist = Math.Min(1.0, distance / maxDistance);

            var scale = VirtualView.FocusedItemScale +
                (VirtualView.UnfocusedItemScale - VirtualView.FocusedItemScale) * normalizedDist;

            element.RenderTransform = new Microsoft.UI.Xaml.Media.ScaleTransform
            {
                ScaleX = scale,
                ScaleY = scale,
                CenterX = element.ActualSize.X / 2,
                CenterY = element.ActualSize.Y / 2
            };
        }
    }

    void OnScrollSettled()
    {
        if (isUpdatingPosition || repeater is null || PlatformView is null)
            return;

        var midX = PlatformView.HorizontalOffset + PlatformView.ViewportWidth / 2;
        var closestIndex = -1;
        var minDistance = double.MaxValue;

        for (var i = 0; i < (repeater.ItemsSourceView?.Count ?? 0); i++)
        {
            var element = repeater.TryGetElement(i);
            if (element is null)
                continue;

            var transform = element.TransformToVisual(repeater);
            var point = transform.TransformPoint(new Windows.Foundation.Point(
                element.ActualSize.X / 2, 0));

            var distance = Math.Abs(midX - point.X);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        if (closestIndex >= 0 && closestIndex != VirtualView.CurrentPosition)
        {
            isUpdatingPosition = true;
            VirtualView.RaisePositionChanged(closestIndex);
            isUpdatingPosition = false;
        }
    }

    static partial void MapItemsSource(CarouselGalleryHandler handler, CarouselGallery view)
    {
        handler.UpdateItemsSource();
    }

    static partial void MapCurrentPosition(CarouselGalleryHandler handler, CarouselGallery view)
    {
        if (handler.isUpdatingPosition || handler.repeater is null)
            return;

        var element = handler.repeater.TryGetElement(view.CurrentPosition);
        if (element is not null)
            element.StartBringIntoView(new BringIntoViewOptions { AnimationDesired = true });
    }

    static partial void MapItemSize(CarouselGalleryHandler handler, CarouselGallery view)
    {
        // Items will be sized via the MAUI template
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
        handler.UpdateItemsSource();
    }

    static partial void MapSnapCount(CarouselGalleryHandler handler, CarouselGallery view)
    {
        if (handler.PlatformView is null)
            return;

        handler.PlatformView.HorizontalSnapPointsType = view.SnapCount >= 1
            ? Microsoft.UI.Xaml.Controls.SnapPointsType.MandatorySingle
            : Microsoft.UI.Xaml.Controls.SnapPointsType.None;
    }
}
#endif
