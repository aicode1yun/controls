namespace Shiny.Maui.Controls;

public partial class Overlay : ContentView
{
    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly ContentView overlayContainer;
    FrostedGlassView? blurBackdrop;

    public Overlay()
    {
        backdrop = new BoxView
        {
            Color = OverlayColor,
            Opacity = 0,
            IsVisible = false,
            InputTransparent = true
        };

        overlayContainer = new ContentView
        {
            IsVisible = false,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        rootGrid = new Grid();
        Content = rootGrid;
    }

    protected override void OnChildAdded(Element child)
    {
        base.OnChildAdded(child);
        RebuildLayout();
    }

    protected override void OnChildRemoved(Element child, int oldLogicalIndex)
    {
        base.OnChildRemoved(child, oldLogicalIndex);
        RebuildLayout();
    }

    void RebuildLayout()
    {
        // Ensure backdrop and overlay container are always the last children (on top)
        if (blurBackdrop != null && !rootGrid.Children.Contains(blurBackdrop))
            rootGrid.Children.Add(blurBackdrop);
        if (!rootGrid.Children.Contains(backdrop))
            rootGrid.Children.Add(backdrop);
        if (!rootGrid.Children.Contains(overlayContainer))
            rootGrid.Children.Add(overlayContainer);
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
                    Opacity = 0,
                    IsVisible = false,
                    InputTransparent = true
                };
                // Insert blur backdrop behind the color backdrop
                var idx = rootGrid.Children.IndexOf(backdrop);
                if (idx >= 0)
                    rootGrid.Children.Insert(idx, blurBackdrop);
                else
                    rootGrid.Children.Add(blurBackdrop);
            }
            else
            {
                blurBackdrop.BlurRadius = BlurRadius;
            }
        }
        else if (blurBackdrop != null)
        {
            rootGrid.Children.Remove(blurBackdrop);
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
        if (shown)
        {
            backdrop.InputTransparent = false;
            backdrop.IsVisible = true;
            overlayContainer.IsVisible = true;

            if (blurBackdrop != null)
            {
                blurBackdrop.InputTransparent = false;
                blurBackdrop.IsVisible = true;
                _ = blurBackdrop.FadeToAsync(1, AnimationDuration);
            }

            await backdrop.FadeToAsync(OverlayOpacity, AnimationDuration);
        }
        else
        {
            if (blurBackdrop != null)
                _ = blurBackdrop.FadeToAsync(0, AnimationDuration);

            await backdrop.FadeToAsync(0, AnimationDuration);
            backdrop.IsVisible = false;
            backdrop.InputTransparent = true;
            overlayContainer.IsVisible = false;

            if (blurBackdrop != null)
            {
                blurBackdrop.IsVisible = false;
                blurBackdrop.InputTransparent = true;
            }
        }
    }
}
