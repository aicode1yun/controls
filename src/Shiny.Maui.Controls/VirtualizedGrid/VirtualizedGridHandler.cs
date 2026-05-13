#if ANDROID || IOS || MACCATALYST || WINDOWS
using Microsoft.Maui.Handlers;

namespace Shiny.Maui.Controls.VirtualizedGrid;

public partial class VirtualizedGridHandler
{
    public static IPropertyMapper<VirtualizedGrid, VirtualizedGridHandler> Mapper =
        new PropertyMapper<VirtualizedGrid, VirtualizedGridHandler>(ViewHandler.ViewMapper)
        {
            [nameof(VirtualizedGrid.ItemsSource)] = MapItemsSource,
            [nameof(VirtualizedGrid.ColumnCount)] = MapColumnCount,
            [nameof(VirtualizedGrid.PortraitColumnCount)] = MapColumnCount,
            [nameof(VirtualizedGrid.LandscapeColumnCount)] = MapColumnCount,
            [nameof(VirtualizedGrid.IsGroupingEnabled)] = MapGrouping,
            [nameof(VirtualizedGrid.GroupHeaderTemplate)] = MapGrouping,
            [nameof(VirtualizedGrid.HasStickyHeaders)] = MapStickyHeaders,
            [nameof(VirtualizedGrid.CellPadding)] = MapCellPadding,
            [nameof(VirtualizedGrid.ItemSpacing)] = MapCellPadding,
            [nameof(VirtualizedGrid.ItemTemplate)] = MapItemTemplate,
            [nameof(VirtualizedGrid.ItemTemplateSelector)] = MapItemTemplate,
            [nameof(VirtualizedGrid.ShowLoadMoreButton)] = MapLoadMore,
            [nameof(VirtualizedGrid.LoadMoreButtonTemplate)] = MapLoadMore,
        };

    public static CommandMapper<VirtualizedGrid, VirtualizedGridHandler> CommandMapper =
        new(ViewHandler.ViewCommandMapper);

    public VirtualizedGridHandler() : base(Mapper, CommandMapper)
    {
    }

    static partial void MapItemsSource(VirtualizedGridHandler handler, VirtualizedGrid view);
    static partial void MapColumnCount(VirtualizedGridHandler handler, VirtualizedGrid view);
    static partial void MapGrouping(VirtualizedGridHandler handler, VirtualizedGrid view);
    static partial void MapStickyHeaders(VirtualizedGridHandler handler, VirtualizedGrid view);
    static partial void MapCellPadding(VirtualizedGridHandler handler, VirtualizedGrid view);
    static partial void MapItemTemplate(VirtualizedGridHandler handler, VirtualizedGrid view);
    static partial void MapLoadMore(VirtualizedGridHandler handler, VirtualizedGrid view);
}
#endif
