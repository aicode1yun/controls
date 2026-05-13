#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Shiny.Maui.Controls.Collections;

internal class MauiElementFactory : IElementFactory
{
    readonly CollectionControlBase control;
    readonly IMauiContext mauiContext;
    readonly List<(View MauiView, FrameworkElement PlatformView)> recyclePool = [];

    public MauiElementFactory(CollectionControlBase control, IMauiContext mauiContext)
    {
        this.control = control;
        this.mauiContext = mauiContext;
    }

    public UIElement GetElement(ElementFactoryGetArgs args)
    {
        var item = args.Data;

        // Try to reuse from pool
        if (recyclePool.Count > 0)
        {
            var pooled = recyclePool[^1];
            recyclePool.RemoveAt(recyclePool.Count - 1);
            control.RecycleItemView(pooled.MauiView, item);
            return pooled.PlatformView;
        }

        // Create new
        var mauiView = control.CreateItemView(item);
        var platformView = mauiView.ToPlatform(mauiContext);
        platformView.Tag = mauiView;
        return platformView;
    }

    public void RecycleElement(ElementFactoryRecycleArgs args)
    {
        if (args.Element is FrameworkElement fe && fe.Tag is View mauiView)
        {
            mauiView.BindingContext = null;
            recyclePool.Add((mauiView, fe));
        }
    }
}
#endif
