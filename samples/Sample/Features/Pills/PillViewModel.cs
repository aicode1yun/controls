using CommunityToolkit.Mvvm.ComponentModel;

using Shiny;

namespace Sample.Features.Pills;

[ShellMap<PillPage>(registerRoute: false)]
public partial class PillViewModel : ObservableObject
{
    [ObservableProperty]
    string customPillText = "Custom";
}
