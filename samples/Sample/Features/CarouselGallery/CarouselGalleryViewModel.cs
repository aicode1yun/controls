using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;

namespace Sample.Features.CarouselGallery;

[ShellMap<CarouselGalleryPage>(registerRoute: false)]
public partial class CarouselGalleryViewModel : ObservableObject
{
    [ObservableProperty]
    int currentPosition;

    [ObservableProperty]
    string statusMessage = "Swipe horizontally — tap a slide to select it";

    public ObservableCollection<CarouselItem> BasicItems { get; } = new(BuildGenres());
    public ObservableCollection<CarouselItem> GradientItems { get; } = new(BuildDestinations());
    public ObservableCollection<CarouselItem> PhotoItems { get; } = new(BuildPhotos());

    [RelayCommand]
    void ItemSelected(object item)
    {
        if (item is CarouselItem ci)
            StatusMessage = $"Selected: {ci.Title}";
    }

    static IEnumerable<CarouselItem> BuildGenres()
    {
        var genres = new (string Title, string Color, string Icon)[]
        {
            ("Action",     "#E53E3E", "\U0001f3ac"),
            ("Comedy",     "#DD6B20", "\U0001f602"),
            ("Drama",      "#38A169", "\U0001f3ad"),
            ("Sci-Fi",     "#3182CE", "\U0001f680"),
            ("Horror",     "#805AD5", "\U0001f47b"),
            ("Romance",    "#D53F8C", "❤️"),
            ("Thriller",   "#1F2937", "\U0001f5e1"),
            ("Mystery",    "#4338CA", "\U0001f50d"),
            ("Fantasy",    "#7C3AED", "\U0001f9d9"),
            ("Western",    "#A16207", "\U0001f3a9"),
            ("Animation",  "#0891B2", "\U0001f3a8"),
            ("Musical",    "#BE185D", "\U0001f3b6"),
            ("Adventure",  "#15803D", "\U0001f5fa️"),
            ("Crime",      "#374151", "\U0001f52b"),
            ("Documentary","#0F766E", "\U0001f4f9"),
            ("Family",     "#16A34A", "\U0001f46a"),
            ("History",    "#92400E", "\U0001f4dc"),
            ("War",        "#7F1D1D", "⚔️"),
            ("Biography",  "#0369A1", "\U0001f464"),
            ("Sport",      "#16A34A", "⚽")
        };
        return genres.Select(g => new CarouselItem(g.Title, g.Color, g.Icon));
    }

    static IEnumerable<CarouselItem> BuildDestinations()
    {
        var destinations = new (string Title, string Color, string Icon, string Subtitle)[]
        {
            ("Mountains",   "#2D3748", "\U0001f3d4️", "Explore the peaks"),
            ("Ocean",       "#2B6CB0", "\U0001f30a",       "Deep blue sea"),
            ("Forest",      "#276749", "\U0001f332",       "Into the wild"),
            ("Desert",      "#C05621", "\U0001f3dc️", "Golden sands"),
            ("City",        "#553C9A", "\U0001f303",       "Urban nights"),
            ("Tropics",     "#15803D", "\U0001f334",       "Paradise found"),
            ("Arctic",      "#1E40AF", "\U0001f9ca",       "Frozen vistas"),
            ("Volcano",     "#7F1D1D", "\U0001f30b",       "Liquid fire"),
            ("Canyon",      "#9A3412", "\U0001f3de️", "Carved in time"),
            ("Reef",        "#0E7490", "\U0001f41f",       "Coral gardens"),
            ("Glacier",     "#0369A1", "❄️",          "Ancient ice"),
            ("Savannah",    "#A16207", "\U0001f981",       "Roaming herds"),
            ("Highlands",   "#166534", "\U0001f409",       "Misty hills"),
            ("Lagoon",      "#0891B2", "\U0001f3dd️", "Calm waters"),
            ("Outback",     "#B45309", "\U0001f998",       "Endless plains"),
            ("Tundra",      "#3B82F6", "\U0001f43a",       "Cold wilderness")
        };
        return destinations.Select(d => new CarouselItem(d.Title, d.Color, d.Icon, d.Subtitle));
    }

    static IEnumerable<CarouselItem> BuildPhotos()
    {
        var captions = new[]
        {
            "Golden Hour", "Misty Morning", "City Skyline", "Quiet Street",
            "Wave Break",  "Forest Light",  "Stone Cliffs", "Sunset Pier",
            "Snowy Pine",  "Coffee Shop",   "Old Library",  "Vinyl Wall",
            "Train Yard",  "Studio Set",    "Rooftop View", "Calm Lake",
            "Stone Bridge","Lavender Row",  "Brick Alley",  "Brass Lamp",
            "Open Road",   "Quiet Harbor",  "Iron Gate",    "Hidden Door"
        };
        for (var i = 0; i < captions.Length; i++)
        {
            yield return new CarouselPhotoItem(
                captions[i],
                $"https://picsum.photos/seed/carousel{i + 1}/600/400",
                $"#{(i * 25 % 256):X2}{(i * 47 % 256):X2}{(i * 73 % 256):X2}"
            );
        }
    }
}

public class CarouselItem
{
    public CarouselItem(string title, string color, string icon, string? subtitle = null)
    {
        Title = title;
        Color = Microsoft.Maui.Graphics.Color.Parse(color);
        Icon = icon;
        Subtitle = subtitle ?? "";
    }

    public string Title { get; }
    public Color Color { get; }
    public string Icon { get; }
    public string Subtitle { get; }
    public virtual string? ImageUrl => null;
}

public class CarouselPhotoItem : CarouselItem
{
    public CarouselPhotoItem(string title, string imageUrl, string color)
        : base(title, color, "")
    {
        photoUrl = imageUrl;
    }

    readonly string photoUrl;
    public override string? ImageUrl => photoUrl;
}
