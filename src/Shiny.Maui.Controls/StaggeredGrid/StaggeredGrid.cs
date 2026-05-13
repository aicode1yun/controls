using Shiny.Maui.Controls.Collections;

namespace Shiny.Maui.Controls.StaggeredGrid;

public class StaggeredGrid : CollectionControlBase
{
    public static readonly BindableProperty ColumnCountProperty = BindableProperty.Create(
        nameof(ColumnCount),
        typeof(int),
        typeof(StaggeredGrid),
        2,
        validateValue: (_, v) => (int)v >= 1);

    public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(
        nameof(RowSpacing),
        typeof(double),
        typeof(StaggeredGrid),
        0.0);

    public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(
        nameof(ColumnSpacing),
        typeof(double),
        typeof(StaggeredGrid),
        0.0);

    public int ColumnCount
    {
        get => (int)GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    public double RowSpacing
    {
        get => (double)GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    public double ColumnSpacing
    {
        get => (double)GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }
}
