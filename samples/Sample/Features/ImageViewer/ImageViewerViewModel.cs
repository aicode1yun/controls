using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Shiny;

namespace Sample.Features.ImageViewer;

[ShellMap<ImageViewerPage>(registerRoute: false)]
public partial class ImageViewerViewModel : ObservableObject
{
    [ObservableProperty]
    bool isViewerOpen;

    [RelayCommand]
    void CloseViewer() => IsViewerOpen = false;
}
