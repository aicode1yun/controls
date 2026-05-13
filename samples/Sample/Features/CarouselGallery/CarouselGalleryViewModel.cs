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
    string statusMessage = "Tap a card to select it";

    public ObservableCollection<CarouselItem> BasicItems { get; } = new(new[]
    {
        new CarouselItem("Action", "#E53E3E", "\U0001f3ac"),
        new CarouselItem("Comedy", "#DD6B20", "\U0001f602"),
        new CarouselItem("Drama", "#38A169", "\U0001f3ad"),
        new CarouselItem("Sci-Fi", "#3182CE", "\U0001f680"),
        new CarouselItem("Horror", "#805AD5", "\U0001f47b"),
        new CarouselItem("Romance", "#D53F8C", "\u2764\ufe0f"),
    });

    public ObservableCollection<CarouselItem> GradientItems { get; } = new(new[]
    {
        new CarouselItem("Mountains", "#2D3748", "\U0001f3d4\ufe0f", "Explore the peaks"),
        new CarouselItem("Ocean", "#2B6CB0", "\U0001f30a", "Deep blue sea"),
        new CarouselItem("Forest", "#276749", "\U0001f332", "Into the wild"),
        new CarouselItem("Desert", "#C05621", "\U0001f3dc\ufe0f", "Golden sands"),
        new CarouselItem("City", "#553C9A", "\U0001f303", "Urban nights"),
    });

    [RelayCommand]
    void ItemSelected(object item)
    {
        if (item is CarouselItem ci)
            StatusMessage = $"Selected: {ci.Title}";
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
}
