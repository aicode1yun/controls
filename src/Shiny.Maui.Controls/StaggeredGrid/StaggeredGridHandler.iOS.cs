#if IOS || MACCATALYST
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using Shiny.Maui.Controls.Collections;
using UIKit;

namespace Shiny.Maui.Controls.StaggeredGrid;

public partial class StaggeredGridHandler : ViewHandler<StaggeredGrid, UICollectionView>
{
    WaterfallLayout? waterfallLayout;
    StaggeredDataSource? dataSource;
    StaggeredDelegate? collectionDelegate;

    protected override UICollectionView CreatePlatformView()
    {
        waterfallLayout = new WaterfallLayout(VirtualView);

        var collectionView = new UICollectionView(CGRect.Empty, waterfallLayout);
        collectionView.BackgroundColor = UIColor.Clear;
        collectionView.RegisterClassForCell(typeof(MauiCollectionViewCell), MauiCollectionViewCell.ReuseId);
        collectionView.AlwaysBounceVertical = true;

        return collectionView;
    }

    protected override void ConnectHandler(UICollectionView platformView)
    {
        base.ConnectHandler(platformView);
        waterfallLayout!.SetMauiContext(MauiContext!);
        dataSource = new StaggeredDataSource(VirtualView, MauiContext!);
        collectionDelegate = new StaggeredDelegate(VirtualView);
        platformView.DataSource = dataSource;
        platformView.Delegate = collectionDelegate;
    }

    protected override void DisconnectHandler(UICollectionView platformView)
    {
        platformView.DataSource = null;
        platformView.Delegate = null;
        dataSource?.Dispose();
        collectionDelegate?.Dispose();
        base.DisconnectHandler(platformView);
    }

    static partial void MapItemsSource(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.dataSource?.UpdateItems();
        handler.PlatformView?.ReloadData();
        handler.waterfallLayout?.InvalidateLayout();
    }

    static partial void MapColumnCount(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.waterfallLayout?.InvalidateLayout();
        handler.PlatformView?.ReloadData();
    }

    static partial void MapSpacing(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.waterfallLayout?.InvalidateLayout();
    }

    static partial void MapItemTemplate(StaggeredGridHandler handler, StaggeredGrid view)
    {
        handler.PlatformView?.ReloadData();
        handler.waterfallLayout?.InvalidateLayout();
    }

    class StaggeredDataSource : UICollectionViewDataSource
    {
        readonly CollectionControlBase control;
        readonly IMauiContext mauiContext;
        IList<object> items = [];

        public StaggeredDataSource(CollectionControlBase control, IMauiContext mauiContext)
        {
            this.control = control;
            this.mauiContext = mauiContext;
            UpdateItems();
        }

        public void UpdateItems()
        {
            items = control.GetItemsList();
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => items.Count;

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MauiCollectionViewCell)collectionView.DequeueReusableCell(
                MauiCollectionViewCell.ReuseId, indexPath);

            if (indexPath.Row < items.Count)
            {
                cell.Bind(control, mauiContext, items[indexPath.Row]);
                control.RaiseItemAppearing(items[indexPath.Row], indexPath.Row);
            }

            return cell;
        }
    }

    class StaggeredDelegate : UICollectionViewDelegate
    {
        readonly CollectionControlBase control;

        public StaggeredDelegate(CollectionControlBase control) => this.control = control;

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var items = control.GetItemsList();
            if (indexPath.Row < items.Count)
                control.RaiseItemSelected(items[indexPath.Row], indexPath.Row);
        }

        public override void CellDisplayingEnded(UICollectionView collectionView,
            UICollectionViewCell cell, NSIndexPath indexPath)
        {
            var items = control.GetItemsList();
            if (indexPath.Row < items.Count)
                control.RaiseItemDisappearing(items[indexPath.Row], indexPath.Row);
        }

        public override void WillDisplayCell(UICollectionView collectionView,
            UICollectionViewCell cell, NSIndexPath indexPath)
        {
            var items = control.GetItemsList();

            // Load more check
            if (indexPath.Row >= items.Count - control.LoadMoreThreshold)
                control.RaiseLoadMoreRequested();
        }
    }

    // Custom waterfall/masonry layout — pre-measures each item's MAUI view
    // so columns can stagger based on real content heights.
    class WaterfallLayout : UICollectionViewLayout
    {
        readonly StaggeredGrid grid;
        readonly List<UICollectionViewLayoutAttributes> allAttributes = [];
        readonly Dictionary<DataTemplate, View> sizingViews = new();
        IMauiContext? mauiContext;
        nfloat contentHeight;
        nfloat lastLayoutWidth;

        public WaterfallLayout(StaggeredGrid grid) => this.grid = grid;

        public void SetMauiContext(IMauiContext context) => mauiContext = context;

        public override CGSize CollectionViewContentSize =>
            new(CollectionView?.Bounds.Width ?? 0, contentHeight);

        public override void PrepareLayout()
        {
            base.PrepareLayout();
            allAttributes.Clear();
            contentHeight = 0;

            if (CollectionView is null)
                return;

            var columnCount = Math.Max(1, grid.ColumnCount);
            var totalWidth = CollectionView.Bounds.Width;
            lastLayoutWidth = totalWidth;
            var columnSpacing = (nfloat)grid.ColumnSpacing;
            var rowSpacing = (nfloat)grid.RowSpacing;

            var totalSpacing = columnSpacing * (columnCount - 1);
            var columnWidth = (totalWidth - totalSpacing) / columnCount;
            if (columnWidth <= 0)
                return;

            var columnHeights = new nfloat[columnCount];
            var items = grid.GetItemsList();

            for (var i = 0; i < items.Count; i++)
            {
                var shortestColumn = 0;
                for (var c = 1; c < columnCount; c++)
                {
                    if (columnHeights[c] < columnHeights[shortestColumn])
                        shortestColumn = c;
                }

                var x = shortestColumn * (columnWidth + columnSpacing);
                var y = columnHeights[shortestColumn];
                if (y > 0)
                    y += rowSpacing;

                var itemHeight = MeasureItemHeight(items[i], (double)columnWidth);

                var indexPath = NSIndexPath.FromRowSection(i, 0);
                var attrs = UICollectionViewLayoutAttributes.CreateForCell(indexPath);
                attrs.Frame = new CGRect(x, y, columnWidth, (nfloat)itemHeight);

                allAttributes.Add(attrs);
                columnHeights[shortestColumn] = y + (nfloat)itemHeight;
            }

            contentHeight = columnHeights.Length > 0 ? columnHeights.Max() : 0;
        }

        nfloat MeasureItemHeight(object item, double width)
        {
            if (mauiContext is null)
                return (nfloat)width;

            var template = grid.ResolveTemplate(item);
            if (!sizingViews.TryGetValue(template, out var view))
            {
                view = grid.CreateItemView(item);
                // Create a platform handler so Measure() works through the native layout system.
                // Without this, the detached MAUI view can't measure complex layouts correctly.
                view.ToPlatform(mauiContext);
                sizingViews[template] = view;
            }
            else
            {
                view.BindingContext = item;
            }

            // If the root view has an explicit HeightRequest (common for staggered items),
            // use it directly — it's the most reliable signal and avoids measurement issues
            // with remote images or async content.
            if (view.HeightRequest > 0)
                return (nfloat)view.HeightRequest;

            var size = ((IView)view).Measure(width, double.PositiveInfinity);
            return (nfloat)Math.Max(1, size.Height);
        }

        public override UICollectionViewLayoutAttributes[]? LayoutAttributesForElementsInRect(CGRect rect)
        {
            return allAttributes.Where(a => a.Frame.IntersectsWith(rect)).ToArray();
        }

        public override UICollectionViewLayoutAttributes? LayoutAttributesForItem(NSIndexPath indexPath)
        {
            return indexPath.Row < allAttributes.Count ? allAttributes[indexPath.Row] : null;
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
        {
            return CollectionView is not null && lastLayoutWidth != newBounds.Width;
        }

        // We've already pre-measured every cell in PrepareLayout, so ignore the
        // cell's preferred attributes — they would otherwise cause a layout pass
        // per visible cell.
        public override bool ShouldInvalidateLayout(UICollectionViewLayoutAttributes preferredAttributes, UICollectionViewLayoutAttributes originalAttributes) => false;
    }
}
#endif
