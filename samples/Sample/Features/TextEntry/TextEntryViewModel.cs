using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.TextEntry;

public partial class TextEntryViewModel : ObservableObject
{
    [ObservableProperty] string firstName = "";
    [ObservableProperty] string lastName = "";
    [ObservableProperty] string searchText = "";
    [ObservableProperty] string password = "";
    [ObservableProperty] string bio = "";
    [ObservableProperty] string customText = "";
    [ObservableProperty] string phone = "";
    [ObservableProperty] string amount = "";

    [ObservableProperty] string email = "";
    [ObservableProperty] bool hasEmailError;
    [ObservableProperty] string? emailError;

    partial void OnEmailChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            HasEmailError = false;
            EmailError = null;
        }
        else if (!value.Contains('@'))
        {
            HasEmailError = true;
            EmailError = "Please enter a valid email address";
        }
        else
        {
            HasEmailError = false;
            EmailError = null;
        }
    }
}
