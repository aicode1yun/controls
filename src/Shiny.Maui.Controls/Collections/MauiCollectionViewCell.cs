#if IOS || MACCATALYST
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Shiny.Maui.Controls.Collections;

internal class MauiCollectionViewCell : UICollectionViewCell
{
    public static readonly NSString ReuseId = new("MauiCollectionViewCell");

    IMauiContext? mauiContext;
    View? mauiView;
    UIView? platformView;

    [Export("initWithFrame:")]
    public MauiCollectionViewCell(CGRect frame) : base(frame)
    {
    }

    public View? MauiView => mauiView;

    public void Bind(CollectionControlBase control, IMauiContext context, object item)
    {
        mauiContext = context;

        if (mauiView is null)
        {
            mauiView = control.CreateItemView(item);
            platformView = mauiView.ToPlatform(mauiContext);
            ContentView.AddSubview(platformView);
        }
        else
        {
            control.RecycleItemView(mauiView, item);
        }

        SetNeedsLayout();
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        if (platformView is not null)
            platformView.Frame = ContentView.Bounds;
    }

    public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
        UICollectionViewLayoutAttributes layoutAttributes)
    {
        if (mauiView is null || mauiContext is null)
            return layoutAttributes;

        var targetWidth = layoutAttributes.Frame.Width;
        var size = ((IView)mauiView).Measure(targetWidth, double.PositiveInfinity);

        var newFrame = layoutAttributes.Frame;
        newFrame.Size = new CGSize(targetWidth, size.Height);
        layoutAttributes.Frame = newFrame;

        return layoutAttributes;
    }

    public override void PrepareForReuse()
    {
        base.PrepareForReuse();
        if (mauiView is not null)
            mauiView.BindingContext = null;
    }
}
#endif
