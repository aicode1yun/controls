namespace Shiny.Maui.Controls;

public class LoadingOverlay : Overlay
{
    readonly ActivityIndicator spinner;
    readonly ProgressBar progressBar;
    readonly Label messageLabel;
    readonly StackLayout contentLayout;

    public LoadingOverlay()
    {
        spinner = new ActivityIndicator
        {
            IsRunning = true,
            Color = Colors.White,
            HeightRequest = 48,
            WidthRequest = 48,
            HorizontalOptions = LayoutOptions.Center
        };

        progressBar = new ProgressBar
        {
            IsVisible = false,
            WidthRequest = 200,
            HorizontalOptions = LayoutOptions.Center,
            BarColor = Colors.White,
            TrackColor = Color.FromArgb("#FFFFFF33")
        };

        messageLabel = new Label
        {
            IsVisible = false,
            TextColor = Colors.White,
            FontSize = 14,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        contentLayout = new StackLayout
        {
            Spacing = 12,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { spinner, progressBar, messageLabel }
        };

        OverlayContentTemplate = new DataTemplate(() => contentLayout);
    }

    // IsIndeterminate
    public static readonly BindableProperty IsIndeterminateProperty = BindableProperty.Create(
        nameof(IsIndeterminate), typeof(bool), typeof(LoadingOverlay), true,
        propertyChanged: (b, _, n) => ((LoadingOverlay)b).OnModeChanged());
    public bool IsIndeterminate { get => (bool)GetValue(IsIndeterminateProperty); set => SetValue(IsIndeterminateProperty, value); }

    // Progress (0-100)
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress), typeof(double), typeof(LoadingOverlay), 0.0,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((LoadingOverlay)b).progressBar.Value = (double)n);
    public double Progress { get => (double)GetValue(ProgressProperty); set => SetValue(ProgressProperty, value); }

    // Message
    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        nameof(Message), typeof(string), typeof(LoadingOverlay), null,
        propertyChanged: (b, _, n) =>
        {
            var lo = (LoadingOverlay)b;
            var text = (string?)n;
            lo.messageLabel.Text = text;
            lo.messageLabel.IsVisible = !string.IsNullOrWhiteSpace(text);
        });
    public string? Message { get => (string?)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }

    // SpinnerColor
    public static readonly BindableProperty SpinnerColorProperty = BindableProperty.Create(
        nameof(SpinnerColor), typeof(Color), typeof(LoadingOverlay), Colors.White,
        propertyChanged: (b, _, n) => ((LoadingOverlay)b).spinner.Color = (Color)n);
    public Color SpinnerColor { get => (Color)GetValue(SpinnerColorProperty); set => SetValue(SpinnerColorProperty, value); }

    void OnModeChanged()
    {
        spinner.IsVisible = IsIndeterminate;
        spinner.IsRunning = IsIndeterminate;
        progressBar.IsVisible = !IsIndeterminate;
    }
}
