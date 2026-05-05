using Shiny.Maui.Controls.FloatingPanel;

namespace Shiny.Maui.Controls;

/// <summary>
/// A full-screen overlay control that integrates with OverlayHost/ShinyContentPage.
/// Place inside ShinyContentPage.Panels or an OverlayHost. When IsShown is true,
/// the backdrop dims (with optional blur) and custom content is displayed centered.
/// </summary>
public partial class Overlay : ContentView
{
    readonly ContentView overlayContainer;
    FrostedGlassView? blurBackdrop;
    bool isAnimating;

    public Overlay()
    {
        IsVisible = false;
        InputTransparent = false;

        overlayContainer = new ContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        Content = overlayContainer;
    }

    OverlayHost? GetOverlayHost()
    {
        Element? current = Parent;
        while (current is not null)
        {
            if (current is OverlayHost host)
                return host;
            current = current.Parent;
        }
        return null;
    }

    void OnBlurRadiusChanged()
    {
        if (BlurRadius > 0)
        {
            if (blurBackdrop == null)
            {
                blurBackdrop = new FrostedGlassView
                {
                    BlurRadius = BlurRadius,
                    TintColor = Colors.Transparent,
                    TintOpacity = 0,
                    IsVisible = false,
                    InputTransparent = true
                };
            }
            else
            {
                blurBackdrop.BlurRadius = BlurRadius;
            }
        }
        else
        {
            blurBackdrop = null;
        }
    }

    void UpdateOverlayContent()
    {
        if (OverlayContentTemplate == null)
        {
            overlayContainer.Content = null;
            return;
        }

        var content = OverlayContentTemplate.CreateContent() as View;
        overlayContainer.Content = content;
    }

    async void OnIsShownChanged(bool shown)
    {
        if (isAnimating) return;

        if (shown)
            await ShowAsync();
        else
            await HideAsync();
    }

    async Task ShowAsync()
    {
        isAnimating = true;

        var overlayHost = GetOverlayHost();
        if (overlayHost != null)
            overlayHost.ShowBackdrop(this, AnimationDuration);

        // Show blur layer in the overlay host (behind this view)
        if (blurBackdrop != null && overlayHost != null)
        {
            if (!overlayHost.Children.Contains(blurBackdrop))
            {
                var myIndex = overlayHost.Children.IndexOf(this);
                if (myIndex >= 0)
                    overlayHost.Children.Insert(myIndex, blurBackdrop);
                else
                    overlayHost.Children.Add(blurBackdrop);
            }
            blurBackdrop.IsVisible = true;
            _ = blurBackdrop.FadeToAsync(1, AnimationDuration);
        }

        IsVisible = true;
        Opacity = 0;
        await this.FadeToAsync(1, AnimationDuration);

        isAnimating = false;
    }

    async Task HideAsync()
    {
        isAnimating = true;

        var overlayHost = GetOverlayHost();
        if (overlayHost != null)
            overlayHost.HideBackdrop(this, AnimationDuration);

        if (blurBackdrop != null)
            _ = blurBackdrop.FadeToAsync(0, AnimationDuration);

        await this.FadeToAsync(0, AnimationDuration);
        IsVisible = false;

        if (blurBackdrop != null)
        {
            blurBackdrop.IsVisible = false;
            var host = GetOverlayHost();
            if (host != null && host.Children.Contains(blurBackdrop))
                host.Children.Remove(blurBackdrop);
        }

        isAnimating = false;
    }
}
