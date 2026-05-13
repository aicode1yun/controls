using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace Sample.Features.VirtualizedGrid;

[ShellMap<VirtualizedGridPage>(registerRoute: false)]
public partial class VirtualizedGridViewModel : ObservableObject
{
    static readonly string[] Colors =
    {
        "#E53E3E","#DD6B20","#38A169","#3182CE","#805AD5","#D53F8C",
        "#319795","#B7791F","#0891B2","#9D174D","#0F766E","#7C3AED",
        "#0284C7","#16A34A","#CA8A04","#1D4ED8","#BE123C","#15803D"
    };
    static readonly string[] Icons =
    {
        "\U0001f3af","⚡","\U0001f31f","\U0001f525","\U0001f48e","\U0001f3aa",
        "\U0001f308","\U0001f3b8","\U0001f50d","\U0001f4cc","\U0001f9ed","\U0001f3a8",
        "\U0001f3a7","\U0001f3ac","\U0001f3a5","\U0001f3b2","\U0001f4c8","\U0001f4d6"
    };

    [ObservableProperty]
    int columnCount = 3;

    [ObservableProperty]
    string statusMessage = "Tap a cell to select it";

    [ObservableProperty]
    DisplayMode mode = DisplayMode.Flat;

    [ObservableProperty]
    bool canLoadMore = true;

    [ObservableProperty]
    bool useLoadMoreButton = true;

    public ObservableCollection<GridItem> FlatItems { get; } = new(
        Enumerable.Range(1, 240).Select(i => new GridItem(
            $"Item {i}",
            Colors[(i - 1) % Colors.Length],
            Icons[(i - 1) % Icons.Length]
        ))
    );

    public ObservableCollection<GroupedGridItems> GroupedSource { get; } = new(CreateGroupedItems());

    public ObservableCollection<GridItem> LoadMoreItems { get; } = new(
        Enumerable.Range(1, 24).Select(i => new GridItem(
            $"Item {i}",
            Colors[(i - 1) % Colors.Length],
            Icons[(i - 1) % Icons.Length]
        ))
    );

    public bool IsFlatVisible => Mode == DisplayMode.Flat;
    public bool IsGroupedVisible => Mode == DisplayMode.Grouped;
    public bool IsLoadMoreVisible => Mode == DisplayMode.LoadMore;

    partial void OnModeChanged(DisplayMode value)
    {
        OnPropertyChanged(nameof(IsFlatVisible));
        OnPropertyChanged(nameof(IsGroupedVisible));
        OnPropertyChanged(nameof(IsLoadMoreVisible));
    }

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

    [RelayCommand] void ShowFlat() => Mode = DisplayMode.Flat;
    [RelayCommand] void ShowGrouped() => Mode = DisplayMode.Grouped;
    [RelayCommand] void ShowLoadMore() => Mode = DisplayMode.LoadMore;

    public string LoadMoreModeLabel => UseLoadMoreButton ? "Using: Button" : "Using: Threshold (8)";

    partial void OnUseLoadMoreButtonChanged(bool value)
    {
        OnPropertyChanged(nameof(LoadMoreModeLabel));
    }

    [RelayCommand]
    void UseButtonMode()
    {
        UseLoadMoreButton = true;
        StatusMessage = "Load More: button at end of list";
    }

    [RelayCommand]
    void UseThresholdMode()
    {
        UseLoadMoreButton = false;
        StatusMessage = "Load More: auto-loads when 8 items from end";
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
        await Task.Delay(600);
        var start = LoadMoreItems.Count + 1;
        for (var i = start; i < start + 12; i++)
        {
            LoadMoreItems.Add(new GridItem(
                $"Item {i}",
                Colors[(i - 1) % Colors.Length],
                Icons[(i - 1) % Icons.Length]
            ));
        }

        if (LoadMoreItems.Count >= 96)
            CanLoadMore = false;
    }

    static IEnumerable<GroupedGridItems> CreateGroupedItems()
    {
        var fruits = new[] { "Apple","Banana","Cherry","Grape","Kiwi","Mango","Peach","Pear","Plum","Berry","Lemon","Orange" };
        var fruitIcons = new[] { "\U0001f34e","\U0001f34c","\U0001f352","\U0001f347","\U0001f95d","\U0001f96d","\U0001f351","\U0001f350","\U0001f33f","\U0001f347","\U0001f34b","\U0001f34a" };

        var veggies = new[] { "Carrot","Broccoli","Corn","Pepper","Tomato","Onion","Garlic","Potato","Lettuce","Cucumber","Eggplant","Beet" };
        var veggieIcons = new[] { "\U0001f955","\U0001f966","\U0001f33d","\U0001f336","\U0001f345","\U0001f9c5","\U0001f9c4","\U0001f954","\U0001f96c","\U0001f952","\U0001f346","\U0001f9c4" };

        var snacks = new[] { "Cookie","Donut","Popcorn","Candy","Chocolate","Pretzel","Cracker","Cake","Pie","IceCream","Muffin","Cupcake" };
        var snackIcons = new[] { "\U0001f36a","\U0001f369","\U0001f37f","\U0001f36c","\U0001f36b","\U0001f968","\U0001f9c1","\U0001f370","\U0001f967","\U0001f366","\U0001f9c1","\U0001f9c1" };

        var drinks = new[] { "Coffee","Tea","Juice","Soda","Water","Smoothie","Cocoa","Milk","Beer","Wine","Whiskey","Cider" };
        var drinkIcons = new[] { "☕","\U0001f375","\U0001f9c3","\U0001f964","\U0001f4a7","\U0001f964","☕","\U0001f95b","\U0001f37a","\U0001f377","\U0001f943","\U0001f37a" };

        yield return new GroupedGridItems("Fruits",
            fruits.Select((n, i) => new GridItem(n, Colors[i % Colors.Length], fruitIcons[i])));
        yield return new GroupedGridItems("Vegetables",
            veggies.Select((n, i) => new GridItem(n, Colors[(i + 3) % Colors.Length], veggieIcons[i])));
        yield return new GroupedGridItems("Snacks",
            snacks.Select((n, i) => new GridItem(n, Colors[(i + 6) % Colors.Length], snackIcons[i])));
        yield return new GroupedGridItems("Drinks",
            drinks.Select((n, i) => new GridItem(n, Colors[(i + 9) % Colors.Length], drinkIcons[i])));
    }
}

public enum DisplayMode
{
    Flat,
    Grouped,
    LoadMore
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

// IGrouping<out TKey, out TElement> is covariant, so this satisfies
// IGrouping<object, object> which VirtualizedGrid looks for when grouping is enabled.
public class GroupedGridItems : List<GridItem>, IGrouping<string, GridItem>
{
    public GroupedGridItems(string key, IEnumerable<GridItem> items) : base(items)
    {
        Key = key;
    }

    public string Key { get; }

    public override string ToString() => Key;
}
