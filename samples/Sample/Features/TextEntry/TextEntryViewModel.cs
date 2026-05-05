using CommunityToolkit.Mvvm.ComponentModel;
using Shiny;

namespace Sample.Features.TextEntry;

[ShellMap<TextEntryPage>(registerRoute: false)]
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
    [ObservableProperty] string maskedPhone = "";
    [ObservableProperty] string maskedCard = "";
    [ObservableProperty] string maskedDate = "";
    [ObservableProperty] string quantity = "0";
    [ObservableProperty] string score = "50";

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
