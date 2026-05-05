using CommunityToolkit.Mvvm.ComponentModel;
using Shiny;

namespace Sample.Features.Slider;

[ShellMap<SliderPage>(registerRoute: false)]
public partial class SliderViewModel : ObservableObject
{
    [ObservableProperty] double temperature = 50;
    [ObservableProperty] double intensity = 5;
    [ObservableProperty] double volume = 30;
    [ObservableProperty] double opacity = 0.75;
    [ObservableProperty] double fineValue = 2.5;
}
