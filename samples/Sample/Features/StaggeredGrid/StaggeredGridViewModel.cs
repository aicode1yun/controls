using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace Sample.Features.StaggeredGrid;

[ShellMap<StaggeredGridPage>(registerRoute: false)]
public partial class StaggeredGridViewModel : ObservableObject
{
    [ObservableProperty]
    int columnCount = 3;

    [ObservableProperty]
    string statusMessage = "Tap a card to select it";

    public ObservableCollection<StaggeredItem> Items { get; } = new(new[]
    {
        new StaggeredItem("Travel", "Adventure", 180, "#E53E3E", "\u2708\ufe0f"),
        new StaggeredItem("Food", "Recipes", 240, "#DD6B20", "\U0001f355"),
        new StaggeredItem("Nature", "Photography", 160, "#38A169", "\U0001f33f"),
        new StaggeredItem("Tech", "Gadgets", 200, "#3182CE", "\U0001f4bb"),
        new StaggeredItem("Art", "Design", 280, "#805AD5", "\U0001f3a8"),
        new StaggeredItem("Music", "Playlists", 150, "#D53F8C", "\U0001f3b5"),
        new StaggeredItem("Fitness", "Workouts", 220, "#319795", "\U0001f4aa"),
        new StaggeredItem("Books", "Reading", 190, "#B7791F", "\U0001f4da"),
        new StaggeredItem("Fashion", "Style", 260, "#C53030", "\U0001f457"),
        new StaggeredItem("Gaming", "Reviews", 170, "#2B6CB0", "\U0001f3ae"),
        new StaggeredItem("DIY", "Crafts", 210, "#2F855A", "\U0001f528"),
        new StaggeredItem("Pets", "Animals", 250, "#6B46C1", "\U0001f43e"),
    });

    [RelayCommand]
    void IncrementColumns()
    {
        if (ColumnCount < 5) ColumnCount++;
    }

    [RelayCommand]
    void DecrementColumns()
    {
        if (ColumnCount > 1) ColumnCount--;
    }

    [RelayCommand]
    void ItemSelected(object item)
    {
        if (item is StaggeredItem si)
            StatusMessage = $"Selected: {si.Title}";
    }
}

public class StaggeredItem
{
    public StaggeredItem(string title, string category, double height, string color, string icon)
    {
        Title = title;
        Category = category;
        Height = height;
        Color = Microsoft.Maui.Graphics.Color.Parse(color);
        Icon = icon;
    }

    public string Title { get; }
    public string Category { get; }
    public double Height { get; }
    public Color Color { get; }
    public string Icon { get; }
}
