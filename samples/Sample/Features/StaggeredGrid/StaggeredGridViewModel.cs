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
    static readonly (string Title, string Category, string Accent)[] Catalog =
    {
        ("Alpine Sunrise", "Mountains", "#0F766E"),
        ("Desert Dunes", "Travel", "#D97706"),
        ("Forest Trail", "Nature", "#15803D"),
        ("Neon Tokyo", "Urban", "#DB2777"),
        ("Ocean Calm", "Beach", "#0284C7"),
        ("Cozy Cabin", "Home", "#92400E"),
        ("Modern Loft", "Interior", "#475569"),
        ("Wildflowers", "Garden", "#A21CAF"),
        ("Vintage Camera", "Photography", "#1E293B"),
        ("Espresso", "Coffee", "#78350F"),
        ("Plated Sushi", "Food", "#BE123C"),
        ("Pasta Night", "Recipes", "#B45309"),
        ("Bookshelf", "Reading", "#7C2D12"),
        ("Workspace", "Productivity", "#0F172A"),
        ("Iceberg", "Polar", "#0E7490"),
        ("Sahara", "Desert", "#CA8A04"),
        ("Rainforest", "Jungle", "#166534"),
        ("Skyline", "Architecture", "#1E40AF"),
        ("Surf", "Watersports", "#0891B2"),
        ("Snowboard", "Winter", "#1D4ED8"),
        ("Marathon", "Fitness", "#B91C1C"),
        ("Pottery", "Crafts", "#9A3412"),
        ("Watercolor", "Art", "#7C3AED"),
        ("Vinyl Records", "Music", "#831843"),
        ("Latte Art", "Cafe", "#A16207"),
        ("Pancakes", "Brunch", "#C2410C"),
        ("Charcuterie", "Wine", "#7F1D1D"),
        ("Garden Path", "Outdoor", "#3F6212"),
        ("Hot Spring", "Wellness", "#0369A1"),
        ("Aurora", "Sky", "#4338CA"),
        ("Cherry Blossom", "Spring", "#EC4899"),
        ("Autumn Leaves", "Fall", "#C2410C"),
        ("Lighthouse", "Coast", "#1E40AF"),
        ("Canyon", "Adventure", "#A16207"),
        ("Vintage Bike", "Lifestyle", "#374151"),
        ("Boutique", "Fashion", "#9D174D"),
        ("Sneakers", "Style", "#1F2937"),
        ("Cocktail", "Bar", "#86198F"),
        ("Stadium", "Sports", "#15803D"),
        ("Camp Fire", "Camping", "#9A3412"),
        ("Sailboat", "Sailing", "#1D4ED8"),
        ("Vineyard", "Wine", "#7E22CE"),
        ("Tea House", "Asia", "#065F46"),
        ("Lavender Fields", "France", "#7C3AED"),
        ("Greek Island", "Travel", "#0284C7"),
        ("Northern Lights", "Aurora", "#1E40AF"),
        ("Modern Kitchen", "Design", "#0F172A"),
        ("Macarons", "Dessert", "#DB2777"),
        ("Pizza", "Italian", "#B91C1C"),
        ("Ramen", "Japanese", "#7C2D12"),
        ("Yoga Studio", "Wellness", "#7C3AED"),
        ("Bouquet", "Florist", "#BE185D"),
        ("Studio", "Music", "#1E293B"),
        ("Gaming Setup", "Tech", "#4338CA"),
        ("Vintage Car", "Auto", "#991B1B"),
        ("Skyscraper", "City", "#0F172A"),
        ("Park Bench", "Park", "#166534"),
        ("Antique", "Collectibles", "#78350F"),
        ("Plants", "Plants", "#16A34A"),
        ("Snow Peak", "Skiing", "#1E40AF"),
        ("Festival", "Music", "#A21CAF"),
        ("Cathedral", "Heritage", "#3F3F46"),
        ("Train Station", "Travel", "#374151"),
        ("Spa", "Beauty", "#9D174D"),
        ("Cycling", "Outdoors", "#16A34A"),
        ("Climbing", "Adventure", "#9A3412"),
        ("Reading Nook", "Comfort", "#7E22CE"),
        ("Bakery", "Bread", "#B45309"),
        ("Brunch Plate", "Food", "#DC2626"),
        ("Studio Lights", "Art", "#1F2937"),
        ("Atelier", "Design", "#9D174D"),
        ("Mountain Lake", "Reflection", "#0E7490"),
        ("Coastline", "Cliffs", "#1D4ED8"),
        ("Pier", "Sea", "#0369A1"),
        ("Botanical", "Plants", "#15803D"),
        ("Wood Workshop", "Craft", "#7C2D12"),
        ("Ceramics", "Pottery", "#9A3412"),
        ("Sketchbook", "Drawing", "#374151"),
        ("Linen", "Textile", "#525B7A"),
        ("Tile Floor", "Pattern", "#0F766E"),
        ("Marble", "Texture", "#475569"),
        ("Brass", "Materials", "#A16207"),
        ("Garden Tools", "DIY", "#15803D")
    };

    // Heights chosen to vary in 4 buckets to drive nice staggering across columns.
    static readonly double[] HeightBuckets = { 220, 280, 340, 420 };

    public StaggeredItem(int seed, string title, string category, double height, string accent)
    {
        Seed = seed;
        Title = title;
        Category = category;
        Height = height;
        Accent = Microsoft.Maui.Graphics.Color.Parse(accent);
        // Picsum returns a real image with width=400 and the height we want.
        ImageUrl = $"https://picsum.photos/seed/shiny{seed}/400/{(int)height}";
    }

    public int Seed { get; }
    public string Title { get; }
    public string Category { get; }
    public double Height { get; }
    public Color Accent { get; }
    public string ImageUrl { get; }

    public static IEnumerable<StaggeredItem> GenerateSeed()
    {
        for (var i = 0; i < Catalog.Length; i++)
        {
            var (title, category, accent) = Catalog[i];
            var height = HeightBuckets[i % HeightBuckets.Length];
            // Add some variation to avoid repeating identical heights row after row.
            height += (i % 7) * 8;
            yield return new StaggeredItem(i + 1, title, category, height, accent);
        }
    }
}
