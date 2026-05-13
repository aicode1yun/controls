using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace Sample.Features.StaggeredGrid;

[ShellMap<StaggeredGridPage>(registerRoute: false)]
public partial class StaggeredGridViewModel : ObservableObject
{
    [ObservableProperty]
    int columnCount = 2;

    [ObservableProperty]
    string statusMessage = "Tap a pin to select it";

    public ObservableCollection<StaggeredItem> Items { get; } = new(StaggeredItem.GenerateSeed());

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
    void Shuffle()
    {
        var shuffled = Items.OrderBy(_ => Guid.NewGuid()).ToList();
        Items.Clear();
        foreach (var i in shuffled)
            Items.Add(i);
    }

    [RelayCommand]
    void ItemSelected(object item)
    {
        if (item is StaggeredItem si)
            StatusMessage = $"Selected: {si.Title} ({si.Category})";
    }
}

public class StaggeredItem
{
    static readonly (string Title, string Category, string Accent, string Description)[] Catalog =
    {
        ("Alpine Sunrise", "Mountains", "#0F766E", "Golden light breaks over snow-capped peaks, painting the valley in amber hues."),
        ("Desert Dunes", "Travel", "#D97706", "Endless waves of sand."),
        ("Forest Trail", "Nature", "#15803D", "A winding path through ancient oaks and ferns, dappled sunlight filtering through the canopy. The air smells of moss and earth after rain."),
        ("Neon Tokyo", "Urban", "#DB2777", "Electric signs pulse."),
        ("Ocean Calm", "Beach", "#0284C7", "Crystal blue water stretches to the horizon. The gentle sound of waves lapping against the shore."),
        ("Cozy Cabin", "Home", "#92400E", "Warm firelight."),
        ("Modern Loft", "Interior", "#475569", "Open-plan living with exposed brick walls, concrete floors, and floor-to-ceiling windows overlooking the city skyline. A curated collection of mid-century modern furniture."),
        ("Wildflowers", "Garden", "#A21CAF", "Purple, yellow, and white blooms swaying in the breeze."),
        ("Vintage Camera", "Photography", "#1E293B", "A well-worn Leica M3, its brass showing through years of use."),
        ("Espresso", "Coffee", "#78350F", "Rich crema."),
        ("Plated Sushi", "Food", "#BE123C", "Artfully arranged nigiri on a handmade ceramic plate. Each piece a tiny sculpture of rice and the freshest fish from Tsukiji market."),
        ("Pasta Night", "Recipes", "#B45309", "Handmade tagliatelle."),
        ("Bookshelf", "Reading", "#7C2D12", "Floor-to-ceiling shelves packed with well-loved paperbacks, leather-bound classics, and a few forgotten bookmarks peeking out. A reading lamp casts a warm circle of light on the worn armchair below."),
        ("Workspace", "Productivity", "#0F172A", "Clean desk, clear mind."),
        ("Iceberg", "Polar", "#0E7490", "Only a fraction visible above the waterline, the massive ice structure glows an ethereal blue beneath the Arctic surface."),
        ("Sahara", "Desert", "#CA8A04", "Heat shimmer."),
        ("Rainforest", "Jungle", "#166534", "Layers upon layers of green — ferns, vines, palms, and towering emergent trees reaching for sunlight. Parrots call from the canopy."),
        ("Skyline", "Architecture", "#1E40AF", "Glass and steel."),
        ("Surf", "Watersports", "#0891B2", "A perfect barrel wave, sunlight refracting through the curl. Salt spray catches the wind."),
        ("Snowboard", "Winter", "#1D4ED8", "Fresh powder day."),
        ("Marathon", "Fitness", "#B91C1C", "Mile 20. The wall. Every step is a negotiation between body and will, but the finish line crowd roars ahead."),
        ("Pottery", "Crafts", "#9A3412", "Wet clay spinning on the wheel, hands shaping something from nothing."),
        ("Watercolor", "Art", "#7C3AED", "Pigments bloom and bleed across wet paper, each painting unique and impossible to fully control. That's the beauty of the medium."),
        ("Vinyl Records", "Music", "#831843", "Warm analog sound."),
        ("Latte Art", "Cafe", "#A16207", "A perfect rosetta poured into a ceramic cup. The milk foam holds its shape as steam rises."),
        ("Pancakes", "Brunch", "#C2410C", "Stacked high with maple syrup and fresh berries."),
        ("Charcuterie", "Wine", "#7F1D1D", "A board of aged cheeses, paper-thin prosciutto, cornichons, honeycomb, and crusty sourdough. Paired with a bold Barolo."),
        ("Garden Path", "Outdoor", "#3F6212", "Stepping stones."),
        ("Hot Spring", "Wellness", "#0369A1", "Mineral-rich waters, surrounded by smooth volcanic rock. Steam curls upward into the cold mountain air."),
        ("Aurora", "Sky", "#4338CA", "Green and purple light dancing across the Arctic sky, reflected in the still lake below. A phenomenon that never gets old no matter how many times you see it."),
        ("Cherry Blossom", "Spring", "#EC4899", "Pink petals drift like snow."),
        ("Autumn Leaves", "Fall", "#C2410C", "Crimson, amber, and gold carpet the forest floor."),
        ("Lighthouse", "Coast", "#1E40AF", "Standing sentinel against Atlantic storms for over a century, its beam still sweeps the dark waters every twelve seconds. The keeper's cottage is long abandoned."),
        ("Canyon", "Adventure", "#A16207", "Carved by millennia."),
        ("Vintage Bike", "Lifestyle", "#374151", "A 1970s steel-frame road bike, restored with leather bar tape and a Brooks saddle. Still the smoothest ride on Sunday morning back roads."),
        ("Boutique", "Fashion", "#9D174D", "Curated style."),
        ("Sneakers", "Style", "#1F2937", "Limited edition. Only 500 pairs made."),
        ("Cocktail", "Bar", "#86198F", "An old fashioned, whiskey-forward with a Luxardo cherry and a wide orange peel. The ice is a single clear cube, hand-cut. The kind of drink you nurse slowly while jazz plays."),
        ("Stadium", "Sports", "#15803D", "The roar of 80,000 fans."),
        ("Camp Fire", "Camping", "#9A3412", "Crackling flames under a blanket of stars, the smell of wood smoke and toasting marshmallows. Someone strums a guitar. The simplest kind of happiness."),
    };

    // Heights chosen to vary in 4 buckets to drive nice staggering across columns.
    static readonly double[] HeightBuckets = { 180, 240, 300, 380 };

    public StaggeredItem(int seed, string title, string category, double height, string accent, string description)
    {
        Seed = seed;
        Title = title;
        Category = category;
        Height = height;
        Accent = Microsoft.Maui.Graphics.Color.Parse(accent);
        Description = description;
        ImageUrl = $"https://picsum.photos/seed/shiny{seed}/400/{(int)height}";
    }

    public int Seed { get; }
    public string Title { get; }
    public string Category { get; }
    public double Height { get; }
    public Color Accent { get; }
    public string ImageUrl { get; }
    public string Description { get; }

    public static IEnumerable<StaggeredItem> GenerateSeed()
    {
        for (var i = 0; i < Catalog.Length; i++)
        {
            var (title, category, accent, description) = Catalog[i];
            var height = HeightBuckets[i % HeightBuckets.Length];
            height += (i % 7) * 12;
            yield return new StaggeredItem(i + 1, title, category, height, accent, description);
        }
    }
}
