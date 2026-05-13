#if IOS || MACCATALYST
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Shiny.Maui.Controls.Collections;
using UIKit;

namespace Shiny.Maui.Controls.VirtualizedGrid;

public partial class VirtualizedGridHandler : ViewHandler<VirtualizedGrid, UICollectionView>
{
    static readonly NSString SectionHeaderKind = new("UICollectionElementKindSectionHeader");
    static readonly NSString SectionFooterKind = new("UICollectionElementKindSectionFooter");

    UICollectionViewCompositionalLayout? compositionalLayout;
    GridDataSource? dataSource;
    GridDelegate? gridDelegate;

    protected override UICollectionView CreatePlatformView()
    {
        compositionalLayout = CreateLayout();

        var collectionView = new UICollectionView(CGRect.Empty, compositionalLayout)
        {
            BackgroundColor = UIColor.Clear,
            AlwaysBounceVertical = true
        };

        collectionView.RegisterClassForCell(typeof(MauiCollectionViewCell), MauiCollectionViewCell.ReuseId);
        collectionView.RegisterClassForSupplementaryView(
            typeof(MauiSupplementaryView),
            SectionHeaderKind,
            MauiSupplementaryView.ReuseId);
        collectionView.RegisterClassForSupplementaryView(
            typeof(MauiSupplementaryView),
            SectionFooterKind,
            MauiSupplementaryView.ReuseId);

        return collectionView;
    }

    protected override void ConnectHandler(UICollectionView platformView)
    {
        base.ConnectHandler(platformView);
        dataSource = new GridDataSource(VirtualView, MauiContext!);
        gridDelegate = new GridDelegate(VirtualView, dataSource);
        platformView.DataSource = dataSource;
        platformView.Delegate = gridDelegate;
    }

    protected override void DisconnectHandler(UICollectionView platformView)
    {
        platformView.DataSource = null;
        platformView.Delegate = null;
        dataSource?.Dispose();
        gridDelegate?.Dispose();
        base.DisconnectHandler(platformView);
    }

    UICollectionViewCompositionalLayout CreateLayout()
    {
        return new UICollectionViewCompositionalLayout((sectionIndex, environment) =>
        {
            var columnCount = VirtualView.GetEffectiveColumnCount();
            var spacing = (nfloat)VirtualView.ItemSpacing;
            var padding = VirtualView.CellPadding;

            var itemSize = NSCollectionLayoutSize.Create(
                NSCollectionLayoutDimension.CreateFractionalWidth(1f / columnCount),
                NSCollectionLayoutDimension.CreateEstimated(100));

            var item = NSCollectionLayoutItem.Create(itemSize);
            item.ContentInsets = new NSDirectionalEdgeInsets(
                (nfloat)padding.Top, (nfloat)padding.Left,
                (nfloat)padding.Bottom, (nfloat)padding.Right);

            var groupSize = NSCollectionLayoutSize.Create(
                NSCollectionLayoutDimension.CreateFractionalWidth(1f),
                NSCollectionLayoutDimension.CreateEstimated(100));

            var group = NSCollectionLayoutGroup.CreateHorizontal(groupSize, item, columnCount);
            group.InterItemSpacing = NSCollectionLayoutSpacing.CreateFixed(spacing);

            var section = NSCollectionLayoutSection.Create(group);
            section.InterGroupSpacing = spacing;

            var supplementaryItems = new List<NSCollectionLayoutBoundarySupplementaryItem>();

            // Section header for grouping
            if (VirtualView.IsGroupingEnabled && VirtualView.GroupHeaderTemplate is not null)
            {
                var headerSize = NSCollectionLayoutSize.Create(
                    NSCollectionLayoutDimension.CreateFractionalWidth(1f),
                    NSCollectionLayoutDimension.CreateEstimated(44));

                var header = NSCollectionLayoutBoundarySupplementaryItem.Create(
                    headerSize,
                    SectionHeaderKind,
                    NSRectAlignment.Top);

                header.PinToVisibleBounds = VirtualView.HasStickyHeaders;
                supplementaryItems.Add(header);
            }

            // Load more footer on last section
            if (VirtualView.ShowLoadMoreButton && dataSource is not null &&
                (int)sectionIndex == dataSource.Sections.Count - 1)
            {
                var footerSize = NSCollectionLayoutSize.Create(
                    NSCollectionLayoutDimension.CreateFractionalWidth(1f),
                    NSCollectionLayoutDimension.CreateEstimated(44));

                var footer = NSCollectionLayoutBoundarySupplementaryItem.Create(
                    footerSize,
                    SectionFooterKind,
                    NSRectAlignment.Bottom);

                supplementaryItems.Add(footer);
            }

            if (supplementaryItems.Count > 0)
                section.BoundarySupplementaryItems = supplementaryItems.ToArray();

            return section;
        });
    }

    void ReloadLayout()
    {
        if (PlatformView is null)
            return;

        compositionalLayout = CreateLayout();
        PlatformView.SetCollectionViewLayout(compositionalLayout, true);
        dataSource?.UpdateData();
        PlatformView.ReloadData();
    }

    static partial void MapItemsSource(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.dataSource?.UpdateData();
        handler.PlatformView?.ReloadData();
    }

    static partial void MapColumnCount(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.ReloadLayout();
    }

    static partial void MapGrouping(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.ReloadLayout();
    }

    static partial void MapStickyHeaders(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.ReloadLayout();
    }

    static partial void MapCellPadding(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.ReloadLayout();
    }

    static partial void MapItemTemplate(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.PlatformView?.ReloadData();
    }

    static partial void MapLoadMore(VirtualizedGridHandler handler, VirtualizedGrid view)
    {
        handler.dataSource?.UpdateData();
        handler.PlatformView?.ReloadData();
    }

    class SectionData
    {
        public object? GroupKey { get; init; }
        public IList<object> Items { get; init; } = [];
    }

    class GridDataSource : UICollectionViewDataSource
    {
        readonly VirtualizedGrid control;
        readonly IMauiContext mauiContext;
        List<SectionData> sections = [];

        public GridDataSource(VirtualizedGrid control, IMauiContext mauiContext)
        {
            this.control = control;
            this.mauiContext = mauiContext;
            UpdateData();
        }

        public List<SectionData> Sections => sections;

        public void UpdateData()
        {
            sections = BuildSections();
        }

        List<SectionData> BuildSections()
        {
            var result = new List<SectionData>();

            if (control.IsGroupingEnabled)
            {
                var (groups, _) = control.GetGroupedData();
                foreach (var group in groups)
                {
                    var sectionItems = new List<object>(group.Items);
                    result.Add(new SectionData { GroupKey = group.Key, Items = sectionItems });
                }
            }
            else
            {
                var items = control.GetItemsList();
                result.Add(new SectionData { Items = new List<object>(items) });
            }

            return result;
        }

        public override nint NumberOfSections(UICollectionView collectionView) => sections.Count;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) =>
            section < sections.Count ? sections[(int)section].Items.Count : 0;

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (MauiCollectionViewCell)collectionView.DequeueReusableCell(
                MauiCollectionViewCell.ReuseId, indexPath);

            if (indexPath.Section < sections.Count && indexPath.Row < sections[indexPath.Section].Items.Count)
            {
                var item = sections[indexPath.Section].Items[indexPath.Row];
                cell.Bind(control, mauiContext, item);
                control.RaiseItemVisible(item, indexPath.Row);
            }

            return cell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(
            UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var view = (MauiSupplementaryView)collectionView.DequeueReusableSupplementaryView(
                elementKind, MauiSupplementaryView.ReuseId, indexPath);

            if (elementKind.ToString() == SectionHeaderKind.ToString() &&
                indexPath.Section < sections.Count &&
                sections[indexPath.Section].GroupKey is { } key)
            {
                view.Bind(control, mauiContext, key);
            }
            else if (elementKind.ToString() == SectionFooterKind.ToString() &&
                     control.ShowLoadMoreButton)
            {
                view.BindLoadMore(control, mauiContext);
            }

            return view;
        }
    }

    class GridDelegate : UICollectionViewDelegate
    {
        readonly VirtualizedGrid control;
        readonly GridDataSource dataSource;
        bool isLoadingMore;

        public GridDelegate(VirtualizedGrid control, GridDataSource dataSource)
        {
            this.control = control;
            this.dataSource = dataSource;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath.Section < dataSource.Sections.Count &&
                indexPath.Row < dataSource.Sections[indexPath.Section].Items.Count)
            {
                var item = dataSource.Sections[indexPath.Section].Items[indexPath.Row];
                control.RaiseItemSelected(item, indexPath.Row);
            }
        }

        public override void CellDisplayingEnded(UICollectionView collectionView,
            UICollectionViewCell cell, NSIndexPath indexPath)
        {
            if (indexPath.Section < dataSource.Sections.Count &&
                indexPath.Row < dataSource.Sections[indexPath.Section].Items.Count)
            {
                var item = dataSource.Sections[indexPath.Section].Items[indexPath.Row];
                control.RaiseItemHidden(item, indexPath.Row);
            }
        }

        public override void WillDisplayCell(UICollectionView collectionView,
            UICollectionViewCell cell, NSIndexPath indexPath)
        {
            if (control.ShowLoadMoreButton || isLoadingMore)
                return;

            var section = dataSource.Sections;
            var totalItems = section.Sum(s => s.Items.Count);
            var currentIndex = 0;
            for (var i = 0; i < indexPath.Section; i++)
                currentIndex += section[i].Items.Count;
            currentIndex += indexPath.Row;

            if (currentIndex >= totalItems - control.LoadMoreThreshold)
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

}

internal class MauiSupplementaryView : UICollectionReusableView
{
    public static readonly NSString ReuseId = new("MauiSupplementaryView");

    IMauiContext? mauiContext;
    View? mauiView;
    UIView? platformView;

    [Export("initWithFrame:")]
    public MauiSupplementaryView(CGRect frame) : base(frame)
    {
    }

    public void Bind(VirtualizedGrid control, IMauiContext context, object groupKey)
    {
        mauiContext = context;

        if (mauiView is null)
        {
            mauiView = control.CreateGroupHeaderView(groupKey);
            platformView = mauiView.ToPlatform(mauiContext);
            AddSubview(platformView);
        }
        else
        {
            mauiView.BindingContext = groupKey;
        }

        SetNeedsLayout();
    }

    public void BindLoadMore(VirtualizedGrid control, IMauiContext context)
    {
        mauiContext = context;

        if (mauiView is null)
        {
            mauiView = control.CreateLoadMoreView();
            platformView = mauiView.ToPlatform(mauiContext);
            AddSubview(platformView);
        }

        SetNeedsLayout();
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        if (platformView is not null)
            platformView.Frame = Bounds;
    }

    public override void PrepareForReuse()
    {
        base.PrepareForReuse();
        if (mauiView is not null)
            mauiView.BindingContext = null;
    }
}
#endif
