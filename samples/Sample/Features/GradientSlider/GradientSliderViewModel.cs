using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.GradientSlider;

public partial class GradientSliderViewModel : ObservableObject
{
    [ObservableProperty] double temperature = 50;
    [ObservableProperty] double intensity = 5;
    [ObservableProperty] double volume = 30;
    [ObservableProperty] double opacity = 0.75;
    [ObservableProperty] double fineValue = 2.5;
}
