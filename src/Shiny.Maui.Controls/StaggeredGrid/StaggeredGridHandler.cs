#if ANDROID || IOS || MACCATALYST || WINDOWS
using Microsoft.Maui.Handlers;
using Shiny.Maui.Controls.Collections;

namespace Shiny.Maui.Controls.StaggeredGrid;

public partial class StaggeredGridHandler
{
    public static IPropertyMapper<StaggeredGrid, StaggeredGridHandler> Mapper =
        new PropertyMapper<StaggeredGrid, StaggeredGridHandler>(ViewHandler.ViewMapper)
        {
            [nameof(StaggeredGrid.ItemsSource)] = MapItemsSource,
            [nameof(StaggeredGrid.ColumnCount)] = MapColumnCount,
            [nameof(StaggeredGrid.RowSpacing)] = MapSpacing,
            [nameof(StaggeredGrid.ColumnSpacing)] = MapSpacing,
            [nameof(StaggeredGrid.ItemTemplate)] = MapItemTemplate,
            [nameof(StaggeredGrid.ItemTemplateSelector)] = MapItemTemplate,
        };

    public static CommandMapper<StaggeredGrid, StaggeredGridHandler> CommandMapper =
        new(ViewHandler.ViewCommandMapper);

    public StaggeredGridHandler() : base(Mapper, CommandMapper)
    {
    }

    static partial void MapItemsSource(StaggeredGridHandler handler, StaggeredGrid view);
    static partial void MapColumnCount(StaggeredGridHandler handler, StaggeredGrid view);
    static partial void MapSpacing(StaggeredGridHandler handler, StaggeredGrid view);
    static partial void MapItemTemplate(StaggeredGridHandler handler, StaggeredGrid view);
}
#endif
