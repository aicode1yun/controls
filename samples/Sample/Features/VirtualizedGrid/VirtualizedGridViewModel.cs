using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace Sample.Features.VirtualizedGrid;

[ShellMap<VirtualizedGridPage>(registerRoute: false)]
public partial class VirtualizedGridViewModel : ObservableObject
{
    static readonly string[] Colors = { "#E53E3E", "#DD6B20", "#38A169", "#3182CE", "#805AD5", "#D53F8C", "#319795", "#B7791F" };
    static readonly string[] Icons = { "\U0001f3af", "\u26a1", "\U0001f31f", "\U0001f525", "\U0001f48e", "\U0001f3aa", "\U0001f308", "\U0001f3b8" };

    [ObservableProperty]
    int columnCount = 3;

    [ObservableProperty]
    string statusMessage = "Tap a cell to select it";

    [ObservableProperty]
    bool isFlatVisible = true;

    [ObservableProperty]
    bool isGroupedVisible;

    [ObservableProperty]
    bool isLoadMoreVisible;

    [ObservableProperty]
    bool canLoadMore = true;

    public ObservableCollection<GridItem> FlatItems { get; } = new(
        Enumerable.Range(1, 12).Select(i => new GridItem(
            $"Item {i}",
            Colors[(i - 1) % Colors.Length],
            Icons[(i - 1) % Icons.Length]
        ))
    );

    public ObservableCollection<GridItem> GroupedSource { get; } = new(
        CreateGroupedItems()
    );

    public ObservableCollection<GridItem> LoadMoreItems { get; } = new(
        Enumerable.Range(1, 9).Select(i => new GridItem(
            $"Item {i}",
            Colors[(i - 1) % Colors.Length],
            Icons[(i - 1) % Icons.Length]
        ))
    );

    [RelayCommand]
    void IncrementColumns()
    {
        if (ColumnCount < 6) ColumnCount++;
    }

    [RelayCommand]
    void DecrementColumns()
    {
        if (ColumnCount > 1) ColumnCount--;
    }

    [RelayCommand]
    void ShowFlat()
    {
        IsFlatVisible = true;
        IsGroupedVisible = false;
        IsLoadMoreVisible = false;
    }

    [RelayCommand]
    void ShowGrouped()
    {
        IsFlatVisible = false;
        IsGroupedVisible = true;
        IsLoadMoreVisible = false;
    }

    [RelayCommand]
    void ShowLoadMore()
    {
        IsFlatVisible = false;
        IsGroupedVisible = false;
        IsLoadMoreVisible = true;
    }

    [RelayCommand]
    void ItemSelected(object item)
    {
        if (item is GridItem gi)
            StatusMessage = $"Selected: {gi.Name}";
    }

    [RelayCommand]
    async Task LoadMore()
    {
        await Task.Delay(800);
        var start = LoadMoreItems.Count + 1;
        for (var i = start; i < start + 6; i++)
        {
            LoadMoreItems.Add(new GridItem(
                $"Item {i}",
                Colors[(i - 1) % Colors.Length],
                Icons[(i - 1) % Icons.Length]
            ));
        }

        if (LoadMoreItems.Count >= 30)
            CanLoadMore = false;
    }

    static IEnumerable<GridItem> CreateGroupedItems()
    {
        // Flat list — grouping is handled by the control via IsGroupingEnabled + GroupHeaderTemplate
        var fruits = new[]
        {
            new GridItem("Apple", "#E53E3E", "\U0001f34e"),
            new GridItem("Banana", "#DD6B20", "\U0001f34c"),
            new GridItem("Cherry", "#C53030", "\U0001f352"),
            new GridItem("Grape", "#805AD5", "\U0001f347"),
            new GridItem("Kiwi", "#38A169", "\U0001f95d"),
            new GridItem("Mango", "#D69E2E", "\U0001f96d"),
        };
        var veggies = new[]
        {
            new GridItem("Carrot", "#ED8936", "\U0001f955"),
            new GridItem("Broccoli", "#48BB78", "\U0001f966"),
            new GridItem("Corn", "#ECC94B", "\U0001f33d"),
            new GridItem("Pepper", "#FC8181", "\U0001f336\ufe0f"),
            new GridItem("Tomato", "#F56565", "\U0001f345"),
        };
        var snacks = new[]
        {
            new GridItem("Cookie", "#B7791F", "\U0001f36a"),
            new GridItem("Donut", "#D53F8C", "\U0001f369"),
            new GridItem("Popcorn", "#F6E05E", "\U0001f37f"),
            new GridItem("Candy", "#F687B3", "\U0001f36c"),
        };

        return fruits.Concat(veggies).Concat(snacks);
    }
}

public class GridItem
{
    public GridItem(string name, string color, string icon)
    {
        Name = name;
        Color = Microsoft.Maui.Graphics.Color.Parse(color);
        Icon = icon;
    }

    public string Name { get; }
    public Color Color { get; }
    public string Icon { get; }
}
